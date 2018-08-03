//===================================
// Code snippets from chapter 13
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

// ============================
// Shipping cost examples
// ============================

type Address = {
    //...
    State : string
    Country : string
    }

type ValidatedOrder = {
    //...
    ShippingAddress : Address
    }

module CalculateShippingCost1 = 
    //>CalculateShippingCost1  
    /// Calculate the shipping cost for an order
    let calculateShippingCost validatedOrder = 
        let shippingAddress = validatedOrder.ShippingAddress 
        if shippingAddress.Country = "US" then
            // shipping inside USA
            match shippingAddress.State with
            | "CA" | "OR" | "AZ" | "NV" -> 
                5.0 //local
            | _ -> 
                10.0 //remote
        else
            // shipping outside USA
            20.0
    //<

module CalculateShippingCost2 = 
    //>CalculateShippingCost2a  
    let (|UsLocalState|UsRemoteState|International|) address = 
        if address.Country = "US" then
            match address.State with
            | "CA" | "OR" | "AZ" | "NV" -> 
                UsLocalState
            | _ -> 
                UsRemoteState
        else
            International
    //<

    //>CalculateShippingCost2b  
    let calculateShippingCost validatedOrder = 
        match validatedOrder.ShippingAddress with
        | UsLocalState -> 5.0
        | UsRemoteState -> 10.0
        | International -> 20.0
    //<

    type PricedOrder = DotDotDot
    type PricedOrderWithShippingInfo = DotDotDot
    //>AddShippingInfoToOrder 
    type AddShippingInfoToOrder = PricedOrder -> PricedOrderWithShippingInfo
    //<

    type Price = decimal

    //>ShippingInfo 
    type ShippingMethod = 
        | PostalService 
        | Fedex24 
        | Fedex48 
        | Ups48

    type ShippingInfo = {
        ShippingMethod : ShippingMethod
        ShippingCost : Price
        }

    type PricedOrderWithShippingMethod = {
        ShippingInfo : ShippingInfo 
        PricedOrder : PricedOrder
        }
    //<

    module PricedOrder1 =

        //>PricedOrder1 
        type PricedOrder = {
            //... 
            ShippingInfo : ShippingInfo
            OrderTotal : Price
            }
        //<

    module PricedOrder2 =

        type PricedOrderProductLine = DotDotDot

        //>PricedOrder2 
        type PricedOrderLine = 
            | Product of PricedOrderProductLine
            | ShippingInfo of ShippingInfo
        //<

    module AddShippingInfoToOrder2 =
        type PricedOrder = {
            OrderId : DotDotDot
            }
        type PricedOrderWithShippingInfo = {
            OrderId : DotDotDot
            ShippingInfo : ShippingInfo
            }
        type AddShippingInfoToOrder = PricedOrder -> PricedOrderWithShippingInfo

        //>addShippingInfoToOrderImpl 
        let addShippingInfoToOrder calculateShippingCost : AddShippingInfoToOrder  =
            fun pricedOrder ->
                // create the shipping info
                let shippingInfo = {
                    ShippingMethod = dotDotDot()
                    ShippingCost = calculateShippingCost pricedOrder 
                    }

                // add it to the order
                {
                OrderId = pricedOrder.OrderId
                //...
                ShippingInfo = shippingInfo
                }
        //<

        let unvalidatedOrder _ = dotDotDot()
        let calculateShippingCost = dotDotDot()

        //>AddShippingInfoToOrderWorkflow
        // set up local versions of the pipeline stages
        // using partial application to bake in the dependencies
        let validateOrder unvalidatedOrder = dotDotDot()
        let priceOrder validatedOrder = dotDotDot()
        let addShippingInfo = addShippingInfoToOrder calculateShippingCost 
          
        // compose the pipeline from the new one-parameter functions
        unvalidatedOrder
        |> validateOrder
        |> priceOrder
        |> addShippingInfo
        //...
        //<


// ============================
// VIP examples
// ============================

module VipModel1 =

    //>VipModel1
    type CustomerInfo = {
        //...
        IsVip : bool
        //...
        }
    //<

module VipModel2 = 

    type CustomerInfo = DotDotDot

    //>VipModel2
    type CustomerStatus =
        | Normal of CustomerInfo
        | Vip of CustomerInfo
    
    type Order = {
        //...
        CustomerStatus : CustomerStatus
        //...
        }
    //<

module VipModel3 = 


    //>VipModel3
    type VipStatus =
        | Normal 
        | Vip 
    
    type CustomerInfo = {
        //...
        VipStatus : VipStatus
        //...
        }
    //<

module LoyaltyCardModel = 

    type VipStatus = DotDotDot

    //>LoyaltyCard
    type LoyaltyCardId = DotDotDot
    type LoyaltyCardStatus =
        | None
        | LoyaltyCard of LoyaltyCardId
    
    type CustomerInfo = {
        //...
        VipStatus : VipStatus
        LoyaltyCardStatus : LoyaltyCardStatus
        //...
        }
    //<


