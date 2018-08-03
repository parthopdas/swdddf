//===================================
// Code snippets from chapter 10
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

module Intro1 =
    type UnvalidatedAddress = DotDotDot
    type CheckedAddress = DotDotDot

    //>Intro1
    type CheckAddressExists = 
        UnvalidatedAddress -> CheckedAddress
    //<

module Intro2 =
    type UnvalidatedAddress = DotDotDot
    type CheckedAddress = DotDotDot

    //>Intro2
    type CheckAddressExists = 
        UnvalidatedAddress -> Result<CheckedAddress,AddressValidationError>
    
    and AddressValidationError = 
       | InvalidFormat of string
       | AddressNotFound of string
    //<

module Intro3 =

    let workflowPart1() :int = dotDotDot()

    //>Intro3
    /// A workflow that panics if it gets bad input
    let workflowPart2 input =
        if input = 0 then 
            raise (DivideByZeroException())
        dotDotDot()

    /// Top level function for the application
    /// which traps all exceptions from workflows.
    let main() =

        // wrap all workflows in a try/with block
        try
            let result1 = workflowPart1()
            let result2 = workflowPart2 result1
            printfn "the result is %A" result2
        
        // top level exception handling
        with
        | :? OutOfMemoryException ->
            printfn "exited with OutOfMemoryException"
        | :? DivideByZeroException ->
            printfn "exited with DivideByZeroException"
        | ex ->
            printfn "exited with %s" ex.Message
    //<

module ErrorTypes =
    type RemoteServiceError = DotDotDot
    type ProductCode = DotDotDot

    //>ErrorTypes1 
    type PlaceOrderError =
       | ValidationError of string
       | ProductOutOfStock of ProductCode
       | RemoteServiceError of RemoteServiceError
       //...
    //<

(*>Ugly1
let validateOrder unvalidatedOrder =
   let orderId = ... create order id (or throw exception)
   let customerInfo = ... create info (or throw exception) 
   let shippingAddress = ... create and validate shippingAddress...
   // etc
<*)

(*>Ugly2
let validateOrder unvalidatedOrder =
   let orderIdResult = ... create order id (or return Error)
   if orderIdResult is Error then
       return
   
   let customerInfoResult = ... create name (or return Error)
   if customerInfoResult is Error then
       return
    
   try    
       let shippingAddressResult = ... create valid address (or return Error)
       if shippingAddress is Error then
           return
           
       // ...
       
   with
      | ?: TimeoutException -> Error "service timed out"
      | ?: AuthenticationException -> Error "bad credentials"
       
   // etc
<*)

module Bind =

    do
        //>Bind1
        let bind switchFn  = 
           fun twoTrackInput -> 
               match twoTrackInput with 
               | Ok success -> switchFn success
               | Error failure -> Error failure
        //<
        ()

    //>Bind2
    let bind switchFn twoTrackInput = 
      match twoTrackInput with 
      | Ok success -> switchFn success
      | Error failure -> Error failure
    //<

    //>Map
    let map f aResult = 
       match aResult with 
       | Ok success -> Ok (f success)
       | Error failure -> Error failure
    //<

    //>MapError
    let mapError f aResult = 
       match aResult with 
       | Ok success -> Ok success
       | Error failure -> Error (f failure)
    //<

module OrganizingResult =

    //>OrganizingResult 
    /// Define the Result type
    type Result<'Success,'Failure> = 
       | Ok of 'Success
       | Error of 'Failure
   
    /// Functions that work with Result   
    module Result = 

      let bind f aResult = dotDotDot()
  
      let map f aResult = dotDotDot()
    //<

module Composition =

    type Apple = Apple of string
    type Bananas = Banana of string
    type Cherries = Cherry of string
    type Lemon = Lemon of string

    //>Composition1  
    type FunctionA = Apple -> Result<Bananas,DotDotDot>
    type FunctionB = Bananas -> Result<Cherries,DotDotDot>
    type FunctionC = Cherries -> Result<Lemon,DotDotDot>
    //<

    //>Composition2  
    let functionA : FunctionA = dotDotDot()
    let functionB : FunctionB = dotDotDot()
    let functionC : FunctionC = dotDotDot()

    let functionABC input =
        input
        |> functionA
        |> Result.bind functionB
        |> Result.bind functionC
    //<

