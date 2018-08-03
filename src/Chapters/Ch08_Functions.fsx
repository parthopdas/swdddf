//===================================
// Code snippets from chapter 8
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

module FunctionsAsThings = 

    //>FunctionsAsThings1 
    let plus3 x = x + 3            // plus3 : x:int -> int
    let times2 x = x * 2           // times2 : x:int -> int
    let square = (fun x -> x * x)  // square : x:int -> int
    let addThree = plus3           // addThree : (int -> int)
    //<

    //>FunctionsAsThings2 
    // listOfFunctions : (int -> int) list
    let listOfFunctions =          
      [addThree; times2; square]
    //<

    //>FunctionsAsThings3 
    for fn in listOfFunctions do
      let result = fn 100   // call the function
      printfn "If 100 is the input, the output is %i" result 

    // Result =>
    // If 100 is the input, the output is 103
    // If 100 is the input, the output is 200
    // If 100 is the input, the output is 10000
    //<

module LetDefinedFunction = 
    //>let
    // myString : string 
    let myString = "hello"
    //<
    //>letdefined
    // square : x:int -> int
    let square x = x * x
    //<

module ValueDefinedFunction = 
    //>valdefined
    // square : x:int -> int
    let square = (fun x -> x * x)
    //<
    
module FunctionsAsInput = 

    //>input1a
    let evalWith5ThenAdd2 fn = 
        fn(5) + 2     

    // evalWith5ThenAdd2 : fn:(int -> int) -> int
    //<

    //>input1b
    let add1 x = x + 1     // an int -> int function
    evalWith5ThenAdd2 add1 // fn(5) + 2 becomes add1(5) + 2
    //                     // so output is 8
    //<

    //>input1c
    let square x = x * x     // an int -> int function
    evalWith5ThenAdd2 square // fn(5) + 2 becomes square(5) + 2 
    //                       // so output is 27
    //<

module FunctionsAsOutput = 

    //>output0
    let add1 x = x + 1
    let add2 x = x + 2
    let add3 x = x + 3
    //<

    //>output1a
    let adderGenerator numberToAdd = 
        // return a lambda
        fun x -> numberToAdd + x 

    // val adderGenerator : 
    //    int -> (int -> int)
    //<

    do
        //>output1b
        // test
        let add1 = adderGenerator 1 
        add1 2     // result => 3

        let add100 = adderGenerator 100
        add100 2   // result => 102
        //<
        ()

module FunctionsAsOutput_v2 = 

    //>output2a
    let adderGenerator numberToAdd = 
        // define a nested inner function
        let innerFn x =
            numberToAdd + x 

        // return the inner function
        innerFn 
    //<

module Currying =

    //>Currying1
    // int -> int -> int
    let add x y = x + y
    //<

    //>Currying2
    // int -> (int -> int)
    let adderGenerator x = fun y -> x + y
    //<

module PartialApplication =

    //>PartialApplication1
    // sayGreeting: string -> string -> unit
    let sayGreeting greeting name = 
        printfn "%s %s" greeting name
    //<

    //>PartialApplication2
    // sayHello: string -> unit
    let sayHello = sayGreeting "Hello"

    // sayGoodbye: string -> unit
    let sayGoodbye = sayGreeting "Goodbye"
    //<

    //>PartialApplication3
    sayHello "Alex"
    // output: "Hello Alex"

    sayGoodbye "Alex"
    // output: "Goodbye Alex"
    //<

module TotalFunctions =

    (*>TotalFunctions1 
    let twelveDividedBy n = 
       match n with
       | 6 -> 2
       | 5 -> 2
       | 4 -> 3
       | 3 -> 4
       | 2 -> 6
       | 1 -> 12
       | 0 -> ???
    <*)

    module WithException =
        //>TotalFunctions2 
        let twelveDividedBy n = 
           match n with
           | 6 -> 2
           //...
           | 0 -> failwith "Can't divide by zero"
        //<

    module RestrictedInput =
        //>TotalFunctions3 
        type NonZeroInteger = 
           // Defined to be constrained to non-zero ints.
           // Add smart constructor, etc
           private NonZeroInteger of int

        /// Uses restricted input
        let twelveDividedBy (NonZeroInteger n) = 
           match n with
           | 6 -> 2
           //...
           // 0 can't be in the input
           // so doesn't need to be handled
        //<

    module ExtendedOutput =
        //>TotalFunctions4
        /// Uses extended output
        let twelveDividedBy n = 
           match n with
           | 6 -> Some 2 // valid
           | 5 -> Some 2 // valid
           | 4 -> Some 3 // valid
           //... 
           | 0 -> None   // undefined
        //<

