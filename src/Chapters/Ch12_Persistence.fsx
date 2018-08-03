//===================================
// Code snippets from chapter 12
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


module PersistenceToEdges1 =

    type Invoice = Invoice of unit
        with 
        member this.ApplyPayment _ = notImplemented()
        member this.IsFullyPaid = true

    let loadInvoiceFromDatabase() : Invoice = notImplemented()
    let markAsFullyPaidInDb _ = notImplemented()
    let markAsPartiallyPaidInDb _ = notImplemented()
    let postInvoicePaidEvent _ = notImplemented()

    //>PersistenceToEdges1 
    // workflow mixes domain logic and I/O
    let payInvoice invoiceId payment =
        // load from DB
        let invoice = loadInvoiceFromDatabase(invoiceId)

        // apply payment
        invoice.ApplyPayment(payment)

        // handle different outcomes
        if invoice.IsFullyPaid then
            markAsFullyPaidInDb(invoiceId)
            postInvoicePaidEvent(invoiceId)
        else
            markAsPartiallyPaidInDb(invoiceId)
    //<

module PersistenceToEdges2 =

    type UpdatedInvoice = DotDotDot    
    let applyPayment _ = dotDotDot()
    type UnpaidInvoice = DotDotDot
    let isFullyPaid _ :bool = dotDotDot() 

    //>PersistenceToEdges2a 
    type InvoicePaymentResult = 
        | FullyPaid 
        | PartiallyPaid of DotDotDot

    // domain workflow: pure function
    let applyPaymentToInvoice unpaidInvoice payment :InvoicePaymentResult =
        // apply payment
        let updatedInvoice = unpaidInvoice |> applyPayment payment

        // handle different outcomes
        if isFullyPaid updatedInvoice then
            FullyPaid 
        else
            PartiallyPaid updatedInvoice 
        // return PartiallyPaid or FullyPaid
    //<

    let loadInvoiceFromDatabase _ = notImplemented()
    let markAsFullyPaidInDb _ = notImplemented()
    let markAsPartiallyPaidInDb _ = notImplemented()
    let postInvoicePaidEvent _ = notImplemented()
    let updateInvoiceInDb _ = notImplemented()

    //>PersistenceToEdges2b
    type PayInvoiceCommand = {
        InvoiceId : DotDotDot
        Payment : DotDotDot
        }

    // command handler at the edge of the bounded context
    let payInvoice payInvoiceCommand =
        // load from DB
        let invoiceId = payInvoiceCommand.InvoiceId           
        let unpaidInvoice = 
            loadInvoiceFromDatabase invoiceId  // I/O

        // call into pure domain
        let payment = 
            payInvoiceCommand.Payment          // pure
        let paymentResult = 
            applyPayment unpaidInvoice payment // pure

        // handle result
        match paymentResult with
        | FullyPaid ->
            markAsFullyPaidInDb invoiceId      // I/O
            postInvoicePaidEvent invoiceId     // I/O  
        | PartiallyPaid updatedInvoice ->
            updateInvoiceInDb updatedInvoice   // I/O 
    //<

    module WithInjection =
        //>PersistenceToEdges2c
        // command handler at the edge of the bounded context
        let payInvoice 
            loadUnpaidInvoiceFromDatabase // dependency
            markAsFullyPaidInDb           // dependency
            updateInvoiceInDb             // dependency
            payInvoiceCommand =

            // load from DB
            let invoiceId = payInvoiceCommand.InvoiceId
            let unpaidInvoice = 
                loadUnpaidInvoiceFromDatabase invoiceId 

            // call into pure domain
            let payment = 
                payInvoiceCommand.Payment
            let paymentResult = 
                applyPayment unpaidInvoice payment

            // handle result
            match paymentResult with
            | FullyPaid ->
                markAsFullyPaidInDb(invoiceId)
                postInvoicePaidEvent(invoiceId)
            | PartiallyPaid updatedInvoice ->
                updateInvoiceInDb updatedInvoice   
        //<

    


