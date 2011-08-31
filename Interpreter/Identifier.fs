namespace push.types.stock

module StockTypesIdentifier =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("NAME")>]
    type Identifier =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}

        new (s : string) = {inherit PushTypeBase(s)}

        override t.ToString() =
          base.ToString()

