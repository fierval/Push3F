namespace push.types

module TypeFactory =

    open TypesShared
    open TypeAttributes
    open System.Reflection
    open push.exceptions.PushExceptions
    open Type
    open push.stack.Stack


    let createPushObject (t:System.Type) : #PushTypeBase * string =
        let ci = t.GetConstructor(Array.empty)
        let name = (t.GetCustomAttributes(typeof<PushTypeAttribute>, false).[0] :?> PushTypeAttribute).Name
        let pushObj = ci.Invoke(Array.empty)
        let typedObj = 
            match pushObj with
            | :? #PushTypeBase as pushO -> pushO
            | _ -> failwithf "Can't happen, but the type is not push-based"
        typedObj, name

    // discovers the types and maps them by name
    let discoverByAssemblyAttribute (attribute : System.Type) (assembly : string option) =
        assembly 
        |> loadTypes
        |> getAnnotatedTypes attribute
        |> Seq.fold (fun map t -> 
                        let pushObj, name = createPushObject t
                        map |> Map.add name pushObj) Map.empty


    //static function to run through the assemblies and discover new types
    let discoverPushTypes (assembly : string option) =
        discoverByAssemblyAttribute typeof<PushTypeAttribute> assembly

    // stack of currently implemented types
    let typeStacks (map : Map<string, #PushTypeBase>) : Map<string, Stack<#PushTypeBase>> = map |> Map.map (fun key o -> empty)

    let appendMaps map1 map2 = ((map1 |> Map.toList) @ (map2 |> Map.toList)) |> Map.ofList

    // keeps the actual stock types
    type StockTypes(? assembly : string) = 
        let mutable ptypes = discoverPushTypes assembly
        let mutable stacks = typeStacks ptypes

        member t.Stacks with get() = stacks
        member t.Types with get() = ptypes

        member t.appendStacksFromAssembly (assembly : string) =
            let newTypes = discoverPushTypes (Some assembly)
            ptypes <- appendMaps ptypes newTypes
            stacks <- typeStacks ptypes


