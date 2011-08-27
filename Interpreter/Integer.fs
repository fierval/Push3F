namespace push.types.stock

module StockTypesInteger =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("INTEGER")>]
    type Integer  =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}

        new (n : int64) = {inherit PushTypeBase(n)}

        [<PushOperation("+")>]
        static member Add() =
            match processArgs2 with
            | (Some a1, Some a2) -> pushResult(new Integer(a1.Raw<int64>() + a2.Raw<int64>()))
            | _ -> ()
            
