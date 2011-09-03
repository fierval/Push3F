namespace push.types.stock

module StockTypesInteger =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("INTEGER")>]
    type Integer  =
        inherit PushTypeBase

        new () = {inherit PushTypeBase() }
        new (n : int64) = {inherit PushTypeBase(n)}

        // custom parsing
        static member parse s =
            let result = ref Unchecked.defaultof<int64>
            if not (System.Int64.TryParse(s, result)) 
            then 
                Unchecked.defaultof<PushTypeBase> 
            else 
                new Integer(!result) :> PushTypeBase

        override t.Parser 
            with get() = 
                ExtendedTypeParser(Integer.parse)
        
        override t.ToString() =
            base.ToString()

        [<PushOperation("+")>]
        static member Add() =
            match processArgs2 typeof<Integer> with
            | [a1; a2] -> pushResult(new Integer(a1.Raw<int64>() + a2.Raw<int64>()))
            | _ -> ()
            
        [<PushOperation("*")>]
        static member Multiply() =
            match processArgs2 typeof<Integer> with
            | [a1; a2] -> pushResult(new Integer(a1.Raw<int64>() * a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("-")>]
        static member Subtract() =
            match processArgs2 typeof<Integer> with
            | [a1; a2] -> pushResult(new Integer(a1.Raw<int64>() - a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("/")>]
        static member Divide() =
            match processArgs2 typeof<Integer> with
            | [a1; a2] -> 
                if a1.Raw<int64>() = 0L 
                then pushResult(new Integer(System.Int64.MinValue)) 
                else pushResult(new Integer(a1.Raw<int64>() - a2.Raw<int64>()))
            | _ -> ()
