namespace push.types.stock

module StockTypes =
    open push.types.Type
    open push.types.TypeAttributes

    [<PushType("INTEGER")>]
    type Integer () =

        [<PushOperation("+")>]
        static member Add() =
            let args = Integer.GetArgsForOperation(2)
            if args.Length < 2 then ignore else Integer.Return(List.head args + List.head List.tail args)