module CommonError =

    type Apple = Apple of string
    type Bananas = Banana of string
    type Cherries = Cherry of string
    type AppleError = AppleError of string
    type BananaError = BananaError of string

    //>CommonError1
    type FunctionA = Apple -> Result<Bananas,AppleError>
    type FunctionB = Bananas -> Result<Cherries,BananaError>
    //<

    //>CommonError2
    type FruitError = 
        | AppleErrorCase of AppleError
        | BananaErrorCase of BananaError
    //<

    //>CommonError3
    let functionA : FunctionA = dotDotDot()

    let functionAWithFruitError input = 
        input 
        |> functionA 
        |> Result.mapError (fun appleError -> AppleErrorCase appleError)
    //<

    do
        //>CommonError4
        let functionAWithFruitError input = 
            input 
            |> functionA 
            |> Result.mapError AppleErrorCase 
        //<
        ()

    (*>CommonError5
    // type of functionA
    Apple -> Result<Bananas,AppleError>

    // type of functionAWithFruitError 
    Apple -> Result<Bananas,FruitError>
    <*)

    do
        //>CommonError6
        let functionA : FunctionA = dotDotDot()
        let functionB : FunctionB = dotDotDot()

        // convert functionA to use "FruitError"
        let functionAWithFruitError input = 
            input |> functionA |> Result.mapError AppleErrorCase

        // convert functionB to use "FruitError"    
        let functionBWithFruitError input = 
            input |> functionB |> Result.mapError BananaErrorCase
    
        // and now we can compose the new versions with "bind"    
        let functionAB input = 
            input 
            |> functionAWithFruitError
            |> Result.bind functionBWithFruitError
        //<
        ()

        (*>CommonError7
        val functionAB : Apple -> Result<Cherries,FruitError>
        <*)

module Pipeline =
    type UnvalidatedOrder = DotDotDot
    type ValidatedOrder = DotDotDot
    type ValidationError = DotDotDot

    //>Pipeline1
    type ValidateOrder = 
      // ignoring additional dependencies for now
      UnvalidatedOrder                             // input
        -> Result<ValidatedOrder, ValidationError> // output
    //<

    type PricedOrder = DotDotDot
    type PricingError = DotDotDot
  
    //>Pipeline2
    type PriceOrder = 
      ValidatedOrder                          // input
        -> Result<PricedOrder, PricingError>  // output
    //<

    type CustomerName = DotDotDot
    type AcknowledgmentLetter = DotDotDot
    type OrderAcknowledgmentSent = DotDotDot
    type PlaceOrderEvent = DotDotDot

    //>Pipeline3
    type AcknowledgeOrder = 
      PricedOrder                         // input
        -> OrderAcknowledgmentSent option // output

    type CreateEvents = 
      PricedOrder                            // input
        -> OrderAcknowledgmentSent option    // input (event from previous step)
        -> PlaceOrderEvent list              // output
    //<

    //>Pipeline4
    type PlaceOrderError = 
        | Validation of ValidationError
        | Pricing of PricingError 
    //<


    do
        let priceOrder _ = dotDotDot()
        let validateOrder _ = dotDotDot()

        //>Pipeline5
        // Adapted to return a PlaceOrderError
        let validateOrderAdapted input = 
            input 
            |> validateOrder // the original function
            |> Result.mapError PlaceOrderError.Validation

        // Adapted to return a PlaceOrderError
        let priceOrderAdapted input = 
            input 
            |> priceOrder // the original function
            |> Result.mapError PlaceOrderError.Pricing
        //<

        //>Pipeline6
        let placeOrder unvalidatedOrder = 
            unvalidatedOrder
            |> validateOrderAdapted             // adapted version 
            |> Result.bind validateOrderAdapted // adapted version 
        //<

        do
            let acknowledgeOrder _ = dotDotDot()
            let createEvents _ = dotDotDot()

            //>Pipeline7
            let placeOrder unvalidatedOrder = 
                unvalidatedOrder 
                |> validateOrderAdapted
                |> Result.bind priceOrderAdapted
                |> Result.map acknowledgeOrder   // use map to convert to two-track
                |> Result.map createEvents       // convert to two-track
            //<
            ()
        ()

        (*>Pipeline8
        UnvalidatedOrder -> Result<PlaceOrderEvent list,PlaceOrderError>
        <*)

