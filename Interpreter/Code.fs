namespace push.types.stock

module StockTypesCode =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("CODE")>]
    type Code =
        inherit PushTypeBase

        [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

        override t.Operations 
            with get() = 
                if Unchecked.defaultof<Map<string, MethodInfo>> = Code.operations 
                    then 
                        Code.operations <- PushTypeBase.GetOperations(new Code())
                Code.operations

        new () = {inherit PushTypeBase ()}

        new (s : string) = {inherit PushTypeBase(s)}

        override t.ToString() =
          base.ToString()