module Pipe =

    //>pipe1
    let add1 x = x + 1     // an int -> int function
    let square x = x * x   // an int -> int function

    let add1ThenSquare x =   
        x |> add1 |> square 

    // test
    add1ThenSquare 5       // result is 36
    //<

    //>pipe2
    let isEven x = 
      (x % 2) = 0              // an int -> bool function

    let printBool x = 
      sprintf "value is %b" x  // a bool -> string function
    
    let isEvenThenPrint x =  
      x |> isEven |> printBool 

    // test
    isEvenThenPrint 2          // result is "value is true"
    //<

module CompositionChallenge = 

    //>Challenge1
    // a function that has an int as output
    let add1 x = x + 1

    // a function that has an Option<int> as input
    let printOption x = 
       match x with
       | Some i -> printfn "The int is %i" i
       | None -> printfn "No value"
    //<

    //>Challenge2
    5 |> add1 |> Some |> printOption
    //<


module Syntax = 
    //>SyntaxValues
    // single line comments use a double slash

    // The "let" keyword defines an (immutable) value
    let myInt = 5
    let myFloat = 3.14
    let myString = "hello"   // note that no types needed
    //<

    // ======== Lists ============
    //>lists
    let twoToFive = [2;3;4;5]        // Square brackets create a list with
                                     // semicolon delimiters.
    let oneToFive = 1 :: twoToFive   // :: creates list with new 1st element
    // The result is [1;2;3;4;5]
    let zeroToFive = [0;1] @ twoToFive   // @ concats two lists

    // IMPORTANT: commas are never used as delimiters, only semicolons!
    //<

    // ======== Functions ========
    //>SyntaxFunctions
    // The "let" keyword also defines a named function.
    let square x = x * x          // Note that no parens are used.

    // In F# returns are implicit -- no "return" needed. A function always
    // returns the result of the last expression.

    let add x y = x + y           // don't use add (x,y)! It means something
                                  // completely different.

    square 3                      // Run the function. No parens!
    add 2 3                       // Run the function. No parens!

    // to define a multiline function, just use indents. No semicolons needed.
    let evens list =
       let isEven x = x%2 = 0     // Define "isEven" as an inner ("nested") function
       List.filter isEven list    // List.filter is a library function
                                  // with two parameters: a boolean function
                                  // and a list to work on

    evens oneToFive               // Now run the function
    //<

    //>SyntaxRecords
    // Record types have named fields. Semicolons are separators.
    type Person = {First:string; Last:string}

    // To create a value of a record type, use similar syntax to the definition
    // and assign each field to a value
    let person1 = {First="john"; Last="Doe"}
    //<

    //>SyntaxUnions
    // Union types have choices. Vertical bars are separators.
    type Temp = 
        | DegreesC of float
        | DegreesF of int

    // To create a value of a union type, use the case tag as the constructor
    let tempInC = DegreesC 37.1
    let tempInF = DegreesF 98
    //<

    //>SyntaxComplex
    // Types can be combined recursively in complex ways.
    // E.g. here is a union type that contains a list of the same type:
    type Employee = 
      | Worker of Person
      | Manager of Employee list
    let jdoe = {First="John";Last="Doe"}
    let worker = Worker jdoe
    //<

    // ========= Printing =========
    //>SyntaxPrinting
    // The printf/printfn functions are similar to the
    // Console.Write/WriteLine functions in C#.
    printfn "Printing an int %i, a float %f, a bool %b" 1 2.0 true
    printfn "A string %s, and something generic %A" "hello" [1;2;3;4]

    // all complex types have pretty printing built in (using %A)
    printfn "Person=%A,\nTemp=%A,\nEmployee=%A" 
             person1 tempInC worker
    //<





    // ======== Pattern Matching ========
    // Match..with.. is a supercharged case/switch statement.
    let simplePatternMatch =
       let x = "a"
       match x with
        | "a" -> printfn "x is a"
        | "b" -> printfn "x is b"
        | _ -> printfn "x is something else"   // underscore matches anything

    // Some(..) and None are roughly analogous to Nullable wrappers
    let validValue = Some(99)
    let invalidValue = None

    // In this example, match..with matches the "Some" and the "None",
    // and also unpacks the value in the "Some" at the same time.
    let optionPatternMatch input =
       match input with
        | Some i -> printfn "input is an int=%d" i
        | None -> printfn "input is missing"

    optionPatternMatch validValue
    optionPatternMatch invalidValue