module Crud1 =
    type DataStoreState = Undefined 
    type NewDataStoreState = Undefined 
    type Data = Undefined 

    //>Crud1 
    type InsertData = DataStoreState -> Data -> NewDataStoreState
    //<

module Crud2 =
    type DataStoreState = Undefined 
    type NewDataStoreState = Undefined 
    type Data = Undefined 
    type Query = Undefined 
    type Key = Undefined 

    //>Crud2 
    type InsertData = DataStoreState -> Data -> NewDataStoreState
    type ReadData = DataStoreState -> Query -> Data
    type UpdateData = DataStoreState -> Data -> NewDataStoreState
    type DeleteData = DataStoreState -> Key -> NewDataStoreState
    //<

module Crud3 =

    type DbConnection = Undefined 
    type Data = Undefined 
    type Query = Undefined
    type Key = Undefined  

    //>Crud3 
    type InsertData = DbConnection -> Data -> Unit
    type ReadData = DbConnection -> Query -> Data
    type UpdateData = DbConnection -> Data -> Unit
    type DeleteData = DbConnection -> Key -> Unit
    //<

module Crud4 =

    type Data = Undefined 
    type Query = Undefined 
    type Key = Undefined 

    //>Crud4 
    type InsertData = Data -> Unit
    type ReadData = Query -> Data
    type UpdateData = Data -> Unit
    type DeleteData = Key -> Unit
    //<

module Crud5 =

    type Data = Undefined 
    type Query = Undefined 
    type Key = Undefined 
    

    //>Crud5
    type DbError = DotDotDot
    type DbResult<'a> = AsyncResult<'a,DbError>

    type InsertData = Data -> DbResult<Unit>
    type ReadData = Query -> DbResult<Data>
    type UpdateData = Data -> DbResult<Unit>
    type DeleteData = Key -> DbResult<Unit>
    //<


module CQRS1 =

    type Customer = Undefined 
    type CustomerId = Undefined 
    type DbResult<'a> = DbResult of 'a

    //>CQRS1 
    type SaveCustomer = Customer -> DbResult<Unit>
    type LoadCustomer = CustomerId -> DbResult<Customer>   
    //<


module CQRS2 =
    module WriteModel =
        type Customer = Undefined 
    module ReadModel =
        type Customer = Undefined 
    type CustomerId = Undefined 
    type DbResult<'a> = DbResult of 'a

    //>CQRS2
    type SaveCustomer = WriteModel.Customer -> DbResult<Unit>
    type LoadCustomer = CustomerId -> DbResult<ReadModel.Customer>   
    //<

// =========================================================
// Document database example
//
// This snippets are designed to work with Azure
// =========================================================
#I "../../packages/WindowsAzure.Storage/lib/net45"
#r "Microsoft.WindowsAzure.Storage"

#I "../../packages/Newtonsoft.Json/lib/net45"
#r "Newtonsoft.Json"

module Json =

    open Newtonsoft.Json
    
    let serialize obj =
        JsonConvert.SerializeObject obj

    let deserialize<'a> str =
        try
           JsonConvert.DeserializeObject<'a> str
           |> Result.Ok
        with
        | ex -> 
           Result.Error ex


module AzureExample = 

    //>Azure1
    open Microsoft.WindowsAzure
    open Microsoft.WindowsAzure.Storage
    open Microsoft.WindowsAzure.Storage.Blob

    let connString = "... Azure connection string ..."
    let storageAccount = CloudStorageAccount.Parse(connString)
    let blobClient = storageAccount.CreateCloudBlobClient()

    let container = blobClient.GetContainerReference("Person");
    container.CreateIfNotExists()
    //<

    //>Azure2
    type PersonDto = {
        PersonId : int
        //...
        }

    let savePersonDtoToBlob personDto = 
        let blobId = sprintf "Person%i" personDto.PersonId 
        let blob = container.GetBlockBlobReference(blobId)
        let json = Json.serialize personDto 
        blob.UploadText(json)
    //<

