namespace push.types.stock

[<AutoOpen>]
module StockTypesLiteral =
    open push.types
    open push.parser
    open push.stack
    open push.types.stock
    open System.Reflection
    open System

    [<PushType("LITERAL", Description = "Double-quoted literals")>]
    type Literal =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()} 
        new (l : string) = {inherit PushTypeBase(l)} 

        static member internal Me = new Literal()

        override t.ToString() =
          base.ToString()

        // custom parsing.
        // in this case custom parsing is disabled.
        // Push will parse these values
        override t.Parser 
            with get() = 
                Unchecked.defaultof<ExtendedTypeParser>

        override t.isQuotable with get() = false

        [<PushOperation("=")>]
        static member Eq() =
            match processArgs2 Literal.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<string>() = a2.Raw<string>()))
            | _ -> ()

        [<PushOperation("RAND", Description = "Pushes a random literal on top of the literal stack")>]
        static member Rand() =
            pushResult (Literal (Name.GetRandomString 15))