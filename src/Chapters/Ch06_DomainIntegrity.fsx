//===================================
// Code snippets from chapter 6
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

module SimpleValues1 =

    //>SimpleValues1 
    type WidgetCode = WidgetCode of string   // starting with "W" then 4 digits
    type UnitQuantity = UnitQuantity of int  // between 1 and 1000
    type KilogramQuantity = KilogramQuantity of decimal // between 0.05 and 100.00
    //<

module SimpleValues2 =

    //>SimpleValues2a 
    type UnitQuantity = private UnitQuantity of int
    //                  ^ private constructor
    //<

// F# VERSION DIFFERENCE
// Modules with the same name as a non-generic type will cause an error in versions of F# before v4.1 (VS2017) 
// so change the module definition to include a [<CompilationRepresentation>] attribute like this:
    (*>CompilationRepresentation
    type UnitQuantity = ...

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module UnitQuantity =
       ...
    <*)

    //>SimpleValues2b
    // define a module with the same name as the type 
    module UnitQuantity =

        /// Define a "smart constructor" for UnitQuantity	
        /// int -> Result<UnitQuantity,string>
        let create qty =
            if qty < 1 then
                // failure
                Error "UnitQuantity can not be negative" 
            else if qty > 1000 then
                // failure
                Error "UnitQuantity can not be more than 1000" 
            else
                // success -- construct the return value
                Ok (UnitQuantity qty)
    //<

    //>SimpleValues2c
        /// Return the wrapped value
        let value (UnitQuantity qty) = qty
    //<

    //>SimpleValues2d
        /// An Active pattern to replace 
        /// the constructor pattern matching
        let (|UnitQuantity|) unitQuantity =
            // extract the quantity
            let (UnitQuantity qty) = unitQuantity 
            // and return it
            qty
    //<

module SimpleTypesExample1 =
    open SimpleValues2

    //>SimpleValuesExample1
    // let unitQty = UnitQuantity 1
    //            ^ The union cases of the type 'UnitQuantity' 
    //              are not accessible 
    //<

module SimpleTypesExample2 =
    open SimpleValues2

    //>SimpleValuesExample2
    let unitQtyResult = UnitQuantity.create 1

    match unitQtyResult with
    | Error msg -> 
        printfn "Failure, Message is %s" msg
    | Ok uQty -> 
        printfn "Success. Value is %A" uQty
        let innerValue = UnitQuantity.value uQty
        printfn "innerValue is %i" innerValue 
    //<

module UnitsOfMeasure =

    //>UOM1 
    [<Measure>] 
    type kg

    [<Measure>] 
    type m
    //<

    //>UOM2 
    let fiveKilos = 5.0<kg>
    let fiveMeters = 5.0<m>
    //<

    //>UOM3 
    // compiler error 
    // fiveKilos = fiveMeters   
    //          ^ Expecting a float<kg> but given a float<m> 

    let listOfWeights = [
       fiveKilos
       // fiveMeters  // <-- compiler error
       //             The unit of measure 'kg' 
       //             does not match the unit of measure 'm'
       ]
    //<

    //>UOM4 
    type KilogramQuantity = KilogramQuantity of decimal<kg>
    //<

module Invariants = 

    //>Invariants1 
    type NonEmptyList<'a> = {
       First: 'a
       Rest: 'a list
       }
    //<

    type OrderLine = Undefined

    //>Invariants2 
    type Order = {
      //...
      OrderLines : NonEmptyList<OrderLine>
      //...
      }
    //<

module BusinessRuleImplementation1 = 

    type EmailAddress = Undefined

    //>BusinessRuleImplementation1 
    type CustomerEmail = {
      EmailAddress : EmailAddress
      IsVerified : bool
      }
    //<

module BusinessRuleImplementation2 = 

    type EmailAddress = Undefined

    //>BusinessRuleImplementation2 
    type CustomerEmail = 
      | Unverified of EmailAddress 
      | Verified of EmailAddress 
    //<

