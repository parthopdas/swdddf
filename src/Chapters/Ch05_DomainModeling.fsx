//===================================
// Code snippets from chapter 5
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


// ============================

module PrimitiveValues = 

    //>Prim1
    type CustomerId = 
      | CustomerId of int
    //<

    module OneLineScu = 
        //>Prim2
        type CustomerId = CustomerId of int
        //<

    //>Prim3
    type WidgetCode = WidgetCode of string  
    type UnitQuantity = UnitQuantity of int 
    type KilogramQuantity = KilogramQuantity of decimal 
    //<

    module OneLineScuWithComment = 
        //>Prim4
        type CustomerId = CustomerId of int
        //   ^type name   ^case label
        //<

module WorkingWithScus = 

    module OneLineScuWithComment = 
        //>Scu1
        type CustomerId = CustomerId of int
        //                ^this case name will be the constructor function
        //<

        //>Scu2
        let customerId = CustomerId 42
        //                ^this is a function with an int parameter
        //<

    //>Scu3
    // define some types
    type CustomerId = CustomerId of int
    type OrderId = OrderId of int

    // define some values
    let customerId = CustomerId 42
    let orderId = OrderId 42

    // try to compare them -- compiler error!
    // printfn "%b" (orderId = customerId) 
    //                      ^ This expression was expected to 
    //                      have type 'OrderId'
    //<

    //>Scu4
    // define a function using a CustomerId
    let processCustomerId (id:CustomerId) = dotDotDot

    // call it with an OrderId -- compiler error!
    // processCustomerId orderId
    //                ^ This expression was expected to 
    //                have type 'CustomerId' but here has 
    //                type 'OrderId'    
    //<

module WorkingWithScus2 =
    open WorkingWithScus

    //>Scu5
    // construct
    let customerId = CustomerId 42

    // deconstruct
    let (CustomerId innerValue) = customerId
    //              ^ innerValue is set to 42

    printfn "%i" innerValue  // prints "42"
    //<

    //>Scu6
    // deconstruct
    let processCustomerId (CustomerId innerValue) = 
        printfn "innerValue is %i" innerValue
   
    // function signature
    // val processCustomerId: CustomerId -> unit
    //<


module ScuPerformance = 

    //>ScuPerformance1
    type UnitQuantity = int
    //<

    (*>ScuPerformance2
    [<Struct>]
    type UnitQuantity = UnitQuantity of int  
    <*)

    //>ScuPerformance3
    type UnitQuantities = UnitQuantities of int[]
    //<


module ModelingWithRecords = 

    type CustomerInfo = Undefined
    type ShippingAddress = Undefined
    type BillingAddress = Undefined
    type OrderLine = Undefined
    type Price = Undefined

    //>ModelingWithRecords1
    type Order = {
        CustomerInfo : CustomerInfo
        ShippingAddress : ShippingAddress 
        BillingAddress : BillingAddress 
        OrderLines : OrderLine list
        AmountToBill : DotDotDot
        }
    //<

module ModelingUndefined = 

    //>ModelingUndefined1
    type Undefined = exn
    //<

    //>ModelingUndefined2
    type CustomerInfo = Undefined
    type ShippingAddress = Undefined
    type BillingAddress = Undefined
    type OrderLine = Undefined
    type BillingAmount = Undefined

    type Order = {
        CustomerInfo : CustomerInfo 
        ShippingAddress : ShippingAddress 
        BillingAddress : BillingAddress 
        OrderLines : OrderLine list
        AmountToBill : BillingAmount
        }
    //<

module ModelingWithChoice = 

    type WidgetCode = PrimitiveValues.WidgetCode
    type GizmoCode = Undefined
    type UnitQuantity = PrimitiveValues.UnitQuantity
    type KilogramQuantity = PrimitiveValues.KilogramQuantity


    //>ModelingWithChoice1
    type ProductCode =
        | Widget of WidgetCode
        | Gizmo of GizmoCode

    type OrderQuantity =
        | Unit of UnitQuantity
        | Kilogram of KilogramQuantity
    //<

