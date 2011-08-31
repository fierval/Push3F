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

        override t.Operations 
            with get() = 
                if Unchecked.defaultof<Map<string, MethodInfo>> = Bool.operations 
                    then 
                        Bool.operations <- PushTypeBase.GetOperations(new Bool())
                Bool.operations

        new () = {inherit PushTypeBase ()}

        new (b : bool) = {inherit PushTypeBase(b)}

        override t.ToString() =
            base.ToString()
