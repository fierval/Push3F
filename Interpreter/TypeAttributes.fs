namespace push.types

module TypeAttributes =
    open System

    [<AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = false) >]
    type PushTypeAttribute(name:string)  =
        inherit Attribute()
        
        let mutable name = name
        
        member x.Name 
            with get() = name
            and set value = name <- value


    [<AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = false) >]
    type PushOperationAttribute(name:string, ptype:string)  =
        inherit Attribute()
        
        let mutable name = name
        let mutable ptype = ptype

        member x.Name 
            with get() = name
            and set value = name <- value

        member x.PushType
            with get() = ptype
            and set value = ptype <- value
