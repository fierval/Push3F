namespace push.types

[<AutoOpen>]
module TypesShared =

    open System.Reflection
    open System.Runtime.CompilerServices
    open push.types.TypeAttributes
    open push.exceptions.PushExceptions

    [<assembly: InternalsVisibleTo ("InterpreterTests")>]
    do 
        ()

    type State =
    | Quote = 0
    | Exec = 1

    // map extensions
    type Microsoft.FSharp.Collections.Map<'Key, 'Value when 'Key : comparison>  with
        member t.Replace (key, value) =
            t.Remove(key).Add(key, value)

        member t.Append (map) =
            ((t |> Map.toList) @ (map |> Map.toList)) |> Map.ofList    

        member t.KeyCollection = t |> Map.toList |> List.map (fun (key,value) -> key)

    let intersect (left : 'a list when 'a : comparison) (right : 'a list) =
        let cache = Set<'a>(right)
        left |> List.filter (fun e -> cache.Contains e) 

    let except (left : 'a list when 'a : comparison) (right : 'a list) =
        let cache = Set<'a>(right)
        left |> List.filter (fun e -> not (cache.Contains e))

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

    // allows for operation aliasing. Two operation names may mean the same thing        
    let internal addToOpsMap (mi : MethodInfo * string[]) map =
        snd mi |> Array.fold (fun map e -> map |> Map.add e (fst mi)) map

    //for each of the members, we can discover its operations.
    let internal getOperationsForType ptype attribute =
        let opAttributes = ptype.GetType().GetMethods(BindingFlags.NonPublic ||| BindingFlags.Public ||| BindingFlags.Static) //discover all relevant methods
                            |> Seq.map ( // create a seq of MethodInfo * Attribute []
                                fun mi -> mi, mi.GetCustomAttributes(attribute, false)) // optimization: only calling GetCustomAttributes() once
                            |> Seq.filter ( // filter the sequence to only have PushOperationAttribute - containing methods
                                fun e -> (snd e).Length > 0)
        if Seq.length opAttributes = 0 then raise (PushException("no operations were found on the type. Make sure operations are declared static")) 
        else
            opAttributes
            |> Seq.map ( // map to the sequence of MethodInfo * string [] (where string [] are operation aliases)
                fun e -> fst e, ((snd e) |> Array.map (fun e -> (e :?> PushOperationAttribute).Name)))
            |> Seq.fold ( // finally, convert to the map of Map<string, MethodInfo>
                fun acc mi -> addToOpsMap mi acc) Map.empty 

    // gets generic operations from the Ops type
    let internal getGenericOperations = 
        let tp = (Assembly.GetCallingAssembly().GetType("push.types.GenericOperations+Ops"))
        let opsObj = tp.GetConstructor(Array.empty).Invoke(Array.empty)
        getOperationsForType opsObj typeof<GenericPushOperationAttribute>

    let internal getNonGenericOperations (ptypes : Map<string, 'b>) =
        ptypes |> Map.map (fun typeName ptype -> (getOperationsForType ptype typeof<PushOperationAttribute>))

    // groups all operations into a map of: Map(typeName, Map(operationName, operation))
    let internal getOperations (ptypes : Map<string, 'b>) =
        let nonGenericOps = getNonGenericOperations ptypes
        let genericOps = 
            getGenericOperations 
            |> Map.map 
                (fun key mi -> 
                    let attr = (mi.GetCustomAttributes(typeof<GenericPushOperationAttribute>, false)).[0] :?> GenericPushOperationAttribute
                    mi, Set(attr.AppliesTo)
                    ) // this will store the filter next to the operation method info so we can later on apply it if necessary
        nonGenericOps 
        |> Map.map 
            (fun key value -> 
                let filterdGenerics = 
                    genericOps 
                    |> Map.filter
                        (fun k (entryMethod, filterSet) -> 
                            filterSet.IsEmpty || filterSet.Contains(key))
                    |> Map.map(fun k v -> fst v) // we don't need the filter anymore, drop it.
                
                value.Append(filterdGenerics))





