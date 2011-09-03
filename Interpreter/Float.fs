namespace push.types.stock

module StockTypesFloat =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("FLOAT")>]
    type Float =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (f : float) = {inherit PushTypeBase(f)}

        override t.ToString() =
            base.ToString()

        // custom parsing
        static member parse s =
            let result = ref Unchecked.defaultof<float>
            if not (System.Double.TryParse(s, result)) 
            then 
                Unchecked.defaultof<PushTypeBase> 
            else 
                new Float(!result) :> PushTypeBase

        override t.Parser 
            with get() = 
                ExtendedTypeParser(Float.parse)

        [<PushOperation("+")>]
        static member Add() =
            match processArgs2 "FLOAT" with
            | [a1; a2] -> pushResult(new Float(a1.Raw<float>() + a2.Raw<float>()))
            | _ -> ()
            
        [<PushOperation("*")>]
        static member Multiply() =
            match processArgs2 "FLOAT" with
            | [a1; a2] -> pushResult(new Float(a1.Raw<float>() * a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("-")>]
        static member Subtract() =
            match processArgs2 "FLOAT" with
            | [a1; a2] -> pushResult(new Float(a1.Raw<float>() - a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("/")>]
        static member Divide() =
            match processArgs2 "FLOAT" with
            | [a1; a2] -> 
                if a1.Raw<float>() = 0. 
                then pushResult(new Float(System.Double.MinValue)) 
                else pushResult(new Float(a1.Raw<float>() - a2.Raw<float>()))
            | _ -> ()
