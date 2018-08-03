//===================================
// Code snippets from chapter 7
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

//>AsyncResult
type AsyncResult<'success,'failure> = Async<Result<'success,'failure>>
//<

module Input = 

    type UnvalidatedCustomerInfo  = Undefined
    type UnvalidatedAddress = Undefined

    //>Input1
    type UnvalidatedOrder = {
       OrderId : string
       CustomerInfo : UnvalidatedCustomerInfo  
       ShippingAddress : UnvalidatedAddress 
       //...
       }
    //<

module Commands1 =
    open System
    type UnvalidatedOrder = Undefined

    //>Commands1
    type PlaceOrder = {
       OrderForm : UnvalidatedOrder
       Timestamp: DateTime
       UserId: string
       // etc
       }
    //<
    

module Commands2 =
    open System
    type UnvalidatedOrder = Undefined

    //>Commands2a
    type Command<'data> = {
       Data : 'data
       Timestamp: DateTime
       UserId: string
       // etc
       }
    //<

    //>Commands2b
    type PlaceOrder = Command<UnvalidatedOrder>
    //<

    type ChangeOrder = Undefined
    type CancelOrder = Undefined

    //>Commands3
    type OrderTakingCommand = 
       | Place of PlaceOrder
       | Change of ChangeOrder 
       | Cancel of CancelOrder
    //<

module States1 =

    type OrderId = Undefined

    //>States1
    type Order = {
      OrderId : OrderId
      //...
      IsValidated : bool  // set when validated
      IsPriced : bool     // set when priced
      AmountToBill : decimal option // also set when priced
      }
    //<    

module States2 =

    type OrderId = Undefined
    type CustomerInfo = Undefined
    type Address = Undefined
    type ValidatedOrderLine = Undefined

    //>States2a
    type ValidatedOrder = {
       OrderId : OrderId
       CustomerInfo : CustomerInfo
       ShippingAddress : Address
       BillingAddress : Address
       OrderLines : ValidatedOrderLine list
       }
    //<    

    type PricedOrderLine = Undefined
    type BillingAmount = Undefined

    //>States2b
    type PricedOrder = {
       OrderId : DotDotDot
       CustomerInfo : CustomerInfo
       ShippingAddress : Address
       BillingAddress : Address
       // different from ValidatedOrder
       OrderLines : PricedOrderLine list 
       AmountToBill : BillingAmount      
       }
    //<

    type UnvalidatedOrder = DotDotDot

    //>States2d
    type Order =
       | Unvalidated of UnvalidatedOrder
       | Validated of ValidatedOrder
       | Priced of PricedOrder
       // etc
    //<

module ShoppingCart = 

    //>ShoppingCart1 
    type Item = DotDotDot
    type ActiveCartData = { UnpaidItems: Item list }
    type PaidCartData = { PaidItems: Item list; Payment: float }

    type ShoppingCart = 
        | EmptyCart  // no data
        | ActiveCart of ActiveCartData
        | PaidCart of PaidCartData
    //<

    //>ShoppingCart2 
    let addItem cart item = 
        match cart with
        | EmptyCart -> 
            // create a new active cart with one item
            ActiveCart {UnpaidItems=[item]}

        | ActiveCart {UnpaidItems=existingItems} -> 
            // create a new ActiveCart with the item added
            ActiveCart {UnpaidItems = item :: existingItems}

        | PaidCart _ ->  
            // ignore
            cart
    //<

    //>ShoppingCart3 
    let makePayment cart payment = 
        match cart with
        | EmptyCart -> 
            // ignore
            cart

        | ActiveCart {UnpaidItems=existingItems} -> 
            // create a new PaidCart with the payment
            PaidCart {PaidItems = existingItems; Payment=payment}

        | PaidCart _ ->  
            // ignore
            cart
    //<

