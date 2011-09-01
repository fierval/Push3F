namespace push.types.stock

module StockTypesBool =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("BOOL")>]
    type Bool =
        inherit PushTypeBase

        [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

        new () = {inherit PushTypeBase ()}
        new (b : bool) = {inherit PushTypeBase(b)}

        override t.ToString() =
            base.ToString()

        [<PushOperation("NOOP")>]
        static member Noop() = ()