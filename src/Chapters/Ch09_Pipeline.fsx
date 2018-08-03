
//===================================
// Code snippets from chapter 9
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

#load "Common.fsx"
open Common
open System


module Intro =

    (*
    "Here's an example of how we want our code to look, using the piping approach"
    *)
    
    let validateOrder _ = dotDotDot
    let priceOrder _ = dotDotDot
    let acknowledgeOrder _ = dotDotDot
    let createEvents _ = dotDotDot

    //>IntroPlaceOrder 
    let placeOrder unvalidatedOrder =
       unvalidatedOrder
       |> validateOrder
       |> priceOrder
       |> acknowledgeOrder
       |> createEvents
    //<


module SimpleTypesImplementation =

    open System 
    
    (*
    "Let's convert this into an implementation."
    *)

    //>SimpleTypes1
    module Domain = 
        type OrderId = private OrderId of string

        module OrderId =
            /// Define a "Smart constructor" for OrderId 
            /// string -> OrderId 
            let create str = 
                if String.IsNullOrEmpty(str) then
                    // use exceptions rather than Result for now
                    failwith "OrderId must not be null or empty" 
                elif str.Length > 50 then
                    failwith "OrderId must not be more than 50 chars" 
                else
                    OrderId str

            /// Extract the inner value from an OrderId
            /// OrderId -> string
            let value (OrderId str) = // unwrap in the parameter!
              str                     // return the inner value
    //<

    // F# VERSION DIFFERENCE
    // Modules with the same name as a non-generic type will cause an error in versions of F# before v4.1 (VS2017) 
    // so change the module definition to include a [<CompilationRepresentation>] attribute like this:
    (*>SimpleTypes2
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module OrderId =
        dotDotDot()
    <*)

module UsingFunctionSignatures = 
    (*
    "The simplest is to continue as we have been, adding any missing parameters as"
    *)

    do
        //>UsingFunctionSignatures1
        let validateOrder 
          checkProductCodeExists // dependency
          checkAddressExists     // dependency
          unvalidatedOrder =     // input
            dotDotDot
        //<
        ()

    type Param1 = DotDotDot
    type Param2 = DotDotDot
    type Result = DotDotDot

    (*
    "if we want to make it clear that we are implementing a specific function signature"
    *)
    //>UsingFunctionSignatures2
    // define a function signature
    type MyFunctionSignature = Param1 -> Param2 -> Result

    // define a function that implements that signature
    let myFunc: MyFunctionSignature = 
        fun param1 param2 -> 
            dotDotDot()
    //<

    
    type UnvalidatedOrder = Undefined
    type ValidatedOrder = Undefined
    type ValidationError = Undefined
    type ProductCode = ProductCode of string
    type CheckProductCodeExists = ProductCode -> bool
    type CheckAddressExists = Undefined
    
    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // dependency
          -> UnvalidatedOrder     // input
          -> Result<ValidatedOrder,ValidationError list>  // outpu

    (*
    "Applying this approach to the `validateOrder` function gives us:"
    *)
    //>UsingFunctionSignatures3
    let validateOrder : ValidateOrder = 
      fun checkProductCodeExists checkAddressExists unvalidatedOrder -> 
        // ^dependency           ^dependency        ^input   
            dotDotDot()
    //<

    do
        //>UsingFunctionSignatures4
        //let validateOrder : ValidateOrder = 
        //   fun checkProductCodeExists checkAddressExists unvalidatedOrder -> 
        //      if checkProductCodeExists 42 then
        //         //       compiler error ^ 
        //         // This expression was expected to have type ProductCode
        //         // but here has type int    
        //         dotDotDot()
        //      dotDotDot()
        //<
        ()


// ============================================================================
// COMMON TYPES FOR IMPLEMENTATION
// ============================================================================


