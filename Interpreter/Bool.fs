namespace push.types.stock

[<AutoOpen>]
module StockTypesBool =
    open push.types
    open System

    open System.Reflection

    [<PushType("BOOLEAN")>]
    type Bool =
        inherit PushTypeBase

        [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

        new () = {inherit PushTypeBase ()}
        new (b : bool) = {inherit PushTypeBase(b)}

        static member Me = Bool()

        // custom parsing
        static member parse s =
            let result = ref Unchecked.defaultof<bool>
            if not (System.Boolean.TryParse(s, result)) 
            then 
                Unchecked.defaultof<PushTypeBase> 
            else 
                Bool(!result) :> PushTypeBase

        override t.Parser 
            with get() = 
                ExtendedTypeParser(Bool.parse)

        override t.ToString() =
            base.ToString()

        [<PushOperation("=", Description = "TRUE if top two booleans are equal false otherwise")>]
        static member Eq() = 
            match processArgs2 Bool.Me.MyType with
            | [a1; a2] -> 
                pushResult(Bool(a1.Raw<bool>() = a2.Raw<bool>()))
            | _ -> ()
    
        [<PushOperation("AND", Description = "Logical AND of the top two booleans")>]
        static member And() = 
            match processArgs2 Bool.Me.MyType with
            | [a1; a2] -> 
                pushResult(Bool(a1.Raw<bool>() && a2.Raw<bool>()))
            | _ -> ()

        [<PushOperation("FROMFLOAT", Description = "Pushies FALSE if top of the FLOAT stack is 0.0. True otherwise")>]
        static member fromFloat() =
            let top = processArgs1 "FLOAT"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then
                match top.Raw<float>() with
                | 0.0 -> pushResult (Bool(false))
                | _ -> pushResult (Bool (true))

        [<PushOperation("FROMINTEGER", Description = "Pushies FALSE if top of the INTEGER stack is 0. True otherwise")>]
        static member fromInt() =
            let top = processArgs1 "INTEGER"
            if top <> Unchecked.defaultof<PushTypeBase> 
            then
                match top.Raw<int64>() with
                | 0L -> pushResult (Bool(false))
                | _ -> pushResult (Bool (true))


        [<PushOperation("NOT", Description = "Logical OR of the top of the stack")>]
        static member Not() = 
            let a = processArgs1 Bool.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> then () else pushResult(Bool(not (a.Raw<bool>())))

        [<PushOperation("OR", Description = "Logical OR of the top two booleans")>]
        static member Or() = 
            match processArgs2 Bool.Me.MyType with
            | [a1; a2] -> 
                pushResult(Bool(a1.Raw<bool>() || a2.Raw<bool>()))
            | _ -> ()

        [<PushOperation("RAND", Description = "Generates a random boolean")>]
        static member Rand() =
            let rnd = new Random(int DateTime.UtcNow.Ticks)
            let res = if rnd.NextDouble() > 0.5 then true else false
            pushResult(Bool(res))
