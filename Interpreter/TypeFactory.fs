namespace push.types

module TypeFactory =

    open TypesShared
    open TypeAttributes
    open System.Reflection
    open push.exceptions.PushExceptions
    open Type
    open push.stack.Stack

    // Class that wraps a PushTypeFactory. The core of all things
    // All push types should inherit from this class
    type PushTypeFactory () =

        static member internal CreatePushObject (t:System.Type) : #PushTypeBase * string =
            let ci = t.GetConstructor(Array.empty)
            let name = (t.GetCustomAttributes(typeof<PushTypeAttribute>, false).[0] :?> PushTypeAttribute).Name
            let pushObj = ci.Invoke(Array.empty)
            let typedObj = 
                match pushObj with
                | :? #PushTypeBase as pushO -> pushO
                | _ -> failwithf "Can't happen, but the type is not push-based"
            typedObj, name

        // discovers the types and maps them by name
        static member internal DiscoverByAssemblyAttribute(attribute : System.Type, assembly : string option) =
            assembly 
            |> loadTypes
            |> getAnnotatedTypes attribute
            |> Seq.fold (fun map t -> 
                            let pushObj, name = PushTypeFactory.CreatePushObject t
                            map |> Map.add name pushObj) Map.empty


        //static function to run through the assemblies and discover new types
        static member internal DiscoverPushTypes(?assembly : string) =
            PushTypeFactory.DiscoverByAssemblyAttribute(typeof<PushTypeAttribute>, assembly)

        static member internal CreatePushStacks map =
            stacksInit map
                          



