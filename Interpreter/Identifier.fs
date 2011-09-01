namespace push.types.stock

module StockTypesIdentifier =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("NAME")>]
    type Identifier =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (s : string) = {inherit PushTypeBase(s)}

        override t.ToString() =
          base.ToString()

        [<PushOperation("NOOP")>]
        static member Noop() = ()