(*
Common code used in all the examples
*)

// used for placeholders
type Undefined = exn  

// Will be replaced with ellipses in the book.
// Used in the example code to let the code type check without having to provide full details.
type DotDotDot = Undefined 
type PrivateDotDotDot = Undefined 
let dotDotDot() = failwith "undefined"

// Will be replaced with question marks in the book.
let question() = failwith "to be defined"



let notImplemented() = failwith "not implemented"

type Result<'Success,'Failure> = 
    | Ok of 'Success 
    | Error of 'Failure


module Result =
    let map f xR=
        match xR with
        | Ok x -> Ok (f x)
        | Error e -> Error e 

    let mapError f xR=
        match xR with
        | Ok x -> Ok x
        | Error e -> Error (f e)

    let apply fR xR = 
        match fR, xR with
        | Ok f, Ok x -> Ok (f x)
        | Error errs1, Ok _ -> Error errs1
        | Ok _, Error errs2 -> Error errs2
        | Error errs1, Error errs2 -> Error (errs1 @ errs2)

    let applyM fR xR = 
        match fR, xR with
        | Ok f, Ok x -> Ok (f x)
        | Error errs1, Ok _ -> Error errs1
        | Ok _, Error errs2 -> Error errs2
        | Error errs1, Error errs2 -> Error errs1 


    let bind f xR =
        match xR with
        | Ok x -> f x
        | Error e -> Error e 

    let bindOption f xOpt =
        match xOpt with
        | Some x -> f x |> map Some
        | None -> Ok None

    /// Convert an Option into a Result
    let ofOption errorValue opt = 
        match opt with
        | Some v -> Ok v
        | None -> Error errorValue

    /// Prepend a Result<item> to a Result<list>
    let prepend firstR restR = 
        match firstR, restR with
        | Ok first, Ok rest -> Ok (first::rest)
        | Error err1, Ok _ -> Error err1
        | Ok _, Error err2 -> Error err2
        | Error err1, Error _ -> Error err1 
    

    let sequence aListOfResults = 
        let initialValue = Ok [] // empty list inside Result
 
        // loop through the list in reverse order,
        // prepending each element to the initial value
        List.foldBack prepend aListOfResults initialValue



type ResultBuilder() =
    member this.Return(x) = Ok x
    member this.Bind(x,f) = Result.bind f x

let result = ResultBuilder()



[<RequireQualifiedAccess>]  // RequireQualifiedAccess forces the `Async.xxx` prefix to be used
module Async =

    /// Lift a function to Async
    let map f xA = 
        async { 
        let! x = xA
        return f x 
        }

    /// Lift a value to Async
    let retn x = 
        async.Return x


type AsyncResult<'success,'failure> = Async<Result<'success,'failure>>

module AsyncResult =

    /// Lift a function to AsyncResult
    let map f (x:AsyncResult<_,_>) : AsyncResult<_,_> =
        Async.map (Result.map f) x

    /// Lift a function to AsyncResult
    let mapError f (x:AsyncResult<_,_>) : AsyncResult<_,_> =
        Async.map (Result.mapError f) x

    /// Lift a value to AsyncResult
    let retn x : AsyncResult<_,_> = 
        x |> Result.Ok |> Async.retn

    /// Apply a monadic function to an AsyncResult value  
    let bind (f: 'a -> AsyncResult<'b,'c>) (xAsyncResult : AsyncResult<_, _>) :AsyncResult<_,_> = async {
        let! xResult = xAsyncResult 
        match xResult with
        | Ok x -> return! f x
        | Error err -> return (Error err)
        }

    /// Lift a Result into an AsyncResult
    let ofResult x : AsyncResult<_,_> = 
        x |> Async.retn

// ==================================
// AsyncResult computation expression
// ==================================

/// The `asyncResult` computation expression is available globally without qualification
[<AutoOpen>]
module AsyncResultComputationExpression = 

    type AsyncResultBuilder() = 
        member __.Return(x) = AsyncResult.retn x
        member __.Bind(x, f) = AsyncResult.bind f x

    let asyncResult = AsyncResultBuilder()