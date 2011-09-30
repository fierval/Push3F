namespace push.types

[<AutoOpen>]
module Type =
    
    open System.Reflection
    open push.exceptions
    open System.Diagnostics
    open System

    [<DebuggerDisplay("Value = {Value}")>]
    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<AbstractClass>]
    type PushTypeBase () = 
        [<DefaultValue>] 
        val mutable private value : obj

        [<DefaultValue>]
        val mutable private myType : string

        [<DefaultValue>]
        static val mutable private rndSeed : int

        new (v) as this = PushTypeBase()
                            then this.value <- v

        member t.Value with get() = t.value

        static member RandomSeed 
            with get () = 
                if PushTypeBase.rndSeed = Unchecked.defaultof<int> 
                then 
                    PushTypeBase.rndSeed <- Random(int DateTime.UtcNow.Ticks).Next(0, System.Int32.MaxValue) 
                else
                    PushTypeBase.rndSeed <- Random(PushTypeBase.rndSeed).Next(0, System.Int32.MaxValue)
                PushTypeBase.rndSeed

        static member private GetMyType (me : #PushTypeBase) =
            if System.String.IsNullOrEmpty(me.myType)
            then
                me.myType <- (me.GetType().GetCustomAttributes(typeof<PushTypeAttribute>, false).[0] :?> PushTypeAttribute).Name
            me.myType   

        // if something more than default action is necessary
        // this should be overridden
        abstract member Eval : PushTypeBase
        default t.Eval = t
                
        abstract member MyType : string with get
        default t.MyType with get () = PushTypeBase.GetMyType(t)

        abstract member isQuotable : bool with get
        default t.isQuotable with get () = false

        member t.Raw<'a> () =
            match t.Value with
            | :? 'a as raw -> raw
            | _ -> failwithf "could not convert to the right type"

        abstract StructuredFormatDisplay : obj
        default t.StructuredFormatDisplay =
            box t.Value

        override t.ToString() =
            t.Value.ToString()

        abstract Parser : ExtendedTypeParser with get
    
    and     // override this delegate to parse extended types
        ExtendedTypeParser = delegate of string -> PushTypeBase
