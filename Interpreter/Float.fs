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

        [<PushOperation("FROMINTEGER", Description = "Pushes int64 converted into float onto FLOAT stack")>]
        static member fromFloat() =
            let top = processArgs1 "FLOAT"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then pushResult (new Float(float (top.Raw<int64>())))

        [<PushOperation("FROMBOOLEAN", Description = "Pushes 1 onto INTEGER stack if top boolean is true, 0 otherwise")>]
        static member fromBool() =
            let top = processArgs1 "BOOLEAN"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then pushResult (new Float(if top.Raw<bool>() = true then 1. else 0.))
