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
        inherit Attribute()
        
        let mutable name = name
        let mutable description = System.String.Empty

        member x.Description 
            with get () = description 
            and set value = description <- value

        member x.Name 
            with get() = name
            and set value = name <- value

