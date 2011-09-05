namespace push.types.stock

module StockTypesCode =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory
    open push.parser.Ast

    open System.Reflection

    [<PushType("CODE")>]
    type Code =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (p : Push) = {inherit PushTypeBase(p)}

        override t.ToString() =
          base.ToString()

        // custom parsing.
        // in this case custom parsing is disabled.
        // Push will parse these values
        override t.Parser 
            with get() = 
                Unchecked.defaultof<ExtendedTypeParser>

        [<PushOperation("NOOP")>]
        static member Noop() = ()