module Exceptions =

    //>Exceptions1 
    type ServiceInfo = {
       Name : string
       Endpoint: Uri
       }
    //<

    exception AuthorizationException of string

    //>Exceptions2 
    type RemoteServiceError = {
        Service : ServiceInfo 
        Exception : System.Exception
        }
    //<

    //>Exceptions2a 
    /// "Adapter block" that converts exception-throwing services
    /// into Result-returning services.
    let serviceExceptionAdapter serviceInfo serviceFn x =
       try 
          // call the service and return success
          Ok (serviceFn x)
       with
       | :? TimeoutException as ex ->
          Error {Service=serviceInfo; Exception=ex} 
       | :? AuthorizationException as ex ->
          Error {Service=serviceInfo; Exception=ex} 
    //<

    //>Exceptions3 
    let serviceExceptionAdapter2 serviceInfo serviceFn x y =
       try 
          Ok (serviceFn x y)
       with
       | :? TimeoutException as ex -> dotDotDot()
       | :? AuthorizationException as ex -> dotDotDot()
    //<


    //>Exceptions4
    let serviceInfo = {
       Name = "AddressCheckingService"
       Endpoint = dotDotDot()
       }

    // exception-throwing service 
    let checkAddressExists address = 
        dotDotDot()

    // Result-returning service 
    let checkAddressExistsR address = 
        // adapt the service 
        let adaptedService =
           serviceExceptionAdapter serviceInfo checkAddressExists
        // call the service
        adaptedService address
    //<

    (*>Exceptions5
    checkAddressExists : 
        UnvalidatedAddress -> CheckedAddress
    <*)

    (*>Exceptions6
    checkAddressExistsR : 
        UnvalidatedAddress -> Result<CheckedAddress,RemoteServiceError>
    <*)

    type ValidationError = DotDotDot
    type PricingError = DotDotDot

    //>Exceptions7
    type PlaceOrderError = 
        | Validation of ValidationError
        | Pricing of PricingError
        | RemoteService of RemoteServiceError // new!
    //<

    do
        //>Exceptions8
        let checkAddressExistsR address = 
            // adapt the service 
            let adaptedService =
               serviceExceptionAdapter serviceInfo checkAddressExists   
            // call the service
            address
            |> adaptedService 
            |> Result.mapError RemoteService // lift to PlaceOrderError
        //<
        ()

module DeadEnd = 

    //>DeadEnd1
    // string -> unit
    let logError msg =
        printfn "ERROR %s" msg
    //< 
    
    //>DeadEnd2
    // ('a -> unit) -> ('a -> 'a)
    let tee f x = 
        f x 
        x
    //< 

    //>DeadEnd3
    // ('a -> unit) -> (Result<'a,'error> -> Result<'a,'error>)
    let adaptDeadEnd f = 
        Result.map (tee f)
    //<
    
    //>DeadEnd4
    let logErrorR x = 
        (adaptDeadEnd logError) x
    //<

module CE =

    let validateOrderAdapted _ = dotDotDot()
    let priceOrderAdapted _ = dotDotDot()
    let acknowledgeOrder _ = dotDotDot()
    let createEvents _ = dotDotDot()

    //>CE1
    let placeOrder unvalidatedOrder = 
        unvalidatedOrder 
        |> validateOrderAdapted
        |> Result.bind priceOrderAdapted
        |> Result.map acknowledgeOrder   
        |> Result.map createEvents       
    //<

    //>ResultCE
    type ResultBuilder() =
        member this.Return(x) = Ok x
        member this.Bind(x,f) = Result.bind f x

    let result = ResultBuilder()
    //<

    
    type ValidationError = DotDotDot
    type PricingError = DotDotDot
    type RemoteServiceError = DotDotDot
    type PlaceOrderError = 
        | Validation of ValidationError
        | Pricing of PricingError
        | RemoteService of RemoteServiceError // new!
    
    let validateOrder _ = dotDotDot()
    let priceOrder _ = dotDotDot()

    do
        //>CE2
        let placeOrder unvalidatedOrder = 
            result {
                let! validatedOrder = 
                    validateOrder unvalidatedOrder 
                    |> Result.mapError PlaceOrderError.Validation
                let! pricedOrder = 
                    priceOrder validatedOrder 
                    |> Result.mapError PlaceOrderError.Pricing
                let acknowledgmentOption = 
                    acknowledgeOrder pricedOrder 
                let events = 
                    createEvents pricedOrder acknowledgmentOption 
                return events
            }
        //<
        ()