// =========================================================
// Relational database example
//
// This snippets are designed to work with SqlExpress
// =========================================================
#I "../../packages/FSharp.Data.SqlClient/lib/net40"
#r "FSharp.Data.SqlClient"
open FSharp.Data

module RelationalDomain = 


    //>RelationalDomain_Record
    type CustomerId = CustomerId of int
    type String50 = String50 of string
    type Birthdate = Birthdate of DateTime 


    type Customer = {
      CustomerId : CustomerId
      Name : String50
      Birthdate : Birthdate option
      } 
    //<   

    (*>RelationalTable_Record
    CREATE TABLE Customer (
       CustomerId int NOT NULL, 
       Name NVARCHAR(50) NOT NULL, 
       Birthdate DATETIME NULL,
       CONSTRAINT PK_Customer PRIMARY KEY (CustomerId)
    )
    <*)



    //>RelationalDomain_Choice 
    type Contact = {
      ContactId : ContactId
      Info : ContactInfo 
      } 
  
    and ContactInfo =
      | Email of EmailAddress
      | Phone of PhoneNumber

    and EmailAddress = EmailAddress of string
    and PhoneNumber = PhoneNumber of string
    and ContactId = ContactId of int
    //<   



    (*>RelationalTable_Choice1
    CREATE TABLE ContactInfo (
       -- shared data
       ContactId int NOT NULL, 
       -- case flags
       IsEmail bit NOT NULL,
       IsPhone bit NOT NULL,
       -- data for the "Email" case
       EmailAddress NVARCHAR(100), -- Nullable
       -- data for the "Phone" case
       PhoneNumber NVARCHAR(25), -- Nullable
       -- primary key constraint
       CONSTRAINT PK_ContactInfo PRIMARY KEY (ContactId)
    )
    <*)


    (*>RelationalTable_Choice2
    -- Main table
    CREATE TABLE ContactInfo (
       -- shared data
       ContactId int NOT NULL, 
       -- case flags
       IsEmail bit NOT NULL,
       IsPhone bit NOT NULL,
       CONSTRAINT PK_ContactInfo PRIMARY KEY (ContactId)
    )

    -- Child table for "Email" case
    CREATE TABLE ContactEmail (
       ContactId int NOT NULL, 
       -- case-specific data
       EmailAddress NVARCHAR(100) NOT NULL,
       CONSTRAINT PK_ContactEmail PRIMARY KEY (ContactId)
    )

    -- Child table for "Phone" case
    CREATE TABLE ContactPhone (
       ContactId int NOT NULL, 
       -- case-specific data
       PhoneNumber NVARCHAR(25) NOT NULL,
       CONSTRAINT PK_ContactPhone PRIMARY KEY (ContactId)
    )
    <*)


    (*>RelationalTable_NestedTypes1
    CREATE TABLE Order (
       OrderId int NOT NULL, 
       -- and other columns
    )

    CREATE TABLE OrderLine (
       OrderLineId int NOT NULL, 
       OrderId int NOT NULL, 
       -- and other columns
    )
    <*)

    (*>RelationalTable_NestedTypes2
    CREATE TABLE Order (
       OrderId int NOT NULL, 

       -- inline the shipping address Value Object
       ShippingAddress1 varchar(50)
       ShippingAddress2 varchar(50)
       ShippingAddressCity varchar(50)
       -- and so on

       -- inline the billing address Value Object
       BillingAddress1 varchar(50)
       BillingAddress2 varchar(50)
       BillingAddressCity varchar(50)
       -- and so on

       -- other columns
    )
    <*)

open RelationalDomain
open System.Data.SqlClient

//>CompileTimeConnectionString
open FSharp.Data

