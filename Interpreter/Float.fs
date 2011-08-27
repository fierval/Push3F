namespace push.types.stock

module StockTypesFloat =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    [<PushType("FLOAT")>]
    type Float =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}

        new (f : float) = {inherit PushTypeBase(f)}

        [<PushOperation("+")>]
        static member Add() =
            match processArgs2 with
            | (Some a1, Some a2) -> pushResult(new Float(a1.Raw<float>() + a2.Raw<float>()))
            | _ -> ()
            
