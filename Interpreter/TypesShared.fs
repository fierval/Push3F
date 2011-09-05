﻿namespace push.types

module TypesShared =

    open System.Reflection
    open System.Runtime.CompilerServices
    open push.types.TypeAttributes
    open push.exceptions.PushExceptions

    [<assembly: InternalsVisibleTo ("InterpreterTests")>]
    do 
        ()


    // map extensions
    type Microsoft.FSharp.Collections.Map<'Key, 'Value when 'Key : comparison>  with
        member t.Replace (key, value) =
            t.Remove(key).Add(key, value)

        member t.Append (map) =
            ((t |> Map.toList) @ (map |> Map.toList)) |> Map.ofList    

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
        let opAttributes = ptype.GetType().GetMethods(BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static) 
                            |> Seq.filter(
                                fun m -> m.GetCustomAttributes(typeof<PushOperationAttribute>, false).Length = 1)    
        if Seq.length opAttributes = 0 then raise (PushException("no operations were found on the type. Make sure operations are declared static")) 
        else
            opAttributes |> Seq.fold (fun acc mi -> Map.add (extractName mi) mi acc) Map.empty

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
        nonGenericOps |> Map.map (fun key value -> value.Append(genericOps))

    
    //given a push object retrieve the name of its type
    let internal getObjectPushType pushObj = (pushObj.GetType().GetCustomAttributes(typeof<PushTypeAttribute>, true).[0] :?> PushTypeAttribute).Name