namespace push.types

module TypeFactory =

    open TypesShared
    open TypeAttributes
    open System.Reflection
    open push.exceptions.PushExceptions
    open Type
    open push.stack.Stack
    open push.exceptions.PushExceptions

    let internal createPushObject (t:System.Type) : #PushTypeBase * string =
        let ci = t.GetConstructor(Array.empty)
        let pushObj = ci.Invoke(Array.empty)
        let typedObj = 
            match pushObj with
            | :? #PushTypeBase as pushO -> pushO
            | _ -> raise  (PushException("The type needs to dervie from PushTypeBase: " + t.Name))
        typedObj, typedObj.MyType

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
    let internal typeStacks (map : Map<string, #PushTypeBase>) : Map<string, Stack<#PushTypeBase>> = 
        map |> Map.map (fun key o -> empty)

    // keeps the actual stock types.
    // internal to this module only, so nobody externally
    // can instantiate it.
    type internal StockTypes(? assembly : string) = 
        let mutable ptypes = discoverPushTypes assembly
        // this is a test hook. When adding types, it is good to restore
        // the state to the original one
        let mutable origPtypes = ptypes
        let mutable stacks = typeStacks ptypes
        let mutable operations = getOperations ptypes
        let mutable bindings : Map<string, PushTypeBase> = Map.empty

        // stores the stacks currently in use
        member t.Stacks 
            with get() = stacks
            and set value = stacks <- value

        // stores stock and extended types
        member t.Types 
            with get() = ptypes

        // stores operations
        member t.Operations 
            with get() = operations

        // stores NAME bindings
        member t.Bindings 
            with get() = bindings
            and set value = bindings <- value

        // appends types from the specified assembly
        member t.appendStacksFromAssembly (assembly : string) =
            let newTypes = discoverPushTypes (Some assembly)
            ptypes <- ptypes.Append(newTypes)
            stacks <- typeStacks ptypes
            operations <- getOperations ptypes

        // retrieves arguments from the appropriate stack
        member t.popArguments key n =
            if not (stacks.ContainsKey(key)) then List.empty
            else
                let stack = stacks.[key]
                if stack.length < n then List.empty
                else
                    let result, leftOver = popManyReverse n stack 
                    stacks <- stacks.Replace(key, leftOver)
                    result

        member t.pushResult (resObj : PushTypeBase) =
            let key = resObj.MyType
            let stack = stacks.[key]
            stacks <- stacks.Replace(key, push resObj stack)

        // good for test to clean up the stacks
        member t.cleanAllStacks() =
            ptypes <- origPtypes
            stacks <- typeStacks ptypes
            operations <- getOperations ptypes


    let internal stockTypes = new StockTypes()         

    let appendStacksFromAssembly assembly = stockTypes.appendStacksFromAssembly assembly
    let popArguments sysType n = stockTypes.popArguments sysType n
    let pushResult resObj = stockTypes.pushResult resObj

    let processArgs2 sysType =
        let args = popArguments sysType 2
        if not (args.Length = 2) then []
        else
            let arg1, arg2 = List.head args , List.head (List.tail args)
            [arg1; arg2]
        
    let processArgs1 sysType = 
        let args = popArguments sysType 1
        if not (args.Length = 1) then Unchecked.defaultof<PushTypeBase>
        else
            let arg = List.head args
            arg


    // given the MethodInfo of the operation, execute it.
    let execOperation pushType (mi : MethodInfo) =
        // if this is an operation, requiring type parameter
        if mi.GetParameters().Length > 0
        then
            mi.Invoke(null, [|pushType|]) |> ignore
        else
            mi.Invoke(null, Array.empty) |> ignore
            
    // given the push type name and the operation name, 
    // execute the operation
    let internal exec typeName operation =
        execOperation typeName stockTypes.Operations.[typeName].[operation]

    let peek stackName = 
        let stack = stockTypes.Stacks.[stackName]
        peek stack