module ModelingWithFunctions =     

    type UnvalidatedOrder = Undefined
    type ValidatedOrder = Undefined

    //>ModelingWithFunctions1
    type ValidateOrder = UnvalidatedOrder-> ValidatedOrder
    //<

    type AcknowledgmentSent = Undefined
    type OrderPlaced = Undefined
    type BillableOrderPlaced = Undefined

    //>ModelingWithFunctions2
    type PlaceOrderEvents = {
       AcknowledgmentSent : AcknowledgmentSent 
       OrderPlaced : OrderPlaced 
       BillableOrderPlaced : BillableOrderPlaced 
       }
    //<

    //>ModelingWithFunctions3
    type PlaceOrder = UnvalidatedOrder -> PlaceOrderEvents
    //<

    type OrderForm = Undefined
    type QuoteForm = Undefined

    //>ModelingWithFunctions4
    type EnvelopeContents = EnvelopeContents of string
    type CategorizedMail = 
        | Quote of QuoteForm
        | Order of OrderForm
        // etc

    type CategorizeInboundMail = EnvelopeContents -> CategorizedMail
    //<

    type ProductCatalog = Undefined
    type PricedOrder = Undefined

    //>ModelingWithFunctions5
    type CalculatePrices = OrderForm -> ProductCatalog -> PricedOrder
    //<


    module CalculatePricesV2 =
        //>ModelingWithFunctions6
        type CalculatePricesInput = {
            OrderForm : OrderForm
            ProductCatalog : ProductCatalog
            }
        //<

        //>ModelingWithFunctions7
        type CalculatePrices = CalculatePricesInput -> PricedOrder
        //<

module DocumentingEffects =

    type UnvalidatedOrder = Undefined
    type ValidatedOrder = Undefined

    //>DocumentingEffects1
    type ValidateOrder = UnvalidatedOrder -> ValidatedOrder
    //<

    module DocumentingEffectsV2 =

        //>DocumentingEffects2
        type ValidateOrder = 
          UnvalidatedOrder -> Result<ValidatedOrder,ValidationError list>

        and ValidationError = { 
          FieldName : string
          ErrorDescription : string
          }
        //<

    module DocumentingEffectsV3 =
        type ValidationError = Undefined

        //>DocumentingEffects3
        type ValidateOrder = 
           UnvalidatedOrder -> Async<Result<ValidatedOrder,ValidationError list>>
        //<

        //>DocumentingEffects4
        type ValidationResponse<'a> = Async<Result<'a,ValidationError list>>
        //<

    module DocumentingEffectsV4 =
        type ValidationResponse<'a> = DocumentingEffectsV3.ValidationResponse<'a>

        //>DocumentingEffects5
        type ValidateOrder = 
           UnvalidatedOrder -> ValidationResponse<ValidatedOrder>
        //<


module ValueObjects = 
    type WidgetCode = WidgetCode of string

    //>Value1
    let widgetCode1 = WidgetCode "W1234"
    let widgetCode2 = WidgetCode "W1234"
    printfn "%b" (widgetCode1 = widgetCode2)  // prints "true"
    //<

    type PersonalName = {
       FirstName : string
       LastName : string
       }

    //>Value2
    let name1 = {FirstName="Alex"; LastName="Adams"}
    let name2 = {FirstName="Alex"; LastName="Adams"}
    printfn "%b" (name1 = name2)  // prints "true"
    //<

    type UsPostalAddress = {
       StreetAddress : string
       City : string
       Zip : string
       }

    //>Value3
    let address1 = {StreetAddress="123 Main St"; City="New York"; Zip="90001"}
    let address2 = {StreetAddress="123 Main St"; City="New York"; Zip="90001"}
    printfn "%b" (address1 = address2)  // prints "true"
    //<

module Entities =

    //>Entity1
    type ContactId = ContactId of int

    type Contact = {
      ContactId : ContactId 
      PhoneNumber : DotDotDot
      EmailAddress: DotDotDot
      }
    //<

    module IdOutside = 
        //>Entity2
        // Info for the unpaid case (without id)
        type UnpaidInvoiceInfo = DotDotDot  

        // Info for the paid case (without id)
        type PaidInvoiceInfo = DotDotDot 

        // Combined information (without id)
        type InvoiceInfo = 
           | Unpaid of UnpaidInvoiceInfo
           | Paid of PaidInvoiceInfo

        // Id for invoice
        type InvoiceId = DotDotDot  

        // Top level invoice type   
        type Invoice = {
           InvoiceId : InvoiceId // "outside" the two child cases
           InvoiceInfo : InvoiceInfo
           }
        //<

    module IdInside = 
        type InvoiceId = DotDotDot

        //>Entity3
        type UnpaidInvoice = {
           InvoiceId : InvoiceId // id stored "inside" 
           // and other info for the unpaid case
           }
   
        type PaidInvoice = {
           InvoiceId : InvoiceId // id stored "inside" 
           // and other info for the paid case
           }

        // top level invoice type      
        type Invoice = 
           | Unpaid of UnpaidInvoice
           | Paid of PaidInvoice
        //<


        //>Entity4
        let invoice = Paid {InvoiceId = dotDotDot()}

        match invoice with
           | Unpaid unpaidInvoice ->
               printfn "The unpaid invoiceId is %A" unpaidInvoice.InvoiceId
           | Paid paidInvoice ->
               printfn "The paid invoiceId is %A" paidInvoice.InvoiceId
        //<