[< Literal>]
let CompileTimeConnectionString = 
    @"Data Source=(localdb)\MsSqlLocalDb; Initial Catalog=DomainModelingExample;"
//<
    //tip add whitespace to connection string


module ReadCustomer = 

    //>ReadOneCustomerType
    type ReadOneCustomer = SqlCommandProvider<"""
        SELECT CustomerId, Name, Birthdate
        FROM Customer
        WHERE CustomerId = @customerId
        """, CompileTimeConnectionString>
    //<

    //>bindOption
    let bindOption f xOpt =
        match xOpt with
        | Some x -> f x |> Result.map Some
        | None -> Ok None
    //<

    module CustomerId =
        let create customerId = 
            if customerId > 0 then 
                Ok (CustomerId customerId) 
            else 
                Error (sprintf "Invalid customerId %i" customerId)

    module String50 = 
        let create fieldName str : Result<String50,string> = 
            if String.IsNullOrEmpty(str) then
                Error (fieldName + " must be non-empty")
            elif str.Length > 50 then
                Error (fieldName + " must be less that 50 chars")
            else
                Ok (String50 str)

    module Birthdate =
        let create (date:DateTime) : Result<Birthdate,string> = 
            if date.Year < 1900 then
                Error "Birthdate must be after 1900"
            elif date.Year > DateTime.Now.Year then
                Error "Birthdate must not be in the future"
            else
                Ok (Birthdate date) // no error checking

    //>CustomerToDomainUntrusted
    let toDomain (dbRecord:ReadOneCustomer.Record) : Result<Customer,_> =
        result {
            let! customerId = 
                dbRecord.CustomerId
                |> CustomerId.create
            let! name = 
                dbRecord.Name
                |> String50.create "Name"
            let! birthdate = 
                dbRecord.Birthdate
                |> Result.bindOption Birthdate.create
            let customer = {
                CustomerId = customerId
                Name = name
                Birthdate = birthdate
                }
            return customer
            }
    //<

    module TrustedDb = 
        //>PanicOnError 
        exception DatabaseError of string

        let panicOnError columnName result =
            match result with
            | Ok x -> x
            | Error err -> 
                let msg = sprintf "%s: %A" columnName err
                raise (DatabaseError msg)
        //<

        //>CustomerToDomainTrusted 
        let toDomain (dbRecord:ReadOneCustomer.Record) : Customer =
        
            let customerId = 
                dbRecord.CustomerId
                |> CustomerId.create
                |> panicOnError "CustomerId"

            let name = 
                dbRecord.Name
                |> String50.create "Name"
                |> panicOnError "Name"

            let birthdate = 
                dbRecord.Birthdate
                |> Result.bindOption Birthdate.create
                |> panicOnError "Birthdate"

            // return the customer
            {CustomerId = customerId; Name = name; Birthdate = birthdate}
        //<

    exception DatabaseError of string

    //>ReadOneCustomer
    type DbReadError =
        | InvalidRecord of string 
        | MissingRecord of string 

    let readOneCustomer (productionConnection:SqlConnection) (CustomerId customerId) =
        // create the command by instantiating the type we defined earlier
        use cmd = new ReadOneCustomer(productionConnection)
    
        // execute the command
        let records = cmd.Execute(customerId = customerId) |> Seq.toList
    
        // handle the possible cases
        match records with
        // none found
        | [] -> 
            let msg = sprintf "Not found. CustomerId=%A" customerId
            Error (MissingRecord msg)  // return a Result
        
        // exactly one found        
        | [dbCustomer] ->
            dbCustomer
            |> toDomain
            |> Result.mapError InvalidRecord
        
        // more than one found?        
        | _ ->
            let msg = sprintf "Multiple records found for CustomerId=%A" customerId
            raise (DatabaseError msg)
    //<

    module WithHelper = 
        //>ConvertSingleDbRecord 
        let convertSingleDbRecord tableName idValue records toDomain =
            match records with
            // none found
            | [] -> 
                let msg = sprintf "Not found. Table=%s Id=%A" tableName idValue
                Error msg  // return a Result
        
            // exactly one found        
            | [dbRecord] ->
                dbRecord
                |> toDomain
                |> Ok   // return a Result
        
            // more than one found?        
            | _ ->
                let msg = sprintf "Multiple records found. Table=%s Id=%A" tableName idValue
                raise (DatabaseError msg)
        //<

        //>ReadOneCustomerWithHelper 
        let readOneCustomer (productionConnection:SqlConnection) (CustomerId customerId) =
            use cmd = new ReadOneCustomer(productionConnection)
            let tableName = "Customer" 

            let records = cmd.Execute(customerId = customerId) |> Seq.toList
            convertSingleDbRecord tableName customerId records toDomain
        //<

    // =================================