module SimpleTypes =

    /// Constrained to be 50 chars or less, not null
    type String50 = private String50 of string

    /// An email address
    type EmailAddress = private EmailAddress of string

    /// A zip code
    type ZipCode = private ZipCode of string

    /// An Id for Orders. Constrained to be a non-empty string < 10 chars
    type OrderId = private OrderId of string

    /// An Id for OrderLines. Constrained to be a non-empty string < 10 chars
    type OrderLineId = private OrderLineId of string

    /// The codes for Widgets start with a "W" and then four digits
    type WidgetCode = private WidgetCode of string  

    /// The codes for Gizmos start with a "G" and then three digits. 
    type GizmoCode = private GizmoCode of string  

    /// A ProductCode is either a Widget or a Gizmo
    type ProductCode =
        | Widget of WidgetCode
        | Gizmo of GizmoCode

    /// Constrained to be a integer between 1 and 1000
    type UnitQuantity = private UnitQuantity of int 

    /// Constrained to be a decimal between 0.05 and 100.00 
    type KilogramQuantity = private KilogramQuantity of decimal

    /// A Quantity is either a Unit or a Kilogram
    type OrderQuantity =
        | Unit of UnitQuantity
        | Kilogram of KilogramQuantity

    /// Constrained to be a decimal between 0.0 and 1000.00 
    type Price = private Price of decimal

    /// Constrained to be a decimal between 0.0 and 10000.00 
    type BillingAmount = private BillingAmount of decimal

    /// Represents a PDF attachment
    type PdfAttachment = {
        Name : string
        Bytes: byte[]
        }


    // ===============================
    // Reusable constructors and getters for constrained types
    // ===============================

    /// Useful functions for constrained types
    module ConstrainedType =

        /// Create a constrained string using the constructor provided
        /// Return Error if input is null, empty, or length > maxLen
        let createString name ctor maxLen str = 
            if String.IsNullOrEmpty(str) then
                let msg = sprintf "%s must not be null or empty" name 
                failwith msg
            elif str.Length > maxLen then
                let msg = sprintf "%s must not be more than %i chars" name maxLen 
                failwith msg 
            else
                ctor str

        /// Create a optional constrained string using the constructor provided
        /// Return None if input is null, empty. 
        /// Return error if length > maxLen
        /// Return Some if the input is valid
        let createStringOption name ctor maxLen str = 
            if String.IsNullOrEmpty(str) then
                None
            elif str.Length > maxLen then
                let msg = sprintf "%s must not be more than %i chars" name maxLen 
                failwith msg 
            else
                ctor str |> Some

        /// Create a constrained integer using the constructor provided
        /// Return Error if input is less than minVal or more than maxVal
        let createInt name ctor minVal maxVal i = 
            if i < minVal then
                let msg = sprintf "%s: Must not be less than %i" name minVal
                failwith msg
            elif i > maxVal then
                let msg = sprintf "%s: Must not be greater than %i" name maxVal
                failwith msg
            else
                ctor i

        /// Create a constrained decimal using the constructor provided
        /// Return Error if input is less than minVal or more than maxVal
        let createDecimal name ctor minVal maxVal i = 
            if i < minVal then
                let msg = sprintf "%s: Must not be less than %M" name minVal
                failwith msg
            elif i > maxVal then
                let msg = sprintf "%s: Must not be greater than %M" name maxVal
                failwith msg
            else
                ctor i

        /// Create a constrained string using the constructor provided
        /// Return Error if input is null. empty, or does not match the regex pattern
        let createLike name ctor pattern str = 
            if String.IsNullOrEmpty(str) then
                let msg = sprintf "%s: Must not be null or empty" name 
                failwith msg
            elif System.Text.RegularExpressions.Regex.IsMatch(str,pattern) then
                ctor str
            else
                let msg = sprintf "%s: '%s' must match the pattern '%s'" name str pattern
                failwith msg 


    // F# VERSION DIFFERENCE
    // Modules with the same name as a non-generic type will cause an error in versions of F# before v4.1 (VS2017) 
    // so change the module definition to include a [<CompilationRepresentation>] attribute like this:
    (*
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    module String50 =
    *)

    module String50 =

        /// Return the value inside a String50
        let value (String50 str) = str

        /// Create an String50 from a string
        /// Return Error if input is null, empty, or length > 50
        let create str = 
            ConstrainedType.createString "String50" String50 50 str

        /// Create an String50 from a string
        /// Return None if input is null, empty. 
        /// Return error if length > maxLen
        /// Return Some if the input is valid
        let createOption str = 
            ConstrainedType.createStringOption "String50" String50 50 str

    module EmailAddress =

        /// Return the string value inside an EmailAddress 
        let value (EmailAddress str) = str

        /// Create an EmailAddress from a string
        /// Return Error if input is null, empty, or doesn't have an "@" in it
        let create str = 
            let pattern = ".+@.+" // anything separated by an "@"
            ConstrainedType.createLike "EmailAddress" EmailAddress pattern str

    module ZipCode =

        /// Return the string value inside a ZipCode
        let value (ZipCode str) = str

        /// Create a ZipCode from a string
        /// Return Error if input is null, empty, or doesn't have 5 digits
        let create str = 
            let pattern = "\d{5}"
            ConstrainedType.createLike "ZipCode" ZipCode pattern str


    module OrderId =

        /// Return the string value inside an OrderId
        let value (OrderId str) = str

        /// Create an OrderId from a string
        /// Return Error if input is null, empty, or length > 50
        let create str = 
            ConstrainedType.createString "OrderId" OrderId 50 str

    module OrderLineId =

        /// Return the string value inside an OrderLineId 
        let value (OrderLineId str) = str

        /// Create an OrderLineId from a string
        /// Return Error if input is null, empty, or length > 50
        let create str = 
            ConstrainedType.createString "OrderLineId" OrderLineId 50 str

    module WidgetCode =

        /// Return the string value inside a WidgetCode 
        let value (WidgetCode code) = code

        /// Create an WidgetCode from a string
        /// Return Error if input is null. empty, or not matching pattern
        let create code = 
            // The codes for Widgets start with a "W" and then four digits
            let pattern = "W\d{4}"
            ConstrainedType.createLike "WidgetCode" WidgetCode pattern code 

    module GizmoCode =

        /// Return the string value inside a GizmoCode
        let value (GizmoCode code) = code

        /// Create an GizmoCode from a string
        /// Return Error if input is null, empty, or not matching pattern
        let create code = 
            // The codes for Gizmos start with a "G" and then three digits. 
            let pattern = "G\d{3}"
            ConstrainedType.createLike "GizmoCode" GizmoCode pattern code 

    module ProductCode =

        /// Return the string value inside a ProductCode 
        let value productCode = 
            match productCode with
            | Widget (WidgetCode wc) -> wc
            | Gizmo (GizmoCode gc) -> gc

        /// Create an ProductCode from a string
        /// Return Error if input is null, empty, or not matching pattern
        let create code = 
            if String.IsNullOrEmpty(code) then
                let msg = sprintf "ProductCode: Must not be null or empty" 
                failwith msg
            else if code.StartsWith("W") then
                WidgetCode.create code 
                |> Widget
            else if code.StartsWith("G") then
                GizmoCode.create code
                |> Gizmo
            else 
                let msg = sprintf "ProductCode: Format not recognized '%s'" code
                failwith msg


    module UnitQuantity  =

        /// Return the value inside a UnitQuantity 
        let value (UnitQuantity v) = v

        /// Create a UnitQuantity from a int
        /// Return Error if input is not an integer between 1 and 1000 
        let create v = 
            ConstrainedType.createInt "UnitQuantity" UnitQuantity 1 1000 v

    module KilogramQuantity =

        /// Return the value inside a KilogramQuantity 
        let value (KilogramQuantity v) = v

        /// Create a KilogramQuantity from a decimal.
        /// Return Error if input is not a decimal between 0.05 and 100.00 
        let create v = 
            ConstrainedType.createDecimal "KilogramQuantity" KilogramQuantity 0.5M 100M v

    module OrderQuantity  =

        /// Return the value inside a OrderQuantity  
        let value qty = 
            match qty with
            | Unit uq -> 
                uq |> UnitQuantity.value |> decimal
            | Kilogram kq -> 
                kq |> KilogramQuantity.value 

        /// Create a UnitQuantity from a int
        /// Return Error if input is not an integer between 1 and 1000 
        let create v = 
            ConstrainedType.createInt "UnitQuantity" UnitQuantity 1 1000 v

    module Price =

        /// Return the value inside a Price 
        let value (Price v) = v

        /// Create a Price from a decimal.
        /// Return Error if input is not a decimal between 0.0 and 1000.00 
        let create v = 
            ConstrainedType.createDecimal "Price" Price 0.0M 1000M v

        //>Price_multiply
        /// Multiply a Price by a decimal qty.
        /// Raise exception if new price is out of bounds.
        let multiply qty (Price p) = 
            create (qty * p)
        //<

    module BillingAmount =

        /// Return the value inside a BillingAmount
        let value (BillingAmount v) = v

        /// Create a BillingAmount from a decimal.
        /// Return Error if input is not a decimal between 0.0 and 10000.00 
        let create v = 
            ConstrainedType.createDecimal "BillingAmount" BillingAmount 0.0M 10000M v

        //>BillingAmount_sumPrices  
        /// Sum a list of prices to make a billing amount
        /// Raise exception if total is out of bounds
        let sumPrices prices =
            let total = prices |> List.map Price.value |> List.sum
            create total 
        //<

