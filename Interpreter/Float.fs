namespace push.types.stock

module StockTypesFloat =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("Float")>]
    type Float (n : float) =
        inherit PushTypeBase()

        //TODO: this should be duck-typed
        static member ProcessArgs n =
            let args = popArguments typeof<Float> 2
            if not (args.Length = n) then (None, None)
            else
                let arg1, arg2 = List.head args :?> Float, List.head (List.tail args) :?> Float
                Some arg1, Some arg2

        [<PushOperation("+")>]
        static member Add() =
            match Float.ProcessArgs 2 with
            | (Some a1, Some a2) -> pushResult(new Float(a1.Raw<float>() + a2.Raw<float>()))
            | _ -> ()
            