module ComposeCE = 

    //>ComposeCE1
    let validateOrder input = result {
       let! validatedOrder = dotDotDot()
       return validatedOrder
       }
   
    let priceOrder input = result {
       let! pricedOrder = dotDotDot()
       return pricedOrder
       }
    //<

    //>ComposeCE2
    let placeOrder unvalidatedOrder = result {
        let! validatedOrder = validateOrder unvalidatedOrder
        let! pricedOrder = priceOrder validatedOrder
        dotDotDot()
        return dotDotDot()
        }
    //<


module ValidateOrderBefore =

    type CheckProductCodeExists = DotDotDot
    type CheckAddressExists = DotDotDot
    type UnvalidatedOrder = {
        OrderId : DotDotDot
        CustomerInfo : DotDotDot
        ShippingAddress : DotDotDot
        BillingAddress : DotDotDot
        Lines : DotDotDot
        }
    type ValidatedOrder = UnvalidatedOrder 
    module OrderId = 
        let create _ = dotDotDot()
    let toCustomerInfo _ = dotDotDot()
    let toAddress _ = dotDotDot()

    type ValidateOrder = 
      CheckProductCodeExists  // dependency
        -> CheckAddressExists // dependency
        -> UnvalidatedOrder   // input
        -> ValidatedOrder     // output

    //>ValidateOrder_Before
    let validateOrder : ValidateOrder = 
        fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
            let orderId = 
                unvalidatedOrder.OrderId 
                |> OrderId.create
            let customerInfo = 
                unvalidatedOrder.CustomerInfo 
                |> toCustomerInfo
            let shippingAddress = 
                unvalidatedOrder.ShippingAddress 
                |> toAddress checkAddressExists
            let billingAddress = dotDotDot()
            let lines = dotDotDot()

            let validatedOrder : ValidatedOrder = {
                OrderId  = orderId 
                CustomerInfo = customerInfo 
                ShippingAddress = shippingAddress 
                BillingAddress = billingAddress  
                Lines = lines 
            }
            validatedOrder 
    //<

module ValidateOrderAfter =

    type CheckProductCodeExists = DotDotDot
    type CheckAddressExists = DotDotDot
    type UnvalidatedOrder = {
        OrderId : DotDotDot
        CustomerInfo : DotDotDot
        ShippingAddress : DotDotDot
        BillingAddress : DotDotDot
        Lines : DotDotDot
        }
    type ValidatedOrder = UnvalidatedOrder 
    module OrderId = 
        let create _ = dotDotDot()
    let toCustomerInfo _ = dotDotDot()
    let toAddress _ = dotDotDot()

    type ValidationError = ValidationError of string

    type ValidateOrder = 
      CheckProductCodeExists  // dependency
        -> CheckAddressExists // dependency
        -> UnvalidatedOrder   // input
        -> Result<ValidatedOrder,ValidationError>     // output

    //>ValidateOrder_After
    let validateOrder : ValidateOrder = 
        fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
            result {
                let! orderId = 
                    unvalidatedOrder.OrderId 
                    |> OrderId.create
                    |> Result.mapError ValidationError
                let! customerInfo = 
                    unvalidatedOrder.CustomerInfo 
                    |> toCustomerInfo
                let! shippingAddress = dotDotDot()
                let! billingAddress  = dotDotDot()
                let! lines = dotDotDot()

                let validatedOrder : ValidatedOrder = {
                    OrderId  = orderId 
                    CustomerInfo = customerInfo 
                    ShippingAddress = shippingAddress 
                    BillingAddress = billingAddress  
                    Lines = lines 
                }
                return validatedOrder 
            }
    //<
    
module Lists =

    type ValidatedOrder = {
        Lines : string list
    }

    let checkProductCodeExists _ = dotDotDot()
    let toValidatedOrderLine _ = dotDotDot()

    //>Lists1
    let validateOrder unvalidatedOrder =
        dotDotDot()

        // convert each line into an OrderLine domain type
        let lines = 
            unvalidatedOrder.Lines 
            |> List.map (toValidatedOrderLine checkProductCodeExists) 

        // create and return a ValidatedOrder
        let validatedOrder : ValidatedOrder = {
           //...
           Lines = lines
           // etc
           }
        validatedOrder 
    //<

    let createValidatedOrder _ _ _ _ = dotDotDot()