module OrderPlacingWorkflow =

    type ProductCode = Undefined

    //>Workflow1a
    type CheckProductCodeExists = 
        ProductCode -> bool
        // ^input      ^output
    //<

    type Address = Undefined
    type UnvalidatedAddress = Undefined

    //>Workflow1b2
    type CheckedAddress = CheckedAddress of UnvalidatedAddress
    //<

    //>Workflow1b
    type AddressValidationError = AddressValidationError of string

    type CheckAddressExists = 
        UnvalidatedAddress -> Result<CheckedAddress,AddressValidationError>
        // ^input                    ^output
    //<

    type UnvalidatedOrder = Undefined
    type ValidatedOrder = Undefined
    type ValidationError = Undefined

    //>Workflow1c
    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // dependency
          -> UnvalidatedOrder     // input
          -> Result<ValidatedOrder,ValidationError>  // output
    //<

    type Price = Undefined

    //>Workflow2a
    type GetProductPrice = 
        ProductCode -> Price
    //<

    type PricedOrder = Undefined

    //>Workflow2b
    type PriceOrder = 
        GetProductPrice      // dependency
          -> ValidatedOrder  // input
          -> PricedOrder     // output
    //<

    type EmailAddress = EmailAddress of string

    //>Workflow3a
    type HtmlString = 
        HtmlString of string

    type OrderAcknowledgment = {
        EmailAddress : EmailAddress
        Letter : HtmlString 
        }
    //<

    //>Workflow3b
    type CreateOrderAcknowledgmentLetter =
        PricedOrder -> HtmlString
    //<

    module SendOrderAcknowledgmentUnit =
        //>Workflow3c
        type SendOrderAcknowledgment =
            OrderAcknowledgment -> unit
        //<

    module SendOrderAcknowledgmentBool =
        //>Workflow3d
        type SendOrderAcknowledgment =
            OrderAcknowledgment -> bool
        //<

    module SendOrderAcknowledgmentEnum =
        //>Workflow3e
        type SendResult = Sent | NotSent

        type SendOrderAcknowledgment =
            OrderAcknowledgment -> SendResult 
        //<

    module SendOrderAcknowledgmentOption =
        type OrderAcknowledgmentSent = Undefined

        //>Workflow3f
        type SendOrderAcknowledgment =
            OrderAcknowledgment -> OrderAcknowledgmentSent option
        //<

   
    type OrderId = Undefined
    type SendOrderAcknowledgment = SendOrderAcknowledgmentEnum.SendOrderAcknowledgment          

    //>Workflow3g
    type OrderAcknowledgmentSent = {
        OrderId : OrderId
        EmailAddress : EmailAddress 
        }
    //<

    //>Workflow3h
    type AcknowledgeOrder = 
        CreateOrderAcknowledgmentLetter     // dependency
          -> SendOrderAcknowledgment        // dependency
          -> PricedOrder                    // input
          -> OrderAcknowledgmentSent option // output
    //<

    type BillingAmount = Undefined

    //>Workflow4a
    type OrderPlaced = PricedOrder
    type BillableOrderPlaced = {
        OrderId : OrderId
        BillingAddress: Address
        AmountToBill : BillingAmount
        }
    //<

    //>Workflow4b
    type PlaceOrderResult = {
        OrderPlaced : OrderPlaced
        BillableOrderPlaced : BillableOrderPlaced
        OrderAcknowledgmentSent : OrderAcknowledgmentSent option
        }
    //<
    
    //>Workflow4c
    type PlaceOrderEvent = 
        | OrderPlaced of OrderPlaced
        | BillableOrderPlaced of BillableOrderPlaced 
        | AcknowledgmentSent  of OrderAcknowledgmentSent
    //<

    //>Workflow4d
    type CreateEvents = 
        PricedOrder -> PlaceOrderEvent list
    //<

module WithEffects = 

    type ProductCode = DotDotDot

    //>WithEffects1a
    type CheckProductCodeExists = ProductCode -> bool
    //<

    type UnvalidatedAddress = Undefined
    type Address = Undefined
    type AddressValidationError = DotDotDot
    type UnvalidatedOrder = DotDotDot
    type ValidatedOrder = DotDotDot
    type ValidationError = DotDotDot
    type CheckedAddress = DotDotDot

    //>WithEffects1b
    type CheckAddressExists = 
        UnvalidatedAddress -> AsyncResult<CheckedAddress,AddressValidationError>
    //<

    //>WithEffects1c
    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // AsyncResult dependency
          -> UnvalidatedOrder     // input
          -> AsyncResult<ValidatedOrder,ValidationError list>  // output
    //<

    type GetProductPrice = DotDotDot
    type PricedOrder = DotDotDot

    //>WithEffects2a
    type PricingError = PricingError of string

    type PriceOrder = 
        GetProductPrice                       // dependency
          -> ValidatedOrder                   // input
          -> Result<PricedOrder,PricingError> // output
    //<

    type OrderAcknowledgment = DotDotDot
    type SendResult = Sent | NotSent
    type CreateOrderAcknowledgmentLetter = DotDotDot
    type OrderAcknowledgmentSent = DotDotDot

    //>WithEffects3a
    type SendOrderAcknowledgment =
        OrderAcknowledgment -> Async<SendResult>
    //<

    //>WithEffects3b
    type AcknowledgeOrder = 
        CreateOrderAcknowledgmentLetter     // dependency
          -> SendOrderAcknowledgment        // Async dependency
          -> PricedOrder                    // input
          -> Async<OrderAcknowledgmentSent option> // Async output
    //<

module Composition = 
    
    type UnvalidatedOrder = DotDotDot
    type ValidatedOrder = DotDotDot
    type ValidationError = DotDotDot
    type PricedOrder = DotDotDot
    type OrderAcknowledgmentSent = DotDotDot
    type PricingError = DotDotDot
    type PlaceOrderEvent = DotDotDot

    //>Composition1
    type ValidateOrder = 
      UnvalidatedOrder                                       // input
        -> AsyncResult<ValidatedOrder,ValidationError list>  // output

    type PriceOrder = 
      ValidatedOrder                            // input
        -> Result<PricedOrder,PricingError>     // output

    type AcknowledgeOrder = 
      PricedOrder                                // input
        -> Async<OrderAcknowledgmentSent option> // output

    type CreateEvents = 
        PricedOrder               // input
          -> PlaceOrderEvent list // output
    //<

