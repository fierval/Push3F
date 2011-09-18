namespace push.types.stock

[<AutoOpen>]
module StockTypesInteger =
    open push.types
    open System

    open System.Reflection

    [<PushType("INTEGER")>]
    type Integer  =
        inherit PushTypeBase

        new () = {inherit PushTypeBase() }
        new (n : int64) = {inherit PushTypeBase(n)}

        static member Me = Integer()

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
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Integer(a1.Raw<int64>() + a2.Raw<int64>()))
            | _ -> ()
            
        [<PushOperation("*")>]
        static member Multiply() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Integer(a1.Raw<int64>() * a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("-")>]
        static member Subtract() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Integer(a1.Raw<int64>() - a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("/")>]
        static member Divide() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> 
                if a1.Raw<int64>() = 0L
                then 
                    pushResult a1
                    pushResult a2
                else pushResult(Integer(a1.Raw<int64>() / a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("%")>]
        static member Mod() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> 
                if a2.Raw<int64>() = 0L
                then 
                    pushResult a1
                    pushResult a2
                else
                    pushResult(Integer(a1.Raw<int64>() % a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("<")>]
        static member Lt() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<int64>() < a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation(">")>]
        static member Gt() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<int64>() > a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("=")>]
        static member Eq() =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<int64>() = a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("FROMFLOAT", Description = "Pushes int64 converted into int64 onto INTEGER stack")>]
        static member fromint64() =
            let top = processArgs1 "FLOAT"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then pushResult (Integer(int64 (top.Raw<int64>())))

        [<PushOperation("FROMBOOLEAN", Description = "Pushes 1 onto INTEGER stack if top boolean is true, 0 otherwise")>]
        static member fromBool() =
            let top = processArgs1 "BOOLEAN"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then pushResult (Integer(if top.Raw<bool>() = true then 1L else 0L))

        [<PushOperation("MAX")>]
        static member Max () =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Integer (Math.Max(a1.Raw<int64>(), a2.Raw<int64>())))
            | _ -> ()

        [<PushOperation("MIN")>]
        static member Min () =
            match processArgs2 Integer.Me.MyType with
            | [a1; a2] -> pushResult(Integer (Math.Min(a1.Raw<int64>(), a2.Raw<int64>())))
            | _ -> ()
    
        [<PushOperation("RAND", Description = "Pushes a random int64. Range is determined by MIN-RANDOM-INTEGER and MAX-RANDOM-INTEGER")>]
        static member Rand () =
            let rnd = new Random(int DateTime.UtcNow.Ticks)
            let rslt = rnd.Next(0, Int32.MaxValue)
            pushResult (Integer(int64 rslt))