module EntityEqualityA = 

    type ContactId = ContactId of int
    type PhoneNumber = PhoneNumber of string
    type EmailAddress = EmailAddress of string

    //>EntityEqualityA1
    [<CustomEquality; NoComparison>]
    type Contact = {
      ContactId : ContactId 
      PhoneNumber : PhoneNumber
      EmailAddress: EmailAddress
      }
      with 
      override this.Equals(obj) =
        match obj with
        | :? Contact as c -> this.ContactId = c.ContactId 
        | _ -> false
      override this.GetHashCode() =
        hash this.ContactId
    //<

    //>EntityEqualityA2
    let contactId = ContactId 1

    let contact1 = {
        ContactId = contactId 
        PhoneNumber = PhoneNumber "123-456-7890"
        EmailAddress = EmailAddress "bob@example.com"
        }
    //<

    //>EntityEqualityA3
    // same contact, different email address
    let contact2 = {
        ContactId = contactId 
        PhoneNumber = PhoneNumber "123-456-7890"
        EmailAddress = EmailAddress "robert@example.com"
        }
    //<

    //>EntityEqualityA4
    // true even though the email addresses are different
    printfn "%b" (contact1 = contact2) 
    //<

module EntityEqualityB = 

    type ContactId = ContactId of int
    type PhoneNumber = PhoneNumber of string
    type EmailAddress = EmailAddress of string

    //>EntityEqualityB1
    [<NoEquality; NoComparison>]
    type Contact = {
        ContactId : ContactId 
        PhoneNumber : PhoneNumber
        EmailAddress: EmailAddress
        }
    //<

    //>EntityEqualityB2
    let contactId = ContactId 1

    let contact1 = {
        ContactId = contactId 
        PhoneNumber = PhoneNumber "123-456-7890"
        EmailAddress = EmailAddress "bob@example.com"
        }
    //<

    //>EntityEqualityB3
    // same contact, different email address
    let contact2 = {
        ContactId = contactId 
        PhoneNumber = PhoneNumber "123-456-7890"
        EmailAddress = EmailAddress "robert@example.com"
        }
    //<

    //>EntityEqualityB4
    // compiler error!
    // printfn "%b" (contact1 = contact2) 
    //            ^ the Contact type does not
    //              support equality  
    //<
    
    //>EntityEqualityB5
    // no compiler error
    printfn "%b" (contact1.ContactId = contact2.ContactId) // true
    //<

module EntityEqualityC = 
    
    type OrderId = OrderId of int
    type ProductId = ProductId of int

    //>EntityEqualityC1
    [<NoEquality;NoComparison>]
    type OrderLine = {
      OrderId : OrderId
      ProductId : ProductId
      Qty : int
      }
      with 
      member this.Key = 
         (this.OrderId,this.ProductId)
    //<

    //>EntityEqualityC2 
    let line1 = {
        OrderId = OrderId 1
        ProductId = ProductId 42
        Qty=99
        }
    let line2 = {
        OrderId = OrderId 1
        ProductId = ProductId 42
        Qty=100
        }
    //<

    //>EntityEqualityC3
    // compiler error!
    // printfn "%b" (line1 = line2) 
    //            ^ the OrderLine type does not
    //              support equality  
    //<

    //>EntityEqualityC4
    printfn "%b" (line1.Key = line2.Key) 
    //<


module Immutability =

    type PersonId = PersonId of int
    type Person = {
        PersonId : PersonId
        Name : string
        }


    //>Immutability1 
    let initialPerson = {PersonId=PersonId 42; Name="Joseph"}
    //<

    //>Immutability2
    let updatedPerson = {initialPerson with Name="Joe"}
    //<

    type Name = string

    module MutableFunction = 
        //>Immutability3
        type UpdateName = Person -> Name -> unit 
        //<

    //>Immutability4
    type UpdateName = Person -> Name -> Person
    //<


