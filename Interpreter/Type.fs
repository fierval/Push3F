namespace push.types

module Type =

    open System.Runtime.CompilerServices
    open TypeAttributes
    open System.Reflection
    open push.exceptions.PushException

    [<assembly: InternalsVisibleTo ("SimpleSearchTest")>]
    do 
        ()

    // loads types from the specified assembly
    // or from the current one by default
    let internal loadTypes assembly = 
        let asm : System.Reflection.Assembly =
            match assembly with
            | Some s -> Assembly.LoadFrom(s)
            | None -> Assembly.GetCallingAssembly()
    
        asm.GetTypes()

    // retrieves types with a specific annotation
    let internal getAnnotatedTypes (attribute : System.Type) (types: System.Type seq) =
        types |>
        Seq.filter (fun (t:System.Type) -> t.GetCustomAttributes(attribute, false).Length > 0)
        
    let internal extractName (mi : MethodInfo) =
        (mi.GetCustomAttributes(typeof<PushOperationAttribute>, false) |> Seq.head :?> PushOperationAttribute).Name

    // Class that wraps a PushType.
    // All push types should inherit from this class
    [<AbstractClass>]
    type PushTypeBase (name: string) =
        let name = name
        let mutable operationsContainer : Map<string, MethodInfo> = Map.empty

        // returns the name of the class
        member this.Name with get() = name

        abstract Operations : Map<string, MethodInfo> with get
        
        static member internal DiscoverByAssemblyAttribute(attribute : System.Type, assembly : string option) =
            assembly 
            |> loadTypes
            |> getAnnotatedTypes attribute


        //static function to run through the assemblies and discover new types
        static member internal DiscoverPushTypes(?assembly : string) =
            PushTypeBase.DiscoverByAssemblyAttribute(typeof<PushTypeAttribute>, assembly)

        //for each of the members, we can discover its operations.
        static member internal DiscoverPushOperations(ptype : #PushTypeBase) =
            let opAttributes = ptype.GetType().GetMethods() 
                                |> Seq.filter(
                                    fun m -> m.GetCustomAttributes(typeof<PushOperationAttribute>, false).Length = 1)    
            if Seq.length opAttributes = 0 then raise (PushException("no operations were found on the type"))
            Seq.fold (fun acc mi -> Map.add (extractName mi) mi acc) Map.empty opAttributes
 
        // implementation of the virtual Operations property
        default this.Operations =
            if operationsContainer.IsEmpty
            then
                operationsContainer <- PushTypeBase.DiscoverPushOperations(this)
            operationsContainer
             



