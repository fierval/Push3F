namespace push.types

module Type =
    
    open System.Reflection
    open push.exceptions.PushExceptions
    open TypeAttributes
    open TypesShared
    open System.Diagnostics

    [<DebuggerDisplay("Value = {Value}")>]
    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<AbstractClass>]
    type PushTypeBase () = 
        [<DefaultValue>] 
        val mutable private value : obj
        
        new (v) as this = PushTypeBase()
                            then this.value <- v

        member t.Value with get() = t.value

        member t.Raw<'a> () =
            match t.Value with
            | :? 'a as raw -> raw
            | _ -> failwithf "could not convert to the right type"

        abstract StructuredFormatDisplay : obj
        default t.StructuredFormatDisplay =
            box t.Value

        override t.ToString() =
            t.Value.ToString()
