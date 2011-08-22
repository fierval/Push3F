namespace push.types

module Type =

    open System.Runtime.CompilerServices
    open TypeAttributes
    open System.Reflection

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
        
    [<AbstractClass>]
    type PushTypeBase (name: string, operations : string list) =
        let name = name
        let operations = operations

        // returns the name of the class
        member this.Name with get() = name

        static member internal DiscoverByAssemblyAttribute(attribute : System.Type, assembly : string option) =
            assembly 
            |> loadTypes
            |> getAnnotatedTypes attribute


        //static function to run through the assemblies and discover new types
        static member internal DiscoverPushTypes(?assembly : string) =
            PushTypeBase.DiscoverByAssemblyAttribute(typeof<PushTypeAttribute>, assembly)

        //for each of the members, we can discover its operations.
        //TODO: Implement DiscoverMyOwnOperations to filter out operations belonging to this class only
        static member internal DiscoverPushOperations(?assembly : string) =
            PushTypeBase.DiscoverByAssemblyAttribute(typeof<PushOperationAttribute>, assembly)
             
    and PushOperationBase (name: string) =
        let name = name

        member this.Name with get() = name




