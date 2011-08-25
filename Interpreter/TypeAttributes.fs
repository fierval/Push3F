﻿namespace push.types

module TypeAttributes =
    open System

    [<AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = false) >]
    type PushTypeAttribute(name:string)  =
        inherit Attribute()
        
        let mutable name = name
        
        member x.Name 
            with get() = name
            and set value = name <- value


    [<AttributeUsage(AttributeTargets.Method, AllowMultiple = false , Inherited = false) >]
    type PushOperationAttribute(name:string)  =
        inherit Attribute()
        
        let mutable name = name

        member x.Name 
            with get() = name
            and set value = name <- value