module CommonTypes =
    open SimpleTypes

    type PersonalName = {
        FirstName : String50
        LastName : String50
        }
    
    type CustomerInfo = {
        Name : PersonalName 
        EmailAddress : EmailAddress 
        }

    type Address = {
        AddressLine1 : String50
        AddressLine2 : String50 option
        AddressLine3 : String50 option
        AddressLine4 : String50 option
        City : String50
        ZipCode : ZipCode
        }



    type UnvalidatedCustomerInfo = {
        FirstName : string
        LastName : string
        EmailAddress : string
        }

    type UnvalidatedAddress = {
        AddressLine1 : string
        AddressLine2 : string
        AddressLine3 : string
        AddressLine4 : string
        City : string
        ZipCode : string
        }

    type UnvalidatedOrderLine =  {
        OrderLineId : string
        ProductCode : string
        Quantity : decimal
        }

    type UnvalidatedOrder = {
        OrderId : string
        CustomerInfo : UnvalidatedCustomerInfo
        ShippingAddress : UnvalidatedAddress
        BillingAddress : UnvalidatedAddress
        Lines : UnvalidatedOrderLine list
        }

    type ValidatedOrderLine =  {
        OrderLineId : OrderLineId 
        ProductCode : ProductCode 
        Quantity : OrderQuantity
        }

    type ValidatedOrder = {
        OrderId : OrderId
        CustomerInfo : CustomerInfo
        ShippingAddress : Address
        BillingAddress : Address
        Lines : ValidatedOrderLine list
        }

    // priced state            
    type PricedOrderLine = {
        OrderLineId : OrderLineId 
        ProductCode : ProductCode 
        Quantity : OrderQuantity
        LinePrice : Price
        }

    type PricedOrder = {
        OrderId : OrderId
        CustomerInfo : CustomerInfo
        ShippingAddress : Address
        BillingAddress : Address
        AmountToBill : BillingAmount
        Lines : PricedOrderLine list
        }

    type HtmlString = 
        HtmlString of string

    type OrderAcknowledgment = {
        EmailAddress : EmailAddress
        Letter : HtmlString 
        }


    /// Event will be created if the Acknowledgment was successfully posted
    type OrderAcknowledgmentSent = {
        OrderId : OrderId
        EmailAddress : EmailAddress 
        }

    /// Event to send to billing context
    /// Will only be created if the AmountToBill is not zero
    type BillableOrderPlaced = {
        OrderId : OrderId
        BillingAddress: Address
        AmountToBill : BillingAmount
        }

open CommonTypes


// ============================================================================
// IMPLEMENTATION
// ============================================================================


module ValidationDesignFromPreviousChapter =
    (*
    The design from "Modeling Workflows" chapter was this:
    *)
    open CommonTypes

    type CheckProductCodeExists = DotDotDot
    type CheckedAddress = DotDotDot
    type AddressValidationError = DotDotDot
    type ValidationError = DotDotDot

    //>DesignFromPreviousChapter
    type CheckAddressExists = 
        UnvalidatedAddress -> AsyncResult<CheckedAddress,AddressValidationError>

    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // AsyncResult dependency
          -> UnvalidatedOrder     // input
          -> AsyncResult<ValidatedOrder,ValidationError list>  // output
    //<

module ValidationDesignWithoutEffects =
    (*
    If we eliminate the effects, we can remove the `AsyncResult` 
    and the `...Error` types, leaving us with this:
    *)

    open CommonTypes
    type CheckProductCodeExists = DotDotDot
    type CheckedAddress = DotDotDot

    //>DesignWithoutEffects
    type CheckAddressExists = 
        UnvalidatedAddress -> CheckedAddress

    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // dependency
          -> UnvalidatedOrder     // input
          -> ValidatedOrder       // output
    //<

