//===================================
// Code snippets from chapter 4 
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

#load "Common.fsx"
open Common

/// define some simple functions
module DefiningFunctions = 
    //>def-named1
    let add1 x = x + 1   // signature is: int -> int
    
    let add x y = x + y  // signature is: int -> int -> int
    //<
    
    //>def-named2
    // squarePlusOne : int -> int
    let squarePlusOne x = 
        let square = x * x
        square + 1
    //<

    //>def-named3    
    // areEqual : 'a -> 'a -> bool
    let areEqual x y = 
        (x = y)
    //<

    (*>def-generic-csharp
    static bool AreEqual<T>(T x, T y) 
    {
        return (x == y);
    }
    <*)


    (*>def-anon1
    fun x -> x + 1       // same as "add1" above
    fun x y -> x + y     // same as "add" above
    fun x -> (x % 2) = 0 // same as "isEven" above
    <*)


/// Examples of discriminated unions and record types
module CompositionOfTypes =

    //>FruitVariety
    type AppleVariety = 
        | GoldenDelicious
        | GrannySmith
        | Fuji

    type BananaVariety = 
        | Cavendish
        | GrosMichel
        | Manzano

    type CherryVariety = 
        | Montmorency
        | Bing
    //<

    //>FruitSalad
    type FruitSalad = {
        Apple: AppleVariety
        Banana: BananaVariety
        Cherries: CherryVariety
    }
    //<

    //>FruitSnack
    type FruitSnack = 
        | Apple of AppleVariety
        | Banana of BananaVariety
        | Cherries of CherryVariety
    //<

    //>SCU1
    type ProductCode = 
      | ProductCode of string
    //<

    module SCU2 =
        //>SCU2
        type ProductCode = ProductCode of string
        //<

module WorkingWithTypes =

    //>WorkingRecord1
    type Person = {First:string; Last:string}
    //<

    //>WorkingRecord2
    let aPerson = {First="Alex"; Last="Adams"}
    //<

    //>WorkingRecord3
    let {First=first; Last=last} = aPerson 
    //<

    
    module AlternatePropertySyntax =
        //>WorkingRecord4
        let first = aPerson.First
        let last = aPerson.Last
        //<
        

    //>WorkingUnion1
    type OrderQuantity =
        | UnitQuantity of int
        | KilogramQuantity of decimal
    //<

    //>WorkingUnion2
    let anOrderQtyInUnits = UnitQuantity 10
    let anOrderQtyInKg = KilogramQuantity 2.5M
    //<

    //>WorkingUnion3
    let printQuantity aOrderQty =
       match aOrderQty with
       | UnitQuantity uQty -> 
          printfn "%i units" uQty 
       | KilogramQuantity kgQty -> 
          printfn "%g kg" kgQty
    //<

    //>WorkingUnion4
    printQuantity anOrderQtyInUnits // "10 units"
    printQuantity anOrderQtyInKg    // "2.5 kg"
    //<

// ----------------------------------
// Building a domain model by composing types
// ----------------------------------
module BuildingADomainModel =

    // 1. Start with some wrappers for primitive types
    //>Payment1
    type CheckNumber = CheckNumber of int
    type CardNumber = CardNumber of string
    //<

    // 2. Next, build up some low level types
    //>Payment2
    type CardType = 
      Visa | Mastercard        // 'OR' type

    type CreditCardInfo = {    // 'AND' type (record)
      CardType : CardType 
      CardNumber : CardNumber  
      }
    //<

    // 3. Now build up a slightly higher level 'OR' type
    //>Payment3
    type PaymentMethod = 
      | Cash
      | Check of CheckNumber
      | Card of CreditCardInfo
    //<

    // 4. Define a few more basic types
    //>Payment4
    type PaymentAmount = PaymentAmount of decimal
    type Currency = EUR | USD
    //<

    // 5. The final record type is built from the smaller types
    //>Payment5
    type Payment = {
        Amount : PaymentAmount
        Currency:  Currency
        Method:  PaymentMethod  
        }
    //<


    // placeholders for code sample
    type UnpaidInvoice = Undefined
    type PaidInvoice = Undefined

    // 6. Pay for an unpaid invoice
    //>Payment6
    type PayInvoice = 
       UnpaidInvoice -> Payment -> PaidInvoice
    //<

    // 7. Or to convert a payment from one currency to another
    //>Payment7
    type ConvertPaymentCurrency = 
       Payment -> Currency -> Payment
    //<