module ReadContact =

    //>ReadOneContactType
    type ReadOneContact = SqlCommandProvider<"""
        SELECT ContactId,IsEmail,IsPhone,EmailAddress,PhoneNumber
        FROM ContactInfo
        WHERE ContactId = @contactId
        """, CompileTimeConnectionString>
    //<

    module ContactId = 
        let create contactId = 
            if contactId > 0 then 
                Ok (ContactId contactId) 
            else 
                Error (sprintf "Invalid contactId %i" contactId)

    module EmailAddress = 
        let create str = 
            if String.IsNullOrEmpty(str) then 
                Error (sprintf "Email must not be blank")
            elif str.Contains("@") then 
                Ok (EmailAddress str) 
            else 
                Error (sprintf "EmailAddress must contain @")

    module PhoneNumber =
        let create str = 
            if String.IsNullOrEmpty(str) then 
                Error (sprintf "PhoneNumber must not be blank")
            else
                Ok (PhoneNumber str) 

    //>ofOption 
    module Result =
        /// Convert an Option into a Result
        let ofOption errorValue opt = 
            match opt with
            | Some v -> Ok v
            | None -> Error errorValue
    //<

    //>ContactToDomain
    let toDomain (dbRecord:ReadOneContact.Record) : Result<Contact,_> =
        result {
            let! contactId = 
                dbRecord.ContactId 
                |> ContactId.create

            let! contactInfo =
                if dbRecord.IsEmail then
                    result {
                        // get the primitive string which should not be NULL
                        let! emailAddressString =
                            dbRecord.EmailAddress 
                            |> Result.ofOption "Email expected to be non null"
                        // create the EmailAddress simple type
                        let! emailAddress = 
                            emailAddressString |> EmailAddress.create
                        // lift to the Email case of Contact Info
                        return (Email emailAddress)
                        } 
                else 
                    result {
                        // get the primitive string which should not be NULL
                        let! phoneNumberString = 
                            dbRecord.PhoneNumber
                            |> Result.ofOption "PhoneNumber expected to be non null"
                        // create the PhoneNumber simple type
                        let! phoneNumber = 
                            phoneNumberString |> PhoneNumber.create
                        // lift to the PhoneNumber case of Contact Info
                        return (Phone phoneNumber)
                        }

            let contact = {
                ContactId = contactId
                Info = contactInfo
                }
            return contact
            }
        //<


    exception DatabaseError of string


    let convertSingleDbRecord = ReadCustomer.WithHelper.convertSingleDbRecord

    //>ReadOneContactWithHelper 
    let readOneContact (productionConnection:SqlConnection) (ContactId contactId) =
        use cmd = new ReadOneContact(productionConnection)
        let tableName = "ContactInfo" 

        let records = cmd.Execute(contactId = contactId) |> Seq.toList
        convertSingleDbRecord tableName contactId records toDomain
    //<

