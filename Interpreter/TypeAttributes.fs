namespace push.types

module TypeAttributes =
    open System

    [<AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = false) >]
    type PushTypeAttribute(name:string)  =
        inherit Attribute()
        
        let mutable name = name
        let mutable description = System.String.Empty

        member x.Name 
            with get() = name
            and set value = name <- value

        member x.Description
            with get() = description
            and set value = description <- value

    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = true , Inherited = false) >]
    type PushOperationAttribute(name:string)  =
        inherit PushTypeAttribute(name)
        
    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = true , Inherited = false) >]
    type GenericPushOperationAttribute (name : string) =
        inherit PushOperationAttribute (name)

        let mutable appliesTo = [||]

        member x.AppliesTo 
            with get() : string array = appliesTo
            and set value = appliesTo <- value
