namespace push.types.stock

module StockTypes =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("INTEGER")>]
    type Integer (n : int) =
        inherit PushTypeBase()

        let value = n

        member t.Value with get() = value

        //TODO: this should be duck-typed
        static member ProcessArgs n =
            let args = stockTypes.popArguments typeof<Integer> 2
            if not (args.Length = n) then (None, None)
            else
                let arg1, arg2 = List.head args :?> Integer, List.head (List.tail args) :?> Integer
                Some arg1.Value, Some arg2.Value

        [<PushOperation("+")>]
        static member Add() =
            match Integer.ProcessArgs 2 with
            | (Some a1, Some a2) -> stockTypes.pushResult(new Integer(a1 + a2))
            | _ -> ()
            
