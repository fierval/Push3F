namespace push.types

module TypesShared =

    open System.Reflection
    open System.Runtime.CompilerServices
    open push.types.TypeAttributes

    [<assembly: InternalsVisibleTo ("InterpreterTests")>]
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


