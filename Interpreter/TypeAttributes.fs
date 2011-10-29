namespace push.types

open System

// new push types are decorated with this attribute
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = false) >]
type PushTypeAttribute(name : string)  =
    inherit Attribute()
        
    let mutable name = name
    let mutable description = System.String.Empty

    member x.Name 
        with get() = name
        and set value = name <- value

    member x.Description
        with get() = description
        and set value = description <- value

// push operations within a push type are decorated with this attribute
[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true , Inherited = false) >]
type PushOperationAttribute(name:string)  =
    inherit PushTypeAttribute(name)

    let mutable shouldPickAtRandom = true

    member x.ShouldPickAtRandom 
        with get() = shouldPickAtRandom
        and set value = shouldPickAtRandom <- value
            
// types that only contain generic operations are decorated with this attribute.
// it can be used to extend regular types by defining operations with GenericPushOperationAttribute
// these types do not have a stack associated with them, so they have no names.
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false , Inherited = false) >]
type GenericPushTypeAttribute () =
    inherit Attribute()

[<AttributeUsage(AttributeTargets.Method, AllowMultiple = true , Inherited = false) >]
type GenericPushOperationAttribute (name : string) =
    inherit PushOperationAttribute (name)

    let mutable appliesTo = [||]

    member x.AppliesTo 
        with get() : string array = appliesTo
        and set value = appliesTo <- value
