namespace push.types.stock

module StockTypesBool =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("BOOLEAN")>]
    type Bool =
        inherit PushTypeBase

        [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

        new () = {inherit PushTypeBase ()}
        new (b : bool) = {inherit PushTypeBase(b)}

        // custom parsing
        static member parse s =
            let result = ref Unchecked.defaultof<bool>
            if not (System.Boolean.TryParse(s, result)) 
            then 
                Unchecked.defaultof<PushTypeBase> 
            else 
                new Bool(!result) :> PushTypeBase

        override t.Parser 
            with get() = 
                ExtendedTypeParser(Bool.parse)

        override t.ToString() =
            base.ToString()

        [<PushOperation("=", Description = "TRUE if top two booleans are equal false otherwise")>]
        static member Eq() = 
            match processArgs2 typeof<Bool> with
            | [a1; a2] -> 
                pushResult(new Bool(a1.Raw<bool>() = a2.Raw<bool>()))
            | _ -> ()
    
        [<PushOperation("AND", Description = "Logical AND of the top two booleans")>]
        static member And() = 
            match processArgs2 typeof<Bool> with
            | [a1; a2] -> 
                pushResult(new Bool(a1.Raw<bool>() && a2.Raw<bool>()))
            | _ -> ()

        [<PushOperation("OR", Description = "Logical OR of the top two booleans")>]
        static member Or() = 
            match processArgs2 typeof<Bool> with
            | [a1; a2] -> 
                pushResult(new Bool(a1.Raw<bool>() || a2.Raw<bool>()))
            | _ -> ()