module VipChangesInput =

    //>VipChangesInput1 
    type VipStatus = DotDotDot

    type CustomerInfo = {
       //... 
       VipStatus : VipStatus
       }
    //<

    //>VipChangesInput2 
    module Domain = 
        type UnvalidatedCustomerInfo = {
           //... 
           VipStatus : string
           }

    module Dto =
        type CustomerInfo = {
           //... 
           VipStatus : string
           }
    //<

    module VipStatus =
        let create str :Result<VipStatus,string> = dotDotDot()
    
    module ValidateCustomerInfo  =
        open Domain

        //>VipChangesInput3  
        let validateCustomerInfo unvalidatedCustomerInfo = 
            result {
                //...

                // new field
                let! vipStatus = 
                    VipStatus.create unvalidatedCustomerInfo.VipStatus 

                let customerInfo : CustomerInfo = {
                    //...
                    VipStatus = vipStatus
                    }
                return customerInfo
            }
        //<    

module CalculateVipShipping =
    type VipStatus =
        | Normal 
        | Vip 

    type ShippingMethod = 
        | PostalService 
        | Fedex24 
        | Fedex48 
        | Ups48

    type ShippingInfo = {
        ShippingMethod : ShippingMethod 
        ShippingCost : float
        }
    type PricedOrderWithShippingMethod = {
        VipStatus : VipStatus 
        ShippingInfo : ShippingInfo
        }

    //>FreeVipShippingType
    type FreeVipShipping = 
        PricedOrderWithShippingMethod -> PricedOrderWithShippingMethod
    //<


    //>freeVipShipping 
    /// Update the shipping cost if customer is VIP
    let freeVipShipping : FreeVipShipping =
        fun order -> 
            let updatedShippingInfo = 
                match order.VipStatus with
                | Normal -> 
                    // untouched
                    order.ShippingInfo
                | Vip -> 
                    {order.ShippingInfo with 
                        ShippingCost = 0.0
                        ShippingMethod=Fedex24 }

            {order with ShippingInfo = updatedShippingInfo }
    //<

    let updatedWorkflow() =
        let unvalidatedOrder _ = dotDotDot()
        let calculateShippingCost = dotDotDot()

        //>FreeVipShippingWorkflow 
        // set up local versions of the pipeline stages
        // using partial application to bake in the dependencies
        let validateOrder unvalidatedOrder = dotDotDot()
        let priceOrder validatedOrder = dotDotDot()
        let addShippingInfo pricedOrder = dotDotDot()
          
        // compose the pipeline from the new one-parameter functions
        unvalidatedOrder
        |> validateOrder
        |> priceOrder
        |> addShippingInfo
        |> freeVipShipping
        //...
        //<

// ============================
// Promotion code examples
// ============================

module PromotionCode1 =

    //>PromotionCode1 
    type PromotionCode = PromotionCode of string

    type ValidatedOrder = {
       //... 
       PromotionCode : PromotionCode option
       }
    //<

    //>PromotionCode2 
    type OrderDto = {
       //... 
       PromotionCode : string
       }

    type UnvalidatedOrder = {
       //... 
       PromotionCode : string
       }
    //<

    module PromotionCode =
        let create str = PromotionCode str
    
    module ValidateOrder =
        let createValidatedOrder _ = dotDotDot()

        //>PromotionCode3  
        let validateOrder unvalidatedOrder = 
           
           let orderIdResult = dotDotDot()
           dotDotDot()

           // new field
           let promotionCodeResult = 
               PromotionCode.create unvalidatedOrder.PromotionCode
           dotDotDot()
        //<    

module GetProductPrice =
    type ProductCode = ProductCode of string
    type Price = decimal
    type PromotionCode = PromotionCode of string

    //>GetProductPriceType 
    type GetProductPrice = ProductCode -> Price 
    //<
    
    module GetPricingFunction1 = 
        //>GetPricingFunctionType 
        type GetPricingFunction = PromotionCode option -> GetProductPrice
        //<

    //>PricingMethodType 
    type PricingMethod =
       | Standard
       | Promotion of PromotionCode 
    //<

    //>ValidatedOrderWithPricingMethod
    type ValidatedOrder = {
       //... //as before
       PricingMethod : PricingMethod
       }
    //<

    //>GetPricingFunctionType2 
    type GetPricingFunction = PricingMethod -> GetProductPrice
    //<

    type PricedOrder = DotDotDot

    //>PriceOrderType 
    type PriceOrder = 
       GetPricingFunction  // new dependency 
         -> ValidatedOrder // input 
         -> PricedOrder    // output 
    //<

