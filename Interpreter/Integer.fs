﻿namespace push.types.stock

module StockTypesInteger =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("INTEGER")>]
    type Integer (n : int64) =
        inherit PushTypeBase(n)

        //TODO: this should be duck-typed
        static member ProcessArgs n =
            let args = popArguments typeof<Integer> 2
            if not (args.Length = n) then (None, None)
            else
                let arg1, arg2 = List.head args :?> Integer, List.head (List.tail args) :?> Integer
                Some arg1, Some arg2

        [<PushOperation("+")>]
        static member Add() =
            match Integer.ProcessArgs 2 with
            | (Some a1, Some a2) -> pushResult(new Integer(a1.Raw<int64>() + a2.Raw<int64>()))
            | _ -> ()
            