module ModelingOptionalValues =
    
    //>Option1
    type Option<'a> =
       | Some of 'a
       | None
    //<

    //>Option2
    type PersonalName = {
       FirstName : string
       MiddleInitial: Option<string>  // optional
       LastName : string
       }
    //<

    module OptionPostfix = 
        //>Option3
        type PersonalName = {
           FirstName : string
           MiddleInitial: string option
           LastName : string
           }
        //<

module ModellingErrors =

    //>Errors1
    type Result<'Success,'Failure> = 
       | Ok of 'Success 
       | Error of 'Failure
    //<

    // placeholders for code sample
    type UnpaidInvoice = Undefined 
    type PaidInvoice = Undefined 
    type Payment = Undefined 

    //>Errors3
    type PaymentError = 
       | CardTypeNotRecognized 
       | PaymentRejected 
       | PaymentProviderOffline 
    //<

    //>Errors2
    type PayInvoice = 
       UnpaidInvoice -> Payment -> Result<PaidInvoice,PaymentError>
    //<


module ModelingNoValue =

    // placeholders for code sample
    type Customer = Undefined 

    //>Unit1
    type SaveCustomer = Customer -> unit
    //<

    //>Unit2
    type NextRandom = unit -> int
    //<


module ModelingLists = 

    // placeholders for code sample
    type OrderId = Undefined 
    type OrderLine = Undefined 

    //>List1
    type Order = {
       OrderId : OrderId
       Lines : OrderLine list // a collection
       }
    //<

    //>List2
    let aList = [1; 2; 3]
    //<

    //>List3
    let aNewList = 0 :: aList  // new list is [0;1;2;3]
    //<

    //>List4
    let printList1 aList =
       // matching against list literals
       match aList with
       | [] ->   
          printfn "list is empty"
       | [x] -> 
          printfn "list has one element: %A" x
       | [x;y] ->       // match using list literal
          printfn "list has two elements: %A and %A" x y
       | longerList ->  // match anything else
          printfn "list has more than two elements" 
    //<

    //>List5
    let printList2 aList =
       // matching against "cons"
       match aList with
       | [] ->   
          printfn "list is empty"
       | first::rest ->  
          printfn "list is non-empty with the first element being: %A" first
    //<

module FileOrganizationInOrder =

    //>FileOrganizationInOrder
    module Payments = 
      // simple types at the top of the file
      type CheckNumber = CheckNumber of int

      // domain types in the middle of the file
      type PaymentMethod = 
        | Cash
        | Check of CheckNumber // defined above
        | Card of DotDotDot

      // top-level types at the bottom of the file  
      type Payment = {
        Amount: DotDotDot
        Currency: DotDotDot
        Method: PaymentMethod  // defined above
        }
    //<

module FileOrganizationUsingRec =
    ()

    (*>FileOrganizationUsingRec
    module rec Payments = 
      type Payment = {
        Amount: DotDotDot
        Currency: DotDotDot
        Method: PaymentMethod  // defined BELOW
        }

      type PaymentMethod = 
        | Cash
        | Check of CheckNumber // defined BELOW
        | Card of DotDotDot

      type CheckNumber = CheckNumber of int
    <*)

module FileOrganizationOutOfOrder =

    //>FileOrganizationOutOfOrder 
    type Payment = {
        Amount: DotDotDot
        Currency:  DotDotDot
        Method:  PaymentMethod // defined BELOW
        }

    and PaymentMethod = 
      | Cash
      | Check of CheckNumber   // defined BELOW
      | Card of DotDotDot

    and CheckNumber = CheckNumber of int
    //<