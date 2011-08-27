namespace push.types.stock

module StockTypesBool =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("BOOL")>]
    type Bool () =
        inherit PushTypeBase()

        new (b : bool) = Bool() 
                               then
                               PushTypeBase() |> ignore
