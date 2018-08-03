//===================================
// Code snippets from chapter 11
//
// * DO NOT ATTEMPT TO RUN THIS CODE! 
//
// This is *not* meant to be a starting point for your own code -- it is just the text of the snippets.
// Many functions are stubbed out and will cause compile errors when used!
//
// For proper working code to look at, please see the `OrderTaking` project elsewhere in this solution.
//
// NOTES:
//
// * The angle brackets after comments ( `//>` and `//<`) are used to delimit the code snippets
//   for the book. Please ignore any code outside these delimiters -- this extra code is added just to make the snippets compile!
// * Because the same bits of code appear multiple times, I have had to create lots of different scopes
//   using modules and "do" blocks so that the names don't collide. This is NOT how real code would be written :)
//
//===================================

open System

#load "Common.fsx"
open Common



module SerializationWorkflow =

    //>SerializationWorkflow1 
    type MyInputType = DotDotDot
    type MyOutputType = DotDotDot

    type Workflow = MyInputType -> MyOutputType
    //<


    //>SerializationWorkflow2 
    type JsonString = string
    type MyInputDto = DotDotDot

    type DeserializeInputDto = JsonString -> MyInputDto
    type InputDtoToDomain = MyInputDto -> MyInputType
    //<

    //>SerializationWorkflow3 
    type MyOutputDto = DotDotDot

    type OutputDtoFromDomain = MyOutputType -> MyOutputDto
    type SerializeOutputDto = MyOutputDto -> JsonString
    //<

    let deserializeInputDto : DeserializeInputDto = fun _ -> notImplemented() 
    let inputDtoToDomain : InputDtoToDomain = fun _ -> notImplemented()
    let workflow : Workflow = fun _ -> notImplemented()
    let outputDtoFromDomain : OutputDtoFromDomain  = fun _ -> notImplemented()
    let serializeOutputDto : SerializeOutputDto  = fun _ -> notImplemented()

    //>SerializationWorkflow4 
    let workflowWithSerialization jsonString = 
        jsonString
        |> deserializeInputDto   // JSON to DTO
        |> inputDtoToDomain      // DTO to domain object
        |> workflow              // the core workflow in the domain
        |> outputDtoFromDomain   // Domain object to DTO
        |> serializeOutputDto    // DTO to JSON
        // final output is another JsonString
    //<

module SerializationDomain =

    //>SerializationDomain
    module Domain = // our domain-driven types

        /// constrained to be not null and at most 50 chars
        type String50 = String50 of string 

        /// constrained to be bigger than 1/1/1900 and less than today's date
        type Birthdate = Birthdate of DateTime 

        /// Domain type
        type Person = {
           First: String50
           Last: String50
           Birthdate : Birthdate
           }
    //<


module SerializationDto =
    open SerializationDomain 

    //>SerializationDto
    /// A module to group all the DTO-related 
    /// types and functions.
    module Dto = 

        type Person = {
           First: string
           Last: string
           Birthdate : DateTime
           }
    //<

    module String50 =
        open Domain

        //>CreateString50 
        let create fieldName str : Result<String50,string> = 
            if String.IsNullOrEmpty(str) then
                Error (fieldName + " must be non-empty")
            elif str.Length > 50 then
                Error (fieldName + " must be less that 50 chars")
            else
                Ok (String50 str)
        //<

        let value (String50 str) = str

    module Birthdate =
        open Domain

        let create (date:DateTime) : Result<Birthdate,string> = 
            if date.Year < 1900 then
                Error "Birthdate must be after 1900"
            elif date.Year > DateTime.Now.Year then
                Error "Birthdate must not be in the future"
            else
                Ok (Birthdate date) // no error checking

        let value (Birthdate dt) = dt


    module SerializationFunctionsSkeletion =
        module Domain =
            type Person = DotDotDot

        //>SerializationFunctions1
        module Dto = 

            module Person =
                let fromDomain (person:Domain.Person) :Dto.Person = 
                    dotDotDot()
            
                let toDomain (dto:Dto.Person) :Result<Domain.Person,string> =
                    dotDotDot()
        //<


    module PersonDto =

        //>SerializationFunctions2
        let fromDomain (person:Domain.Person) :Dto.Person =
           // get the primitive values from the domain object
           let first = person.First |> String50.value
           let last = person.Last |> String50.value
           let birthdate = person.Birthdate |> Birthdate.value 
       
           // combine the components to create the DTO
           {First = first; Last = last; Birthdate = birthdate}
        //<
            
        //>SerializationFunctions3
        let toDomain (dto:Dto.Person) :Result<Domain.Person,string> =
            result {
                // get each (validated) simple type from the DTO as a success or failure 
                let! first = dto.First |> String50.create "First"
                let! last = dto.Last |> String50.create "Last"
                let! birthdate = dto.Birthdate |> Birthdate.create

                // combine the components to create the domain object
                return {
                    First = first
                    Last = last
                    Birthdate = birthdate
                }
            }
          
        //<

