namespace push.types.stock

module StockTypesCode =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("CODE")>]
    type Code =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (s : string) = {inherit PushTypeBase(s)}

        override t.ToString() =
          base.ToString()

        [<PushOperation("NOOP")>]
        static member Noop() = ()