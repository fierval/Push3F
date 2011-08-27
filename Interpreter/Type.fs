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
        
        let mutable operationsContainer : Map<string, MethodInfo> = Map.empty

        new (v) as this = PushTypeBase()
                            then this.value <- v

        member t.Value with get() = t.value

        //for each of the members, we can discover its operations.
        static member internal DiscoverPushOperations(ptype) =
            let opAttributes = ptype.GetType().GetMethods() 
                                |> Seq.filter(
                                    fun m -> m.GetCustomAttributes(typeof<PushOperationAttribute>, false).Length = 1)    
            if Seq.length opAttributes = 0 then raise (PushException("no operations were found on the type"))
            Seq.fold (fun acc mi -> Map.add (extractName mi) mi acc) Map.empty opAttributes

        // implementation of the virtual Operations property
        member this.Operations =
            if operationsContainer.IsEmpty
            then
                operationsContainer <- PushTypeBase.DiscoverPushOperations(this)
            operationsContainer
        
        member t.Raw<'a> () =
            match t.Value with
            | :? 'a as raw -> raw
            | _ -> failwithf "could not convert to the right type"

        abstract StructuredFormatDisplay : obj
        default t.StructuredFormatDisplay =
            box t.Value