module ValidationImplementation1 =
    open SimpleTypes
    open CommonTypes

    (*
    A first attempt at the implementation will therefore look something like this:
    *)

    type CheckProductCodeExists = ProductCode -> bool
    type CheckedAddress = CheckedAddress of UnvalidatedAddress
    type CheckAddressExists = 
        UnvalidatedAddress -> CheckedAddress

    type ValidateOrder = 
        CheckProductCodeExists    // dependency
          -> CheckAddressExists   // dependency
          -> UnvalidatedOrder     // input
          -> ValidatedOrder       // output

    let toCustomerInfo _ = dotDotDot()
    let toAddress _ = dotDotDot()

    //>Validation1_validateOrder 
    let validateOrder : ValidateOrder = 
      fun checkProductCodeExists checkAddressExists unvalidatedOrder -> 

       let orderId = 
           unvalidatedOrder.OrderId 
           |> OrderId.create 
   
       let customerInfo = 
           unvalidatedOrder.CustomerInfo 
           |> toCustomerInfo   // helper function
   
       let shippingAddress = 
           unvalidatedOrder.ShippingAddress 
           |> toAddress        // helper function
  
       // and so on, for each property of the unvalidatedOrder
   
       // when all the fields are ready, use them to 
       // create and return a new "ValidatedOrder" record
       {
           OrderId = orderId
           CustomerInfo = customerInfo
           ShippingAddress = shippingAddress
           BillingAddress = dotDotDot()
           Lines = dotDotDot()
       }
    //<

module ValidationImplementation2 =
    open SimpleTypes
    open CommonTypes
    open ValidationImplementation1 


    //>Validation2_toCustomerInfo
    let toCustomerInfo (customer:UnvalidatedCustomerInfo) : CustomerInfo = 
        // create the various CustomerInfo properties
        // and throw exceptions if invalid
        let firstName = customer.FirstName |> String50.create 
        let lastName = customer.LastName |> String50.create 
        let emailAddress = customer.EmailAddress |> EmailAddress.create 
           
        // create a PersonalName 
        let name : PersonalName = {
            FirstName = firstName 
            LastName = lastName
            }

        // create a CustomerInfo
        let customerInfo : CustomerInfo = {
            Name = name 
            EmailAddress = emailAddress
            }
        // ... and return it
        customerInfo    
    //<

    //>Validation2_toAddress
    let toAddress (checkAddressExists:CheckAddressExists) unvalidatedAddress =
        // call the remote service
        let checkedAddress = checkAddressExists unvalidatedAddress 
        // extract the inner value using pattern matching
        let (CheckedAddress checkedAddress) = checkedAddress 

        let addressLine1 = 
            checkedAddress.AddressLine1 |> String50.create
        let addressLine2 = 
            checkedAddress.AddressLine2 |> String50.createOption
        let addressLine3 = 
            checkedAddress.AddressLine3 |> String50.createOption
        let addressLine4 = 
            checkedAddress.AddressLine4 |> String50.createOption
        let city = 
            checkedAddress.City |> String50.create
        let zipCode = 
            checkedAddress.ZipCode |> ZipCode.create
        // create the address
        let address : Address = {
            AddressLine1 = addressLine1
            AddressLine2 = addressLine2
            AddressLine3 = addressLine3
            AddressLine4 = addressLine4
            City = city
            ZipCode = zipCode
            }
        // return the address
        address
    //<

    //>Validation2_validateOrder 
    let validateOrder : ValidateOrder = 
      fun checkProductCodeExists checkAddressExists unvalidatedOrder -> 

       let orderId = dotDotDot()
       let customerInfo = dotDotDot()
       let shippingAddress = 
           unvalidatedOrder.ShippingAddress 
           |> toAddress checkAddressExists // new parameter
  
       dotDotDot()
    //<

module ValidationImplementation3 =
    open SimpleTypes
    open CommonTypes
    open ValidationImplementation1 
    open ValidationImplementation2 

    module ToProductCode = 
        //>Validation3_toProductCodeBool 
        let toProductCode (checkProductCodeExists:CheckProductCodeExists) productCode = 
            productCode
            |> ProductCode.create
            |> checkProductCodeExists 
            // returns a bool :(
        //<

    /// Function adapter to convert a predicate to a passthru 
    let predicateToPassthru errorMsg f x =
        if f x then
            x
        else
            failwith errorMsg

    //>Validation3_toProductCode 
    let toProductCode (checkProductCodeExists:CheckProductCodeExists) productCode = 

        // create a local ProductCode -> ProductCode function 
        // suitable for using in a pipeline
        let checkProduct productCode = 
            let errorMsg = sprintf "Invalid: %A" productCode 
            predicateToPassthru errorMsg checkProductCodeExists productCode
        
        // assemble the pipeline        
        productCode
        |> ProductCode.create
        |> checkProduct 
    //<

    //>Validation3_toOrderQuantity 
    let toOrderQuantity productCode quantity = 
        match productCode with
        | Widget _ -> 
            quantity
            |> int                  // convert decimal to int 
            |> UnitQuantity.create  // to UnitQuantity
            |> OrderQuantity.Unit   // lift to OrderQuantity type
        | Gizmo _ -> 
            quantity 
            |> KilogramQuantity.create  // to KilogramQuantity
            |> OrderQuantity.Kilogram   // lift to OrderQuantity type
    //<

    //>Validation3_toOrderLine 
    let toValidatedOrderLine checkProductCodeExists (unvalidatedOrderLine:UnvalidatedOrderLine) = 
        let orderLineId = 
            unvalidatedOrderLine.OrderLineId 
            |> OrderLineId.create 
        let productCode = 
            unvalidatedOrderLine.ProductCode 
            |> toProductCode checkProductCodeExists // helper function
        let quantity = 
            unvalidatedOrderLine.Quantity 
            |> toOrderQuantity productCode  // helper function
        let validatedOrderLine = {
            OrderLineId = orderLineId 
            ProductCode = productCode 
            Quantity = quantity 
            }
        validatedOrderLine 

    //<

    //>Validation3_validateOrder 
    let validateOrder : ValidateOrder = 
      fun checkProductCodeExists checkAddressExists unvalidatedOrder -> 

        let orderId = dotDotDot()
        let customerInfo = dotDotDot()
        let shippingAddress = dotDotDot()
  
        let orderLines = 
            unvalidatedOrder.Lines
            // convert each line using `toValidatedOrderLine`
            |> List.map (toValidatedOrderLine checkProductCodeExists)
        dotDotDot()
    //<