module BusinessRuleImplementation3 = 

    type EmailAddress = Undefined
    type VerifiedEmailAddress = Undefined

    //>BusinessRuleImplementation3a 
    type CustomerEmail = 
      | Unverified of EmailAddress 
      | Verified of VerifiedEmailAddress // different from normal EmailAddress
    //<

    //>BusinessRuleImplementation3b 
    type SendPasswordResetEmail = VerifiedEmailAddress -> DotDotDot
    //<

module ContactRuleImplementation1 = 

    type Name = Undefined
    type EmailContactInfo = Undefined
    type PostalContactInfo = Undefined

    //>ContactRuleImplementation1 
    type Contact = {
       Name: Name
       Email: EmailContactInfo
       Address: PostalContactInfo  
       }
    //<

module ContactRuleImplementation2 = 

    type Name = Undefined
    type EmailContactInfo = Undefined
    type PostalContactInfo = Undefined

    //>ContactRuleImplementation2 
    type Contact = {
       Name: Name
       Email: EmailContactInfo option
       Address: PostalContactInfo option  
       }
    //<

module ContactRuleImplementation3 = 

    type Name = Undefined
    type EmailContactInfo = Undefined
    type PostalContactInfo = Undefined

    //>ContactRuleImplementation3a 
    type BothContactMethods = {
       Email: EmailContactInfo
       Address : PostalContactInfo
       }

    type ContactInfo = 
        | EmailOnly of EmailContactInfo
        | AddrOnly of PostalContactInfo
        | EmailAndAddr of BothContactMethods
    //<

    //>ContactRuleImplementation3b 
    type Contact = {
        Name: Name
        ContactInfo : ContactInfo
        }
    //<


module IllegalStatesInOurDomain = 

    //>IllegalStatesInOurDomain1 
    type UnvalidatedAddress = DotDotDot

    type ValidatedAddress = PrivateDotDotDot
    //<

    //>IllegalStatesInOurDomain2 
    type AddressValidationService = 
       UnvalidatedAddress -> ValidatedAddress option 
    //<
    
    //>IllegalStatesInOurDomain3 
    type UnvalidatedOrder = {
        //...
        ShippingAddress : UnvalidatedAddress
        //...
        }

    type ValidatedOrder = {
        //...
        ShippingAddress : ValidatedAddress
        //...
        }
    //<

module Consistency = 

    type OrderLine = {
      OrderLineId : int
      Price : float
      // etc
      }

    type Order = {
      OrderLines : OrderLine list
      AmountToBill : float
      // etc
      }

    let findOrderLine orderLineId (lines:OrderLine list) =
        lines |> List.find (fun ol -> ol.OrderLineId = orderLineId )

    let replaceOrderLine orderLineId newOrderLine lines = 
        lines // no implementation!

    //>Consistency1
    /// We pass in three parameters: 
    /// * the top-level order
    /// * the id of the order line we want to change
    /// * the new price
    let changeOrderLinePrice order orderLineId newPrice =

       // find orderLine in order.OrderLines using orderLineId   
       let orderLine = order.OrderLines |> findOrderLine orderLineId 
   
       // make a new version of the OrderLine with new price
       let newOrderLine = {orderLine with Price = newPrice}                  

       // create new list of lines, replacing old line with new line   
       let newOrderLines = 
           order.OrderLines |> replaceOrderLine orderLineId newOrderLine

       // make a new AmountToBill
       let newAmountToBill = newOrderLines |> List.sumBy (fun line -> line.Price)
   
       // make a new version of the order with the new lines
       let newOrder = {
            order with 
              OrderLines = newOrderLines
              AmountToBill = newAmountToBill
            }
   
       // return the new order
       newOrder
    //<


    type MoneyTransferId = Undefined
    type AccountId = Undefined
    type Money = Undefined

    //>Consistency2
    type MoneyTransfer = {
        Id: MoneyTransferId
        ToAccount : AccountId
        FromAccount : AccountId
        Amount: Money
        }
    //<