module AreDependenciesPartOfTheDesign1 = 

    type CheckProductCodeExists = Undefined
    type CheckAddressExists = Undefined
    type UnvalidatedOrder = Undefined
    type ValidatedOrder = Undefined
    type ValidationError = Undefined
    type GetProductPrice = Undefined
    type PricedOrder = Undefined
    type PricingError = Undefined

    //>Dependencies1
    type ValidateOrder = 
        CheckProductCodeExists    // explicit dependency
          -> CheckAddressExists   // explicit dependency
          -> UnvalidatedOrder     // input
          -> AsyncResult<ValidatedOrder,ValidationError list>  // output

    type PriceOrder =  
        GetProductPrice                        // explicit dependency
          -> ValidatedOrder                    // input
          -> Result<PricedOrder,PricingError>  // output
    //<

module AreDependenciesPartOfTheDesign2 = 

    type UnvalidatedOrder = Undefined
    type ValidatedOrder = Undefined
    type ValidationError = Undefined
    type PricedOrder = Undefined
    type PricingError = Undefined

    //>Dependencies2
    type ValidateOrder = 
        UnvalidatedOrder                                      // input
          -> AsyncResult<ValidatedOrder,ValidationError list> // output

    type PriceOrder = 
        ValidatedOrder                        // input
          -> Result<PricedOrder,PricingError> // output
    //<

    type PlaceOrder = Undefined
    type PlaceOrderEvent = Undefined
    type PlaceOrderError = Undefined

    //>Dependencies3
    type PlaceOrderWorkflow = 
       PlaceOrder                                             // input 
         -> AsyncResult<PlaceOrderEvent list,PlaceOrderError> // output
    //<


module DomainApi = 

    open System

    //>CompletePipeline_Api1 
    // ----------------------
    // Input data
    // ----------------------

    type UnvalidatedOrder = {
       OrderId : string
       CustomerInfo : UnvalidatedCustomer
       ShippingAddress : UnvalidatedAddress
       }
    and UnvalidatedCustomer = {
       Name : string
       Email : string
       }
    and UnvalidatedAddress = DotDotDot

    // ----------------------
    // Input Command
    // ----------------------

    type Command<'data> = {
       Data : 'data
       Timestamp: DateTime
       UserId: string
       // etc
       }
   
    type PlaceOrderCommand = Command<UnvalidatedOrder>  
    //<


    //>CompletePipeline_Api2
    // ----------------------
    // Public API
    // ----------------------

    /// Success output of PlaceOrder workflow
    type OrderPlaced = DotDotDot
    type BillableOrderPlaced = DotDotDot
    type OrderAcknowledgmentSent = DotDotDot
    type PlaceOrderEvent =
        | OrderPlaced of OrderPlaced
        | BillableOrderPlaced of BillableOrderPlaced 
        | AcknowledgmentSent  of OrderAcknowledgmentSent

    /// Failure output of PlaceOrder workflow
    type PlaceOrderError = DotDotDot
   
    type PlaceOrderWorkflow = 
      PlaceOrderCommand                                      // input command
        -> AsyncResult<PlaceOrderEvent list,PlaceOrderError> // output events
    //<

module PlaceOrderWorkflow = 

    //>CompletePipeline_Int1
    
    // bring in the types from the domain API module
    open DomainApi

    // ----------------------
    // Order life cycle
    // ----------------------
    
    // validated state        
    type ValidatedOrderLine =  DotDotDot
    type ValidatedOrder = {
       OrderId : OrderId
       CustomerInfo : CustomerInfo
       ShippingAddress : Address
       BillingAddress : Address
       OrderLines : ValidatedOrderLine list
       }
    and OrderId = Undefined
    and CustomerInfo = DotDotDot
    and Address = DotDotDot

    // priced state            
    type PricedOrderLine = DotDotDot
    type PricedOrder = DotDotDot

    // all states combined
    type Order =
       | Unvalidated of UnvalidatedOrder
       | Validated of ValidatedOrder
       | Priced of PricedOrder
       // etc
    //<

    type ProductCode = Undefined
    type Price = Undefined

    //>CompletePipeline_Int2
    // ----------------------
    // Definitions of Internal Steps
    // ----------------------

    // ----- Validate order ----- 

    // services used by ValidateOrder
    type CheckProductCodeExists = 
        ProductCode -> bool

    type AddressValidationError = DotDotDot
    type CheckedAddress = DotDotDot
    type CheckAddressExists = 
        UnvalidatedAddress 
          -> AsyncResult<CheckedAddress,AddressValidationError>
    
    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // dependency
          -> UnvalidatedOrder     // input
          -> AsyncResult<ValidatedOrder,ValidationError list>  // output
    and ValidationError = DotDotDot

    // ----- Price order ----- 

    // services used by PriceOrder
    type GetProductPrice = 
        ProductCode -> Price

    type PricingError = DotDotDot

    type PriceOrder = 
        GetProductPrice      // dependency
          -> ValidatedOrder  // input
          -> Result<PricedOrder,PricingError>  // output

    // etc
    //<

