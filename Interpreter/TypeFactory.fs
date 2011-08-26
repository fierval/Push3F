namespace push.types

module TypeFactory =

    open TypesShared
    open TypeAttributes
    open System.Reflection
    open push.exceptions.PushExceptions
    open Type
    open push.stack.Stack


    let internal createPushObject (t:System.Type) : #PushTypeBase * string =
        let ci = t.GetConstructor(Array.empty)
        let name = (t.GetCustomAttributes(typeof<PushTypeAttribute>, false).[0] :?> PushTypeAttribute).Name
        let pushObj = ci.Invoke(Array.empty)
        let typedObj = 
            match pushObj with
            | :? #PushTypeBase as pushO -> pushO
            | _ -> failwithf "Can't happen, but the type is not push-based"
        typedObj, name

    // discovers the types and maps them by name
    let internal discoverByAssemblyAttribute (attribute : System.Type) (assembly : string option) =
        assembly 
        |> loadTypes
        |> getAnnotatedTypes attribute
        |> Seq.fold (fun map t -> 
                        let pushObj, name = createPushObject t
                        map |> Map.add name pushObj) Map.empty


    //static function to run through the assemblies and discover new types
    let internal discoverPushTypes (assembly : string option) =
        discoverByAssemblyAttribute typeof<PushTypeAttribute> assembly

    // stack of currently implemented types
    let internal typeStacks (map : Map<string, #PushTypeBase>) : Map<string, Stack<#PushTypeBase>> = map |> Map.fold (fun state key o -> Map.add (o.GetType().Name) empty state) Map.empty

    let internal appendMaps map1 map2 = ((map1 |> Map.toList) @ (map2 |> Map.toList)) |> Map.ofList

    // keeps the actual stock types.
    // internal to this module only, so nobody externally
    // can instantiate it.
    type internal StockTypes(? assembly : string) = 
        let mutable ptypes = discoverPushTypes assembly
        let mutable stacks = typeStacks ptypes

        member t.Stacks with get() = stacks
        member t.Types with get() = ptypes

        member t.appendStacksFromAssembly (assembly : string) =
            let newTypes = discoverPushTypes (Some assembly)
            ptypes <- appendMaps ptypes newTypes
            stacks <- typeStacks ptypes

        // retrieves arguments from the appropriate stack
        member t.popArguments (sysType : System.Type) n =
            let key = sysType.Name
            if not (stacks.ContainsKey(key)) then List.empty
            else
                let stack = stacks.[key]
                if stack.length < n then List.empty
                else
                    stacks <- stacks.Remove(key)
                    let result, leftOver = popMany 2 stack 
                    stacks <- stacks.Add(key, leftOver)
                    result

        member t.pushResult resObj =
            let key = resObj.GetType().Name
            let stack = stacks.[key]
            stacks <- stacks.Remove(key)
            stacks <- stacks.Add(key, push resObj stack)

    let internal stockTypes = new StockTypes()         

    let appendStacksFromAssembly assembly = stockTypes.appendStacksFromAssembly assembly
    let popArguments sysType n = stockTypes.popArguments sysType n
    let pushResult resObj = stockTypes.pushResult resObj