module FunctionAdapters =

    //>FunctionAdapters1 
    let convertToPassthru checkProductCodeExists productCode =
        if checkProductCodeExists productCode then  
            productCode
        else  
            failwith "Invalid Product Code"
    //<    

    (*>FunctionAdapters2 
    val convertToPassthru :
       checkProductCodeExists:('a -> bool) -> productCode:'a -> 'a
    <*)



    do
        //>FunctionAdapters3 
        let predicateToPassthru f x =
            if f x then  
                x
            else  
                failwith "Invalid Product Code"
        //<    
        ()

    //>FunctionAdapters4
    let predicateToPassthru errorMsg f x =
        if f x then  
            x
        else  
            failwith errorMsg 
    //<    

    (*>FunctionAdapters5
    val predicateToPassthru : errorMsg:string -> f:('a -> bool) -> x:'a -> 'a
    <*)



module Pricing =
    open SimpleTypes
    open CommonTypes

    (*
    "We can implement the other steps in the same way. Here's the pricing step design:"
    *)


    //>Pricing_DesignNoEffects
    type GetProductPrice = ProductCode -> Price
    type PriceOrder = 
      GetProductPrice      // dependency
        -> ValidatedOrder  // input
        -> PricedOrder     // output
    //<

    module WithEffects = 
        type PlaceOrderError = DotDotDot
        //>Pricing_DesignWithEffects
        type PriceOrder = 
          GetProductPrice      // dependency
            -> ValidatedOrder  // input
            -> Result<PricedOrder, PlaceOrderError>  // output
        //<
    

    //>Pricing_toPricedOrderLine 
    /// Transform a ValidatedOrderLine to a PricedOrderLine
    let toPricedOrderLine getProductPrice (line:ValidatedOrderLine) : PricedOrderLine =
        let qty = line.Quantity |> OrderQuantity.value 
        let price = line.ProductCode |> getProductPrice 
        let linePrice = price |> Price.multiply qty 
        {
            OrderLineId = line.OrderLineId 
            ProductCode = line.ProductCode
            Quantity = line.Quantity
            LinePrice = linePrice 
        }
    //<

    //>Pricing_priceOrder
    let priceOrder : PriceOrder = 
        fun getProductPrice validatedOrder -> 
            let lines = 
                validatedOrder.Lines 
                |> List.map (toPricedOrderLine getProductPrice) 
            let amountToBill = 
                lines 
                // get each line price
                |> List.map (fun line -> line.LinePrice)  
                // add them together as a BillingAmount
                |> BillingAmount.sumPrices  
            let pricedOrder : PricedOrder = {
                OrderId  = validatedOrder.OrderId 
                CustomerInfo = validatedOrder.CustomerInfo 
                ShippingAddress = validatedOrder.ShippingAddress 
                BillingAddress = validatedOrder.BillingAddress  
                Lines = lines 
                AmountToBill = amountToBill 
                }
            pricedOrder 
    //<

    do
        //>RestOfPipeline_notImplemented
        let priceOrder : PriceOrder = 
           fun getProductPrice validatedOrder -> 
              failwith "not implemented"
        //<
        ()

module AcknowledgeOrder =
    open SimpleTypes
    open CommonTypes

    //>AcknowledgeOrder_Design
    type HtmlString = HtmlString of string
    type CreateOrderAcknowledgmentLetter =
        PricedOrder -> HtmlString

    type OrderAcknowledgment = {
        EmailAddress : EmailAddress
        Letter : HtmlString 
        }
    type SendResult = Sent | NotSent
    type SendOrderAcknowledgment =
        OrderAcknowledgment -> SendResult 
    
    type AcknowledgeOrder = 
        CreateOrderAcknowledgmentLetter     // dependency
          -> SendOrderAcknowledgment        // dependency
          -> PricedOrder                    // input
          -> OrderAcknowledgmentSent option // output
    //<

    //>acknowledgeOrder 
    let acknowledgeOrder : AcknowledgeOrder = 
        fun createAcknowledgmentLetter sendAcknowledgment pricedOrder ->
            let letter = createAcknowledgmentLetter pricedOrder
            let acknowledgment = {
                EmailAddress = pricedOrder.CustomerInfo.EmailAddress
                Letter = letter 
                }

            // if the acknowledgment was successfully sent,
            // return the corresponding event, else return None
            match sendAcknowledgment acknowledgment with
            | Sent -> 
                let event = {
                    OrderId = pricedOrder.OrderId
                    EmailAddress = pricedOrder.CustomerInfo.EmailAddress
                    } 
                Some event
            | NotSent ->
                None
    //<

module CreateEvents = 

    open SimpleTypes
    open CommonTypes

    //>CreateEvents_Design
    /// Event to send to shipping context
    type OrderPlaced = PricedOrder

    /// Event to send to billing context
    /// Will only be created if the AmountToBill is not zero
    type BillableOrderPlaced = {
        OrderId : OrderId
        BillingAddress: Address
        AmountToBill : BillingAmount
        }

    type PlaceOrderEvent = 
        | OrderPlaced of OrderPlaced
        | BillableOrderPlaced of BillableOrderPlaced 
        | AcknowledgmentSent  of OrderAcknowledgmentSent

    type CreateEvents = 
      PricedOrder                            // input
        -> OrderAcknowledgmentSent option    // input (event from previous step)
        -> PlaceOrderEvent list              // output
    //<

    //>createBillingEvent 
    // PricedOrder -> BillableOrderPlaced option 
    let createBillingEvent (placedOrder:PricedOrder) : BillableOrderPlaced option =
        let billingAmount = placedOrder.AmountToBill |> BillingAmount.value
        if billingAmount > 0M then
            let order = {
                OrderId = placedOrder.OrderId
                BillingAddress = placedOrder.BillingAddress
                AmountToBill = placedOrder.AmountToBill 
            }
            Some order 
        else
            None
    //<

    //>listOfOption 
    /// convert an Option into a List
    let listOfOption opt =
        match opt with 
        | Some x -> [x]
        | None -> []
    //<

    module CreateEventsA = 
        //>createEventsA 
        let createEvents : CreateEvents = 
            fun pricedOrder acknowledgmentEventOpt ->
                let event1 = 
                    pricedOrder
                    // convert to common choice type
                    |> PlaceOrderEvent.OrderPlaced
                let event2Opt =
                    acknowledgmentEventOpt 
                    // convert to common choice type
                    |> Option.map PlaceOrderEvent.AcknowledgmentSent
                let event3Opt = 
                    pricedOrder
                    |> createBillingEvent 
                    // convert to common choice type
                    |> Option.map PlaceOrderEvent.BillableOrderPlaced

                // return all the events how?
                dotDotDot()
        //<

    //>createEvents 
    let createEvents : CreateEvents = 
        fun pricedOrder acknowledgmentEventOpt ->
            let events1 = 
                pricedOrder
                // convert to common choice type
                |> PlaceOrderEvent.OrderPlaced
                // convert to list
                |> List.singleton
            let events2 =
                acknowledgmentEventOpt 
                // convert to common choice type
                |> Option.map PlaceOrderEvent.AcknowledgmentSent
                // convert to list
                |> listOfOption
            let events3 = 
                pricedOrder
                |> createBillingEvent 
                // convert to common choice type
                |> Option.map PlaceOrderEvent.BillableOrderPlaced
                // convert to list
                |> listOfOption

            // return all the events
            [
            yield! events1
            yield! events2
            yield! events3
            ]            
    //<

module Composing = 
    open SimpleTypes
    open CommonTypes

    let validateOrder _ = dotDotDot()
    let priceOrder _ = dotDotDot()
    let acknowledgeOrder _ = dotDotDot()
    let createEvents _ = dotDotDot()
    type PlaceOrderEvent = DotDotDot
    type PlaceOrderWorkflow  = 
        UnvalidatedOrder -> PlaceOrderEvent list

    //>Composing1 
    let placeOrder : PlaceOrderWorkflow =
        fun unvalidatedOrder ->
           unvalidatedOrder
           |> validateOrder
           |> priceOrder
           |> acknowledgeOrder
           |> createEvents
    //<

    let checkProductCodeExists _ = dotDotDot()
    let checkAddressExists _ = dotDotDot()

    //>Composing2 
    //let validateOrderWithDependenciesBakedIn = 
    //   validateOrder checkProductCodeExists checkAddressExists

    // new function signature after partial application:    
    // UnvalidatedOrder -> ValidatedOrder       
    //< 

    do
        //>Composing3 
        //let validateOrder = 
        //   validateOrder checkProductCodeExists checkAddressExists
        //< 
        ()

    //>Composing4 
    //let validateOrder' = 
    //   validateOrder checkProductCodeExists checkAddressExists
    //< 


module ComposingWithPA = 

    let validateOrder _ = dotDotDot()
    let priceOrder _ = dotDotDot()
    let sendOrderToShippingAndBilling _ = dotDotDot()
    let checkProductCodeExists _ = dotDotDot()
    let checkAddressExists _ = dotDotDot()
    let getProductPrice _ = dotDotDot()
    let acknowledgeOrder _ = dotDotDot()
    let createAcknowledgmentLetter _ = dotDotDot()
    let sendAcknowledgment _ = dotDotDot()
    let createEvents _ = dotDotDot()
    
    type PlaceOrderWorkflow  = Composing.PlaceOrderWorkflow 

    //>ComposingWithPA
    let placeOrder : PlaceOrderWorkflow =

        // set up local versions of the pipeline stages
        // using partial application to bake in the dependencies
        let validateOrder = 
            validateOrder checkProductCodeExists checkAddressExists
        let priceOrder = 
            priceOrder getProductPrice 
        let acknowledgeOrder = 
            acknowledgeOrder createAcknowledgmentLetter sendAcknowledgment 

        // return the workflow function
        fun unvalidatedOrder ->
          
            // compose the pipeline from the new one-parameter functions
            unvalidatedOrder 
            |> validateOrder
            |> priceOrder
            |> acknowledgeOrder 
            |> createEvents
    //<

    module PlaceOrder2 = 
        let createEvents _ _ = dotDotDot()

        //>ComposingWithPA2
        let placeOrder : PlaceOrderWorkflow =       
            // return the workflow function
            fun unvalidatedOrder -> 
                let validatedOrder = 
                    unvalidatedOrder 
                    |> validateOrder checkProductCodeExists checkAddressExists 
                let pricedOrder = 
                    validatedOrder 
                    |> priceOrder getProductPrice 
                let acknowledgmentOption = 
                    pricedOrder 
                    |> acknowledgeOrder createAcknowledgmentLetter sendAcknowledgment 
                let events = 
                    createEvents pricedOrder acknowledgmentOption 
                events
        //<

module InjectingDependencies =

    //>InjectingDependencies1 
    // low-level helper functions
    let toAddress checkAddressExists unvalidatedAddress =
        dotDotDot()

    let toProductCode checkProductCodeExists productCode = 
        dotDotDot()
    //<    

    //>InjectingDependencies2 
    // helper function
    let toValidatedOrderLine checkProductExists unvalidatedOrderLine = 
    //                       ^ needed for toProductCode, below
    
        // create the components of the line
        let orderLineId = dotDotDot()
        let productCode = 
            unvalidatedOrderLine.ProductCode 
            |> toProductCode checkProductExists //use service

        dotDotDot()
    //<    

module InjectingDependencies2 =
    open InjectingDependencies

    let toValidatedOrderLine _ _ = dotDotDot()
    type ValidateOrder = ValidationImplementation1.ValidateOrder 

    //>InjectingDependencies3 
    let validateOrder : ValidateOrder = 
        fun checkProductExists // dependency for toValidatedOrderLine
          checkAddressExists   // dependency for toAddress
          unvalidatedOrder ->

            // build the validated address using the dependency 
            let shippingAddress = 
                unvalidatedOrder.ShippingAddress 
                |> toAddress checkAddressExists
       
            //...
      
            // build the validated order lines using the dependency 
            let lines = 
                unvalidatedOrder.Lines 
                |> List.map (toValidatedOrderLine checkProductExists) 

            dotDotDot()
    //<    

module InjectingDependencies3 =
    open InjectingDependencies
    open InjectingDependencies2

    let validateOrder _ _  _ = dotDotDot()
    let priceOrder _ _ = dotDotDot()
    let acknowledgeOrder _ _ _ = dotDotDot()
    let createEvents _ = dotDotDot()

    type PlaceOrderWorkflow  = 
        DotDotDot -> DotDotDot list


    //>InjectingDependencies4 
    let placeOrder 
      checkProductExists               // dependency
      checkAddressExists               // dependency
      getProductPrice                  // dependency
      createOrderAcknowledgmentLetter  // dependency
      sendOrderAcknowledgment          // dependency
      : PlaceOrderWorkflow =           // function definition 

        fun unvalidatedOrder -> 
            dotDotDot()

    //<    

module InjectingDependencies5 =
    open InjectingDependencies
    open InjectingDependencies2
    open InjectingDependencies3

    type WebPart = DotDotDot
    let choose _ = dotDotDot()
    let POST _ = dotDotDot()
    let path _ = dotDotDot()
    let placeOrder _ _ _ _ _ = dotDotDot()
    let deserializeOrder _ = dotDotDot()
    let postEvents _ = dotDotDot()
    let (>=>) x y = x >> y

    //>InjectingDependencies5
    let app : WebPart =
        
        // set up the services used by the workflow
        let checkProductExists = dotDotDot()
        let checkAddressExists = dotDotDot()
        let getProductPrice = dotDotDot()
        let createOrderAcknowledgmentLetter = dotDotDot()
        let sendOrderAcknowledgment = dotDotDot()
        let toHttpResponse = dotDotDot()

        // set up the "placeOrder" workflow
        // by partially applying the services to it
        let placeOrder = 
            placeOrder 
                checkProductExists 
                checkAddressExists 
                getProductPrice 
                createOrderAcknowledgmentLetter 
                sendOrderAcknowledgment 

        // set up the other workflows
        let changeOrder = dotDotDot()
        let cancelOrder = dotDotDot()

        // set up the routing
        choose 
            [ POST >=> choose
                [ path "/placeOrder" 
                    >=> deserializeOrder // convert JSON to UnvalidatedOrder
                    >=> placeOrder       // do the workflow
                    >=> postEvents       // post the events onto queues
                    >=> toHttpResponse   // return 200/400/etc based on the output
                  path "/changeOrder"
                    >=> dotDotDot()
                  path "/cancelOrder"
                    >=> dotDotDot()
                ]
            ]
    //<    


module TooManyDependencies = 

    // checkAddressExists service now has 2 extra parameters

    //>TooManyDependencies1 
    let checkAddressExists endPoint credentials = 
        dotDotDot()
    //<

    //>TooManyDependencies2 
    let toAddress checkAddressExists endPoint credentials unvalidatedAddress = 
    //                           only ^ needed ^ for checkAddressExists

        // call the remote service
        let checkedAddress = checkAddressExists endPoint credentials unvalidatedAddress 
        //                     2 extra parameters ^ passed in ^
        dotDotDot()
    //<

    //>TooManyDependencies3 
    let validateOrder 
        checkProductExists 
        checkAddressExists 
        endPoint    // only needed for checkAddressExists 
        credentials // only needed for checkAddressExists 
        unvalidatedOrder = 
            dotDotDot()
    //<


    let checkProductCodeExists _ _ = dotDotDot()

    type PlaceOrderWorkflow  = 
        DotDotDot -> DotDotDot list

    //>TooManyDependencies4 
    let placeOrder : PlaceOrderWorkflow =

        // initialize information (e.g from configuration)
        let endPoint = dotDotDot
        let credentials = dotDotDot
    
        // make a new version of checkAddressExists
        // with the credentials baked in
        let checkAddressExists = checkAddressExists endPoint credentials
        // etc
      
        // set up the steps in the workflow
        let validateOrder = 
            validateOrder checkProductCodeExists checkAddressExists
            //               the new checkAddressExists ^ 
            //               is a one parameter function 
        // etc

        // return the workflow function
        fun unvalidatedOrder -> 
            // compose the pipeline from the steps
            dotDotDot()
    //< 

#r "../../packages/NUnit/lib/net45/nunit.framework.dll"

module Testing = 

    let validateOrder _ _ _ _ = dotDotDot()
    type CheckedAddress = CheckedAddress of UnvalidatedAddress

    //>Testing1
    open NUnit.Framework

    [<Test>]
    let ``If product exists, validation succeeds``() =
       // arrange: set up stub versions of service dependencies
       let checkAddressExists address = 
           CheckedAddress address // succeed 
       let checkProductCodeExists productCode = 
           true                   // succeed
       
       // arrange: set up input       
       let unvalidatedOrder = dotDotDot()
       
       // act: call validateOrder
       let result = validateOrder checkProductCodeExists checkAddressExists dotDotDot
   
       // assert: check that result is a ValidatedOrder, not an error
       dotDotDot()
    //< 

    //>Testing2
    let checkProductCodeExists productCode = 
       false  // fail
    //< 

    module Complete =

        //>Testing3
        [<Test>]
        let ``If product doesn't exist, validation fails``() =
           // arrange: set up stub versions of service dependencies
           let checkAddressExists address = dotDotDot()
           let checkProductCodeExists productCode = 
               false // fail
       
           // arrange: set up input       
           let unvalidatedOrder = dotDotDot()
       
           // act: call validateOrder
           let result = validateOrder checkProductCodeExists checkAddressExists dotDotDot
   
           // assert: check that result is a failure
           dotDotDot()
        //< 


// =====================================
// =====================================

 module CompletePipeline = 

    open SimpleTypes
    open CommonTypes

    module API = ()

    //>CompletePipeline1 
    module PlaceOrderWorkflow =
    
        // make the shared simple types (such as 
        // String50 and ProductCode) available.
        open SimpleTypes    

        // make the public types exposed to the
        // callers available
        open API 

        // ==============================
        // Part 1: Design
        // ==============================

        // NOTE: the public parts of the workflow -- the API -- 
        // such as the `PlaceOrderWorkflow` function and its
        // input `UnvalidatedOrder`, are defined elsewhere.
        // The types below are private to the workflow implementation.

        // ----- Validate Order ----- 

        type CheckProductCodeExists = 
            ProductCode -> bool
        type CheckedAddress = 
            CheckedAddress of UnvalidatedAddress
        type CheckAddressExists = 
            UnvalidatedAddress -> CheckedAddress
        type ValidateOrder = 
            CheckProductCodeExists    // dependency
              -> CheckAddressExists   // dependency
              -> UnvalidatedOrder     // input
              -> ValidatedOrder       // output


        // ----- Price order ----- 

        type GetProductPrice = DotDotDot
        type PriceOrder = DotDotDot
        // etc
    //< 


        //>CompletePipeline2 
        // ==============================
        // Part 2: Implementation 
        // ==============================

        // ------------------------------
        // ValidateOrder implementation
        // ------------------------------

        let toCustomerInfo (unvalidatedCustomerInfo: UnvalidatedCustomerInfo) = 
            dotDotDot()

        let toAddress (checkAddressExists:CheckAddressExists) unvalidatedAddress =
            dotDotDot()

        let predicateToPassthru _ = dotDotDot()

        let toProductCode (checkProductCodeExists:CheckProductCodeExists) productCode = 
            dotDotDot()

        let toOrderQuantity productCode quantity = 
            dotDotDot()
                
        let toValidatedOrderLine checkProductExists (unvalidatedOrderLine:UnvalidatedOrderLine) = 
            dotDotDot()

        /// Implementation of ValidateOrder step
        let validateOrder : ValidateOrder = 
            fun checkProductCodeExists checkAddressExists unvalidatedOrder ->
                let orderId = 
                    unvalidatedOrder.OrderId 
                    |> OrderId.create
                let customerInfo = dotDotDot()
                let shippingAddress = dotDotDot()
                let billingAddress = dotDotDot()
                let lines = 
                    unvalidatedOrder.Lines 
                    |> List.map (toValidatedOrderLine checkProductCodeExists) 
                let validatedOrder : ValidatedOrder = {
                    OrderId  = orderId 
                    CustomerInfo = customerInfo 
                    ShippingAddress = shippingAddress 
                    BillingAddress = billingAddress  
                    Lines = lines 
                }
                validatedOrder 
        //< 

        // mask previous implementation
        let validateOrder _ = dotDotDot()
        let priceOrder _ = dotDotDot()
        let acknowledgeOrder _ = dotDotDot()
        let createEvents _ = dotDotDot()

        type PlaceOrderWorkflow = DotDotDot -> unit

        //>CompletePipeline3
        // ------------------------------
        // The complete workflow
        // ------------------------------
        let placeOrder 
            checkProductExists              // dependency
            checkAddressExists              // dependency
            getProductPrice                 // dependency
            createOrderAcknowledgmentLetter // dependency
            sendOrderAcknowledgment         // dependency
            : PlaceOrderWorkflow =          // definition of function

            fun unvalidatedOrder -> 
                let validatedOrder = 
                    unvalidatedOrder 
                    |> validateOrder checkProductExists checkAddressExists 
                let pricedOrder = 
                    validatedOrder 
                    |> priceOrder getProductPrice 
                let acknowledgmentOption = 
                    pricedOrder 
                    |> acknowledgeOrder createOrderAcknowledgmentLetter sendOrderAcknowledgment 
                let events = 
                    createEvents pricedOrder acknowledgmentOption 
                events
        //< 