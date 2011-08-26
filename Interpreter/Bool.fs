namespace push.types.stock

module StockTypesBool =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("BOOL")>]
    type Bool (n : bool) =
        inherit PushTypeBase()

        let value = n

        member t.Value with get() = value

        //TODO: this should be duck-typed
        static member ProcessArgs n =
            let args = popArguments typeof<Bool> 2
            if not (args.Length = n) then (None, None)
            else
                let arg1, arg2 = List.head args :?> Bool, List.head (List.tail args) :?> Bool
                Some arg1, Some arg2
