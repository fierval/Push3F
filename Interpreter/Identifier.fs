namespace push.types.stock

module StockTypesIdentifier =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("NAME")>]
    type Identifier =
        inherit PushTypeBase

        [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

        override t.Operations 
            with get() = 
                if Unchecked.defaultof<Map<string, MethodInfo>> = Identifier.operations 
                    then 
                        Identifier.operations <- PushTypeBase.GetOperations(new Identifier())
                Identifier.operations

        new () = {inherit PushTypeBase ()}

        new (s : string) = {inherit PushTypeBase(s)}

        override t.ToString() =
          base.ToString()