module Lists2 =
    open Lists

    //>Lists2
    let validateOrder unvalidatedOrder =
        dotDotDot()
        
        let lines = // lines is a "list of Result"   
            unvalidatedOrder.Lines 
            |> List.map (toValidatedOrderLine checkProductCodeExists) 
   
        let validatedOrder : ValidatedOrder = {
           //...
           Lines = lines  // compiler error 
           //       ^ expecting a "Result of list" here
           }
        dotDotDot()
    //<

    module Result = 

        //>prepend
        /// Prepend a Result<item> to a Result<list>
        let prepend firstR restR = 
            match firstR, restR with
            | Ok first, Ok rest -> Ok (first::rest)
            | Error err1, Ok _ -> Error err1
            | Ok _, Error err2 -> Error err2
            | Error err1, Error _ -> Error err1 
        //<

        //>sequence 
        let sequence aListOfResults = 
            let initialValue = Ok [] // empty list inside Result
 
            // loop through the list in reverse order,
            // prepending each element to the initial value
            List.foldBack prepend aListOfResults initialValue
        //<

    //>ListsSuccess
    type IntOrError = Result<int,string>
    
    let listOfSuccesses : IntOrError list = [Ok 1; Ok 2]
    let successResult = 
        Result.sequence listOfSuccesses   // Ok [1; 2]
    //<

    //>ListsError
    let listOfErrors : IntOrError list = [ Error "bad"; Error "terrible" ]

    let errorResult = 
        Result.sequence listOfErrors  // Error "bad"
    //<

module ValidateOrderLines =
    open Lists

    type CheckProductCodeExists = DotDotDot
    type CheckAddressExists = DotDotDot
    type UnvalidatedOrder = {
        OrderId : DotDotDot
        CustomerInfo : DotDotDot
        ShippingAddress : DotDotDot
        BillingAddress : DotDotDot
        Lines : DotDotDot list
        }
    type ValidatedOrder = UnvalidatedOrder 
    module OrderId = 
        let create _ = dotDotDot()
    let toCustomerInfo _ = dotDotDot()
    let toAddress _ = dotDotDot()

    type ValidationError = ValidationError of string

    type ValidateOrder = 
      CheckProductCodeExists  // dependency
        -> CheckAddressExists // dependency
        -> UnvalidatedOrder   // input
        -> Result<ValidatedOrder,ValidationError>     // output

    let checkProductCodeExists _ = dotDotDot()
    let toValidatedOrderLine _ _ = dotDotDot()

    //>ValidateOrder_OrderLines
    let validateOrder : ValidateOrder = 
        fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
            result {
                let! orderId = dotDotDot()
                let! customerInfo = dotDotDot()
                let! shippingAddress = dotDotDot()
                let! billingAddress  = dotDotDot()
                let! lines = 
                    unvalidatedOrder.Lines 
                    |> List.map (toValidatedOrderLine checkProductCodeExists) 
                    |> Result.sequence // convert list of Results to a single Result

                let validatedOrder : ValidatedOrder = {
                    OrderId  = orderId 
                    CustomerInfo = customerInfo 
                    ShippingAddress = shippingAddress 
                    BillingAddress = billingAddress  
                    Lines = lines 
                }
                return validatedOrder 
            }
    //<

module ValidateOrderLines2 =
    open ValidateOrderLines 

    type PlaceOrderEvent = DotDotDot
    type PlaceOrderError =
        | Validation of ValidationError 
        | Pricing of DotDotDot

    type PlaceOrder = 
        UnvalidatedOrder -> Result<PlaceOrderEvent list,PlaceOrderError>

    let checkProductExists _  = dotDotDot()
    let checkAddressExists _ = dotDotDot()
    let getProductPrice _ = dotDotDot()
    
    let validateOrder _ _ _ = dotDotDot()
    let priceOrder _ _ = dotDotDot()

    //>placeOrder 
    let placeOrder : PlaceOrder =       // definition of function
        fun unvalidatedOrder -> 
            result {
                let! validatedOrder = 
                    validateOrder checkProductExists checkAddressExists unvalidatedOrder 
                    |> Result.mapError PlaceOrderError.Validation
                let! pricedOrder = 
                    priceOrder getProductPrice validatedOrder 
                    |> Result.mapError PlaceOrderError.Pricing
                let acknowledgmentOption = dotDotDot()
                let events = dotDotDot()
                return events
            }
    //<