#I "../../packages/Newtonsoft.Json/lib/net40"
#r "Newtonsoft.Json"
(*
/// or dummy version to make code compile!
module Newtonsoft =
    module Json =
        type JsonConvert() =
            static member SerializeObject<'a> x = failwith "not implemented"
            static member DeserializeObject<'a> x = failwith "not implemented"
*)

//>JsonWrapper
module Json =

    open Newtonsoft.Json
    
    let serialize obj =
        JsonConvert.SerializeObject obj

    let deserialize<'a> str =
        try
           JsonConvert.DeserializeObject<'a> str
           |> Result.Ok
        with
        // catch all exceptions and convert to Result
        | ex -> Result.Error ex  
//<

module SerializationComplete =
    open SerializationDomain
    open SerializationDto
    open Json

    module Dto = 
        module Person =
            let fromDomain (person:Domain.Person) :Dto.Person = 
                dotDotDot()
            let toDomain (dto:Dto.Person) :Result<Domain.Person,string> =
                dotDotDot()
     
    //>SerializationComplete
    /// Serialize a Person into a JSON string
    let jsonFromDomain (person:Domain.Person) = 
        person
        |> Dto.Person.fromDomain
        |> Json.serialize
    //<
    
    open Domain

    //>SerializationComplete_Test 
    // input to test with
    let person : Domain.Person = {
        First = String50 "Alex"
        Last = String50 "Adams" 
        Birthdate = Birthdate (DateTime(1980,1,1))
        }

    // use the serialization pipeline
    jsonFromDomain person 

    // The output is
    // "{"First":"Alex","Last":"Adams","Birthdate":"1980-01-01T00:00:00"}"
    //<

    //>DeserializationComplete
    type DtoError =
        | ValidationError of string 
        | DeserializationException of exn

    /// Deserialize a JSON string into a Person    
    let jsonToDomain jsonString :Result<Domain.Person,DtoError> = 
        result {
            let! deserializedValue = 
                jsonString
                |> Json.deserialize 
                |> Result.mapError DeserializationException
        
            let! domainValue = 
                deserializedValue 
                |> Dto.Person.toDomain 
                |> Result.mapError ValidationError

            return domainValue
            }
    //<


    //>DeserializationSuccess
    // JSON string to test with
    let jsonPerson = """{
        "First": "Alex",
        "Last": "Adams",
        "Birthdate": "1980-01-01T00:00:00"
        }"""

    // call the deserialization pipeline
    jsonToDomain jsonPerson |> printfn "%A"

    // The output is:
    //  Ok {First = String50 "Alex";
    //      Last = String50 "Adams";
    //      Birthdate = Birthdate 01/01/1980 00:00:00;}
    //<

    //>DeserializationFailure
    let jsonPersonWithErrors = """{
        "First": "",
        "Last": "Adams",
        "Birthdate": "1776-01-01T00:00:00"
        }"""

    // call the deserialization pipeline
    jsonToDomain jsonPersonWithErrors |> printfn "%A"
    
    // The output is:
    //  Error (ValidationError [
    //        "First must be non-empty" 
    //        ])
    //<

#r "System.Runtime.Serialization"

module OtherSerializers =
    open SerializationDomain
    open SerializationDto
    open System.Runtime.Serialization

    //>DataContractSerializer
    module Dto = 
        [<DataContract>]
        type Person = {
            [<field: DataMember>]
            First: string
            [<field: DataMember>]
            Last: string
            [<field: DataMember>]
            Birthdate : DateTime
            } 
    //<

    // test it
    let personDto : Dto.Person= {
        First = "Alex"
        Last = "Adams" 
        Birthdate = DateTime(1980,1,1)
        }

    let ser = new DataContractSerializer(typeof<Dto.Person>)
    let sr = new IO.StringWriter()
    let xmlWriter= new Xml.XmlTextWriter(sr)
    ser.WriteObject(xmlWriter,personDto) 
    sr.ToString() |> printfn "%s"

