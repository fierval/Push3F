namespace push.types.stock

[<AutoOpen>]
module StockTypesFloat =
    open push.types

    open System.Reflection
    open System

    [<PushType("FLOAT")>]
    type Float =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (f : float) = {inherit PushTypeBase(f)}

        static member Me = Float()

        override t.ToString() =
            t.Raw<float>().ToString("F")

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
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Float(a1.Raw<float>() + a2.Raw<float>()))
            | _ -> ()
            
        [<PushOperation("*")>]
        static member Multiply() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Float(a1.Raw<float>() * a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("-")>]
        static member Subtract() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Float(a1.Raw<float>() - a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("/")>]
        static member Divide() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> 
                if a2.Raw<float>() = 0. 
                then 
                    pushResult a1
                    pushResult a2
                else pushResult(Float(a1.Raw<float>() / a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("%")>]
        static member Mod() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> 
                if a2.Raw<float>() = 0. 
                then 
                    pushResult a1
                    pushResult a2
                else
                    let quot = Math.Floor(a1.Raw<float>()) / Math.Floor(a2.Raw<float>())
                    let res =  a1.Raw<float>() - (float quot) * a2.Raw<float>()
                    pushResult(Float(res))
            | _ -> ()

        [<PushOperation("<")>]
        static member Lt() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<float>() < a2.Raw<float>()))
            | _ -> ()

        [<PushOperation(">")>]
        static member Gt() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<float>() > a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("=")>]
        static member Eq() =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<float>() = a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("FROMINTEGER", Description = "Pushes int64 converted into float onto FLOAT stack")>]
        static member fromFloat() =
            let top = processArgs1 Float.Me.MyType
            if top <> Unchecked.defaultof<PushTypeBase> 
            then pushResult (Float(float (top.Raw<int64>())))

        [<PushOperation("FROMBOOLEAN", Description = "Pushes 1 onto INTEGER stack if top boolean is true, 0 otherwise")>]
        static member fromBool() =
            let top = processArgs1 "BOOLEAN"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then pushResult (Float(if top.Raw<bool>() = true then 1. else 0.))

        [<PushOperation("COS", Description = "Pushes the cos of the top item")>]
        static member Cos() =
            let a1 = processArgs1 Float.Me.MyType
            if a1 = Unchecked.defaultof<PushTypeBase> then ()
            pushResult(Float(Math.Cos(a1.Raw<float>())))

        [<PushOperation("MAX")>]
        static member Max () =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Float (Math.Max(a1.Raw<float>(), a2.Raw<float>())))
            | _ -> ()

        [<PushOperation("MIN")>]
        static member Min () =
            match processArgs2 Float.Me.MyType with
            | [a1; a2] -> pushResult(Float (Math.Min(a1.Raw<float>(), a2.Raw<float>())))
            | _ -> ()
    
        [<PushOperation("RAND", Description = "Pushes a random float. Range is determined by MIN-RANDOM-FLOAT and MAX-RANDOM-FLOAT")>]
        static member Rand () =
            pushResult (Float(Float.Random.NextDouble()))

        [<PushOperation("SIN", Description = "Pushes the sin of the top item")>]
        static member Sin() =
            let a1 = processArgs1 Float.Me.MyType
            if a1 = Unchecked.defaultof<PushTypeBase> then ()
            pushResult(Float(Math.Sin(a1.Raw<float>())))

        [<PushOperation("TAN", Description = "Pushes the tangent of the top item")>]
        static member Tan() =
            let a1 = processArgs1 Float.Me.MyType
            if a1 = Unchecked.defaultof<PushTypeBase> then ()
            if Math.Cos(a1.Raw<float>()) = 0. then pushResult a1
            pushResult(Float(Math.Tan(a1.Raw<float>())))
        