module Aggregates =

    type OrderLine = {
      OrderLineId : int
      Price : float
      // etc
      }

    type Order = {
      OrderLines : OrderLine list
      // etc
      }

    let findOrderLine orderLineId (lines:OrderLine list) =
        lines |> List.find (fun ol -> ol.OrderLineId = orderLineId )

    let replaceOrderLine orderLineId newOrderLine lines = 
        lines // no implementation!

    //>Aggregates1 
    /// We pass in three parameters: 
    /// * the top-level order
    /// * the id of the order line we want to change
    /// * the new price
    let changeOrderLinePrice order orderLineId newPrice =

       // 1. find the line to change using the orderLineId   
       let orderLine = order.OrderLines |> findOrderLine orderLineId 
   
       // 2. make a new version of the OrderLine with the new price
       let newOrderLine = {orderLine with Price = newPrice}                  

       // 3. create a new list of lines, replacing 
       //    the old line with the new line   
       let newOrderLines = 
           order.OrderLines |> replaceOrderLine orderLineId newOrderLine
   
       // 4. make a new version of the entire order, replacing
       //    all the old lines with the new lines
       let newOrder = {order with OrderLines = newOrderLines}                  
   
       // 5. return the new order
       newOrder
    //<

module BadAggregateReference =

    type OrderId = Undefined
    type Customer = Undefined
    type OrderLine = Undefined

    //>BadAggregateReference 
    type Order = {
      OrderId : OrderId
      Customer : Customer  // info about associated customer
      OrderLines : OrderLine list
      // etc
      }
    //<

module AggregateReferences =

    type OrderId = Undefined
    type CustomerId = Undefined
    type OrderLine = Undefined

    //>AggregateReferences
    type Order = {
      OrderId : OrderId
      CustomerId : CustomerId  // reference to associated customer
      OrderLines : OrderLine list
      // etc
      }
    //<

module PuttingItAllTogether = 

    (*>PuttingItAllTogether1 
    namespace OrderTaking.Domain 

    // types follow
    <*)
    
    //>PuttingItAllTogether2 
    // Product code related
    type WidgetCode = WidgetCode of string
        // constraint: starting with "W" then 4 digits
    type GizmoCode = GizmoCode of string
        // constraint: starting with "G" then 3 digits
    type ProductCode = 
        | Widget of WidgetCode 
        | Gizmo of GizmoCode 

    // Order Quantity related
    type UnitQuantity = UnitQuantity of int
    type KilogramQuantity = KilogramQuantity of decimal
    type OrderQuantity = 
        | Unit of UnitQuantity 
        | Kilos of KilogramQuantity
    //<

    //>PuttingItAllTogether3 
    type OrderId = Undefined
    type OrderLineId = Undefined
    type CustomerId = Undefined
    //<

    //>PuttingItAllTogether4 
    type CustomerInfo = Undefined
    type ShippingAddress = Undefined
    type BillingAddress = Undefined
    type Price = Undefined
    type BillingAmount = Undefined

    type Order = {
        Id : OrderId             // id for entity
        CustomerId : CustomerId  // customer reference
        ShippingAddress : ShippingAddress 
        BillingAddress : BillingAddress 
        OrderLines : OrderLine list
        AmountToBill : BillingAmount
        }

    and OrderLine = {
        Id : OrderLineId  // id for entity
        OrderId : OrderId
        ProductCode : ProductCode 
        OrderQuantity : OrderQuantity 
        Price : Price
        }
    //<

    //>PuttingItAllTogether5
    type UnvalidatedOrder = {
       OrderId : string
       CustomerInfo : DotDotDot
       ShippingAddress : DotDotDot
       //...
       }
    //<

    //>PuttingItAllTogether6a
    type PlaceOrderEvents = {
       AcknowledgmentSent : DotDotDot
       OrderPlaced : DotDotDot
       BillableOrderPlaced : DotDotDot
       }
    //<

    //>PuttingItAllTogether6b
    type PlaceOrderError =
       | ValidationError of ValidationError list
       | DotDotDot  // other errors

    and ValidationError = { 
        FieldName : string
        ErrorDescription : string
        }
    //<

    //>PuttingItAllTogether7
    /// The "Place Order" process
    type PlaceOrder = 
        UnvalidatedOrder -> Result<PlaceOrderEvents,PlaceOrderError>
    //<