module ValidateOrderAsync =

    type CheckProductCodeExists = DotDotDot
    type UnvalidatedOrder = {
        OrderId : DotDotDot
        CustomerInfo : DotDotDot
        ShippingAddress : DotDotDot
        BillingAddress : DotDotDot
        Lines : DotDotDot list
        }
    type ValidatedOrder = UnvalidatedOrder 
    module OrderId = 
        let create _ = dotDotDot()
    let toCustomerInfo _ = dotDotDot()
    let toAddress _ = dotDotDot()
    let toValidatedOrderLine _ = dotDotDot()

    type ValidationError = ValidationError of string
    type UnvalidatedAddress = DotDotDot
    type CheckedAddress = DotDotDot
    type AddressValidationError = 
        | InvalidFormat 
        | AddressNotFound 

    //>CheckAddressExists 
    type CheckAddressExists =
        UnvalidatedAddress -> AsyncResult<CheckedAddress,AddressValidationError>
    //<

    //>toCheckedAddressAsync
    /// Call the checkAddressExists and convert the error to a ValidationError
    let toCheckedAddress (checkAddress:CheckAddressExists) address =
        address 
        |> checkAddress 
        |> AsyncResult.mapError (fun addrError -> 
            match addrError with
            | AddressNotFound -> ValidationError "Address not found"
            | InvalidFormat -> ValidationError "Address has bad format"
            )
    //<

    type ValidateOrder = 
      CheckProductCodeExists  // dependency
        -> CheckAddressExists // dependency
        -> UnvalidatedOrder   // input
        -> AsyncResult<ValidatedOrder,ValidationError>     // output


    //>ValidateOrderAsync
    let validateOrder : ValidateOrder = 
        fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
            asyncResult {
                let! orderId = 
                    unvalidatedOrder.OrderId 
                    |> OrderId.create
                    |> Result.mapError ValidationError
                    |> AsyncResult.ofResult   // lift a Result to AsyncResult
                let! customerInfo = 
                    unvalidatedOrder.CustomerInfo 
                    |> toCustomerInfo
                    |> AsyncResult.ofResult
                let! checkedShippingAddress = // extract the checked address 
                    unvalidatedOrder.ShippingAddress 
                    |> toCheckedAddress checkAddressExists
                let! shippingAddress =        // process checked address  
                    checkedShippingAddress 
                    |> toAddress 
                    |> AsyncResult.ofResult
                let! billingAddress = dotDotDot()
                let! lines = 
                    unvalidatedOrder.Lines 
                    |> List.map (toValidatedOrderLine checkProductCodeExists) 
                    |> Result.sequence // convert list of Results to a single Result
                    |> AsyncResult.ofResult
                let validatedOrder : ValidatedOrder = {
                    OrderId  = orderId 
                    CustomerInfo = customerInfo 
                    ShippingAddress = shippingAddress 
                    BillingAddress = billingAddress  
                    Lines = lines 
                }
                return validatedOrder 
            }
    //<
    
module ValidateOrderAsync2 =
    open ValidateOrderAsync

    type PlaceOrderEvent = DotDotDot
    type PlaceOrderError =
        | Validation of ValidationError 
        | Pricing of DotDotDot

    type PlaceOrder = 
        UnvalidatedOrder -> AsyncResult<PlaceOrderEvent list,PlaceOrderError>

    let checkProductExists _  = dotDotDot()
    let checkAddressExists _ = dotDotDot()
    let getProductPrice _ = dotDotDot()
    
    let validateOrder _ _ _ = dotDotDot()
    let priceOrder _ _ = dotDotDot()


    //>PlaceOrderAsync
    let placeOrder : PlaceOrder =       
        fun unvalidatedOrder -> 
            asyncResult {
                let! validatedOrder = 
                    validateOrder checkProductExists checkAddressExists unvalidatedOrder 
                    |> AsyncResult.mapError PlaceOrderError.Validation
                let! pricedOrder = 
                    priceOrder getProductPrice validatedOrder 
                    |> AsyncResult.ofResult
                    |> AsyncResult.mapError PlaceOrderError.Pricing
                let acknowledgmentOption = dotDotDot()
                let events = dotDotDot() 
                return events
            }
    //<