module GetPricingFunctioImpl = 
    open GetProductPrice
    open System.Collections.Generic

    //>getPricingFunction 
    type GetStandardPriceTable = 
        // no input -> return standard prices
        unit -> IDictionary<ProductCode,Price>

    type GetPromotionPriceTable = 
        // promo input -> return prices for promo
        PromotionCode -> IDictionary<ProductCode,Price>

    let getPricingFunction 
        (standardPrices:GetStandardPriceTable) 
        (promoPrices:GetPromotionPriceTable)  
        : GetPricingFunction = 
  

        // the original pricing function
        let getStandardPrice : GetProductPrice =
            // cache the standard prices		
            let standardPrices = standardPrices()
            // return the lookup function
            fun productCode -> standardPrices.[productCode] 

        // the promotional pricing function
        let getPromotionPrice promotionCode : GetProductPrice =
            // cache the promotional prices
            let promotionPrices = promoPrices promotionCode
            // return the lookup function
            fun productCode ->
                match promotionPrices.TryGetValue productCode with
                // found in promotional prices
                | true,price -> price 
                // not found in promotional prices
                // so use standard price
                | false, _ -> getStandardPrice productCode

        // return a function that conforms to GetPricingFunction
        fun pricingMethod ->
            match pricingMethod with
            | Standard -> 
                getStandardPrice
            | Promotion promotionCode -> 
                getPromotionPrice promotionCode 
    //<

module CommentLine = 

    open GetProductPrice
    type PricedOrderProductLine = DotDotDot
    type String100 = String100 of string

    //>CommentLine
    type CommentLine = CommentLine of string

    type PricedOrderLine = 
        | Product of PricedOrderProductLine
        | Comment of CommentLine
    //<

    type PricingMethod = GetProductPrice.PricingMethod
    type ValidatedOrderLine = DotDotDot
    type ValidatedOrder = {
        PricingMethod : PricingMethod
        OrderLines : ValidatedOrderLine list
        }
    type PricedOrder = {
        OrderLines : PricedOrderLine list
        }
    type GetPricingFunction = 
        GetProductPrice.GetPricingFunction 
    type PriceOrder = 
       GetPricingFunction // dependency 
        -> ValidatedOrder // input 
        -> PricedOrder // output 

    module PromotionCode =
        let value (PromotionCode v) = v

    module CommentLine =
        let create s = CommentLine s

    //>PriceOrderWithCommentLine 
    let toPricedOrderLine orderLine = dotDotDot()

    let priceOrder : PriceOrder =
        fun getPricingFunction validatedOrder -> 
            // get the pricing function from the getPricingFunction "factory"
            let getProductPrice = getPricingFunction validatedOrder.PricingMethod
    
            // set the price for each line
            let productOrderLines = 
               validatedOrder.OrderLines 
               |> List.map (toPricedOrderLine getProductPrice) 
       
            // add the special comment line if needed		   
            let orderLines =
                match validatedOrder.PricingMethod with
                | Standard -> 
                    // unchanged
                    productOrderLines
                | Promotion promotion ->  
                    let promoCode = promotion|> PromotionCode.value
                    let commentLine = 
                        sprintf "Applied promotion %s" promoCode 
                        |> CommentLine.create 
                        |> Comment // lift to PricedOrderLine
                    List.append productOrderLines [commentLine]

            // return the new order    
            { 
                //... 
                OrderLines = orderLines
            }
    //<


module ShipmentConsumer =
    type ProductCode = DotDotDot
    type Quantity = Quantity
    type OrderId = DotDotDot
    type AddressDto = DotDotDot

    //>ShippableOrderPlaced 
    type ShippableOrderLine = {
        ProductCode : ProductCode
        Quantity : float
        }
    
    type ShippableOrderPlaced = {
        OrderId : OrderId
        ShippingAddress : Address
        ShipmentLines : ShippableOrderLine list
        }
    //<

    type BillableOrderPlaced = DotDotDot
    type OrderAcknowledgmentSent = DotDotDot

    //>PlaceOrderEvent 
    type PlaceOrderEvent = 
        | ShippableOrderPlaced of ShippableOrderPlaced
        | BillableOrderPlaced of BillableOrderPlaced 
        | AcknowledgmentSent  of OrderAcknowledgmentSent
    //<
    

// ============================
// Business hours examples
// ============================

module BusinessHours1 =

    //>BusinessHours1 
    /// Determine the business hours
    let isBusinessHour hour = 
        hour >= 9 && hour <= 17 

    /// tranformer
    let businessHoursOnly getHour onError onSuccess = 
        let hour = getHour()
        if isBusinessHour hour then
            onSuccess()
        else
            onError()
    //<

    type ValidationError = DotDotDot

    //>PlaceOrderErrorType
    type PlaceOrderError = 
        | Validation of ValidationError 
        //...
        | OutsideBusinessHours  //new!
    //<

    //>businessHoursWorkflow 
    let placeOrder unvalidatedOrder = 
            dotDotDot()

    let placeOrderInBusinessHours unvalidatedOrder = 
        let onError() = 
            Error OutsideBusinessHours
        let onSuccess() = 
            placeOrder unvalidatedOrder 
        let getHour() = DateTime.Now.Hour
        businessHoursOnly getHour onError onSuccess
    //<