module WriteDb = 
    open RelationalDomain

    open FSharp.Data
    open System.Data.SqlClient

    module ContactId = 
        let value (ContactId v) = v
    module EmailAddress = 
        let value (EmailAddress v) = v
    module PhoneNumber = 
        let value (PhoneNumber v) = v

    module SqlProgrammabilityProvider =

        //>SqlProgrammabilityProvider1
        type Db = SqlProgrammabilityProvider<CompileTimeConnectionString>
        //<

        //>SqlProgrammabilityProvider2
        let writeContact (productionConnection:SqlConnection) (contact:Contact) =
        
            // extract the primitive data from the domain object
            let contactId = contact.ContactId |> ContactId.value
            let isEmail,isPhone,emailAddressOpt,phoneNumberOpt = 
                match contact.Info with
                | Email emailAddress->
                    let emailAddressString = emailAddress |> EmailAddress.value
                    true,false,Some emailAddressString,None
                | Phone phoneNumber ->
                    let phoneNumberString = phoneNumber |> PhoneNumber.value
                    false,true,None,Some phoneNumberString  
        
            // create a new row
            let contactInfoTable = new Db.dbo.Tables.ContactInfo()
            let newRow = contactInfoTable.NewRow() 
            newRow.ContactId <- contactId
            newRow.IsEmail <- isEmail
            newRow.IsPhone <- isPhone
            // use optional types to map to NULL in the database
            newRow.EmailAddress <- emailAddressOpt  
            newRow.PhoneNumber <- phoneNumberOpt

            // add to table
            contactInfoTable.Rows.Add newRow

            // push changes to the database
            let recordsAffected = contactInfoTable.Update(productionConnection)
            recordsAffected 
        //<

    module HandWrittenDml =

        //>HandWrittenDml1
        type InsertContact = SqlCommandProvider<"""
            INSERT INTO ContactInfo
            VALUES (@ContactId,@IsEmail,@IsPhone,@EmailAddress,@PhoneNumber)
            """, CompileTimeConnectionString>
        //<

        //>HandWrittenDml2
        let writeContact (productionConnection:SqlConnection) (contact:Contact) =
        
            // extract the primitive data from the domain object
            let contactId = contact.ContactId |> ContactId.value
            let isEmail,isPhone,emailAddress,phoneNumber = 
                match contact.Info with
                | Email emailAddress->
                    let emailAddressString = emailAddress |> EmailAddress.value
                    true,false,emailAddressString,null 
                | Phone phoneNumber ->
                    let phoneNumberString = phoneNumber |> PhoneNumber.value
                    false,true,null,phoneNumberString 
        
            // write to the DB
            use cmd = new InsertContact(productionConnection)
            cmd.Execute(contactId,isEmail,isPhone,emailAddress,phoneNumber)
        //<

#r "System.Transactions"

module Transactions =
    open System.Data.SqlClient

    let markAsFullyPaid _ = notImplemented()
    let markPaymentCompleted _ = notImplemented()
    let markAsFullyPaidAndPaymentCompleted _ = notImplemented()
    let unmarkAsFullyPaid _ = notImplemented()
    let invoiceId = 1
    let paymentId = 1

    do 
        //>Transaction1
        let connection = new SqlConnection()
        let transaction = connection.BeginTransaction()
        
        // do two separate calls to the database
        // in the same transaction
        markAsFullyPaid connection invoiceId
        markPaymentCompleted connection paymentId 
        
        // completed
        transaction.Commit() 
        //<

    do 
        //>Transaction2
        let connection = new SqlConnection()
        // do one call to service
        markAsFullyPaidAndPaymentCompleted connection paymentId invoiceId
        //<

    do 
        let connection = new SqlConnection()
        
        //>Transaction3
        // do first call
        markAsFullyPaid connection invoiceId
        // do second call
        let result = markPaymentCompleted connection paymentId 

        // if second call fails, do compensating transaction
        match result with
        | Error err -> 
            // compensate for error
            unmarkAsFullyPaid connection invoiceId
        | Ok _ -> dotDotDot()
        //<
