﻿namespace push.types

module TypesShared =

    open System.Reflection
    open System.Runtime.CompilerServices
    open push.types.TypeAttributes
    open push.exceptions.PushExceptions

    [<assembly: InternalsVisibleTo ("InterpreterTests")>]
    do 
        ()

    let internal appendMaps map1 map2 = ((map1 |> Map.toList) @ (map2 |> Map.toList)) |> Map.ofList

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


    //for each of the members, we can discover its operations.
    let internal getOperationsForType ptype =
        let opAttributes = ptype.GetType().GetMethods() 
                            |> Seq.filter(
                                fun m -> m.GetCustomAttributes(typeof<PushOperationAttribute>, false).Length = 1)    
        if Seq.length opAttributes = 0 then raise (PushException("no operations were found on the type")) 
        else
            let ops = opAttributes |> Seq.fold (fun acc mi -> Map.add (extractName mi) mi acc) Map.empty

            for op in ops do
                if not op.Value.IsStatic then raise (PushException("Operation must be declared static"))
            ops


    // gets generic operations from the Ops type
    let internal getGenericOperations = 
        let tp = (Assembly.GetCallingAssembly().GetType("push.types.GenericOperations+Ops"))
        let opsObj = tp.GetConstructor(Array.empty).Invoke(Array.empty)
        getOperationsForType opsObj

    let internal getNonGenericOperations (ptypes : Map<string, 'b>) =
        ptypes |> Map.map (fun typeName ptype -> (getOperationsForType ptype))

    // groups all operations into a map of: Map(typeName, Map(operationName, operation))
    let internal getOperations (ptypes : Map<string, 'b>) =
        let nonGenericOps = getNonGenericOperations ptypes
        let genericOps = getGenericOperations
        nonGenericOps |> Map.map (fun key value -> appendMaps value genericOps)