// <PersonDto 
//   xmlns:i="http://www.w3.org/2001/XMLSchema-instance" 
//   xmlns="http://schemas.datacontract.org/2004/07/">
//   <Birthdate_x0040_>1980-01-01T00:00:00</Birthdate_x0040_>
//   <First_x0040_>Alex</First_x0040_>
//   <Last_x0040_>Adams</Last_x0040_>
// </PersonDto>

 
module DtoGuidelines =

    //>DtoGuidelines-SCU
    type ProductCode = ProductCode of string
    //<

    //>DtoGuidelines-Record
    /// Domain types
    type OrderLineId = OrderLineId of int
    type OrderLineQty = OrderLineQty of int
    type OrderLine = {
        OrderLineId : OrderLineId
        ProductCode : ProductCode
        Quantity : OrderLineQty option
        Description : string option
        }

    /// Corresponding DTO type 
    type OrderLineDto = {
        OrderLineId : int
        ProductCode : string
        Quantity : Nullable<int>
        Description : string 
        }
    //<

    //>DtoGuidelines-Collections
    /// Domain type
    type Order = {
        //...
        Lines : OrderLine list
        }

    /// Corresponding DTO type
    type OrderDto = {
        //...
        Lines : OrderLineDto[] 
        }
    //<

    //>DtoGuidelines-Map1
    /// Domain type
    type Price = Price of decimal
    type PriceLookup = Map<ProductCode,Price>

    /// DTO type to represent a map
    type PriceLookupPair = {
        Key : string
        Value : decimal
        }
    type PriceLookupDto = {
        KVPairs : PriceLookupPair []
        }
    //<

    module MapAlternative =
        //>DtoGuidelines-Map2
        /// Alternative DTO type to represent a map
        type PriceLookupDto = {
            Keys : string []
            Values : decimal []
            }
        //<

    module EnumExample = 
        //>DtoGuidelines-Enum
        /// Domain type
        type Color = 
            | Red
            | Green
            | Blue

        /// Corresponding DTO type
        type ColorDto = 
            | Red = 1
            | Green = 2
            | Blue = 3
        //<

        //>DtoGuidelines-Enum2
        let toDomain dto : Result<Color,_> =
           match dto with
           | ColorDto.Red -> Ok Color.Red
           | ColorDto.Green -> Ok Color.Green
           | ColorDto.Blue -> Ok Color.Blue
           | _ -> Error (sprintf "Color %O is not one of Red,Green,Blue" dto)
        //<   

    module TupleExample = 

        //>DtoGuidelines-Tuple
        /// Components of tuple
        type Suit = Heart | Spade | Diamond | Club
        type Rank = Ace | Two | Queen | King // incomplete for clarity

        // Tuple
        type Card = Suit * Rank

        /// Corresponding DTO types
        type SuitDto = Heart = 1 | Spade = 2 | Diamond = 3 | Club = 4 
        type RankDto = Ace = 1 | Two = 2 | Queen = 12 | King = 13
        type CardDto = {
            Suit : SuitDto
            Rank : RankDto
            }
        //<   

    module ChoiceExample = 
        type String50 = String50 of string
        
        module String50 =
            let value (String50 str) = str

            let create str : Result<String50,string> = 
                failwith "not implemented"

        //>DtoGuidelines-Choice1
        /// Domain types
        type Name = {
            First : String50
            Last : String50
            }
    
        type Example = 
            | A 
            | B of int
            | C of string list
            | D of Name
        //<   

        //>DtoGuidelines-Choice2
        /// Corresponding DTO types
        type NameDto = {
            First : string
            Last : string
            }
    
        type ExampleDto = {
            Tag : string // one of "A","B", "C", "D" 
            // no data for A case
            BData : Nullable<int>  // data for B case
            CData : string[]       // data for C case
            DData : NameDto        // data for D case 
            }
        //<   

        //>DtoGuidelines-Choice3
        let nameDtoFromDomain (name:Name) :NameDto =
            let first = name.First |> String50.value
            let last = name.Last |> String50.value
            {First=first; Last=last}

        let fromDomain (domainObj:Example) :ExampleDto =
           let nullBData = Nullable()
           let nullCData = null
           let nullDData = Unchecked.defaultof<NameDto>
           match domainObj with
           | A -> 
               {Tag="A"; BData=nullBData; CData=nullCData; DData=nullDData}
           | B i ->
               let bdata = Nullable i
               {Tag="B"; BData=bdata; CData=nullCData; DData=nullDData}
           | C strList -> 
               let cdata = strList |> List.toArray
               {Tag="C"; BData=nullBData; CData=cdata; DData=nullDData}
           | D name -> 
               let ddata = name |> nameDtoFromDomain
               {Tag="D"; BData=nullBData; CData=nullCData; DData=ddata}
        //<   

        //>DtoGuidelines-Choice4
        let nameDtoToDomain (nameDto:NameDto) :Result<Name,string> =
            result {
                let! first = nameDto.First |> String50.create 
                let! last = nameDto.Last |> String50.create  
                return {First=first; Last=last}
            }

        let toDomain dto : Result<Example,string> =
           match dto.Tag with
            | "A" -> 
                Ok A 
            | "B" -> 
                if dto.BData.HasValue then
                    dto.BData.Value |> B |> Ok
                else
                    Error "B data not expected to be null"
            | "C" -> 
                match dto.CData with
                | null -> 
                    Error "C data not expected to be null"
                | _ -> 
                    dto.CData |> Array.toList |> C |> Ok 
            | "D" -> 
                match box dto.DData with
                | null -> 
                    Error "D data not expected to be null"
                | _ -> 
                    dto.DData 
                    |> nameDtoToDomain  // returns Result...
                    |> Result.map D     // ...so must use "map"
            | _ ->
                // all other cases
                let msg = sprintf "Tag '%s' not recognized" dto.Tag 
                Error msg
        //<   

    module ChoiceExampleAsMap = 
        open ChoiceExample  
        open System.Collections.Generic

        //>DtoGuidelines-SerializeMap1
        let nameDtoFromDomain (name:Name) :IDictionary<string,obj> =
            let first = name.First |> String50.value :> obj
            let last = name.Last |> String50.value :> obj
            [
                ("First",first)
                ("Last",last)
            ] |> dict
        //<
        
        //>DtoGuidelines-SerializeMap2
        let fromDomain (domainObj:Example) :IDictionary<string,obj> =
           match domainObj with
           | A -> 
               [ ("A",null) ] |> dict
           | B i ->
               let bdata = Nullable i :> obj
               [ ("B",bdata) ] |> dict
           | C strList -> 
               let cdata = strList |> List.toArray :> obj
               [ ("C",cdata) ] |> dict
           | D name -> 
               let ddata = name |> nameDtoFromDomain :> obj
               [ ("D",ddata) ] |> dict
        //<   

        //>DtoGuidelines-GetKey
        let getValue key (dict:IDictionary<string,obj>) :Result<'a,string> =
            match dict.TryGetValue key with
            | (true,value) ->  // key found!
                try 
                    // downcast to the type 'a and return Ok
                    (value :?> 'a) |> Ok
                with
                | :? InvalidCastException -> 
                    // the cast failed
                    let typeName = typeof<'a>.Name
                    let msg = sprintf "Value could not be cast to %s" typeName 
                    Error msg
            | (false,_) ->     // key not found
                let msg = sprintf "Key '%s' not found" key
                Error msg
        //<

        //>DtoGuidelines-DeserializeMap1
        let nameDtoToDomain (nameDto:IDictionary<string,obj>) :Result<Name,string> =
            result {
                let! firstStr = nameDto |> getValue "First"
                let! first = firstStr |> String50.create 
                let! lastStr = nameDto |> getValue "Last"
                let! last = lastStr |> String50.create 
                return {First=first; Last=last}
            }
        //<

        //>DtoGuidelines-DeserializeMap2
        let toDomain (dto:IDictionary<string,obj>) : Result<Example,string> =
            if dto.ContainsKey "A" then
                Ok A    // no extra data needed
            elif dto.ContainsKey "B" then
                result {
                    let! bData = dto |> getValue "B" // might fail
                    return B bData 
                    }
            elif dto.ContainsKey "C" then
                result {
                    let! cData = dto |> getValue "C" // might fail 
                    return cData |> Array.toList |> C 
                    }
            elif dto.ContainsKey "D" then
                result {
                    let! dData = dto |> getValue "D" // might fail 
                    let! name = dData |> nameDtoToDomain  // might also fail
                    return name |> D
                    }
            else
                // all other cases
                let msg = sprintf "No union case recognized" 
                Error msg
        //<   

            
    module GenericsExample = 
        //>DtoGuidelines-Generics1
        type ResultDto<'OkData,'ErrorData when 'OkData : null and 'ErrorData: null> = {
            IsError : bool  // replaces "Tag" field
            OkData : 'OkData 
            ErrorData : 'ErrorData
            }
        //<   

        type PlaceOrderEventDto = Undefined
        type PlaceOrderErrorDto = Undefined

        //>DtoGuidelines-Generics2
        type PlaceOrderResultDto = {
            IsError : bool  
            OkData : PlaceOrderEventDto[]
            ErrorData : PlaceOrderErrorDto
            }
        //<   
