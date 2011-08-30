namespace push.parser

module Ast = 
    open System.Reflection
    open System.Diagnostics
    open push.types.Type

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<DebuggerDisplay("{StructuredFormatDisplay}")>]
    type Push = 
        Code of string
        | Value of PushTypeBase
        | PushList of Push list
        | Operation of MethodInfo
        | Exec of string
        | Identifier of string

        with 
            member private t.StructuredFormatDisplay = 
                match t with
                | Code c -> box ("\"" + c + "\"")
                | Value i -> i.StructuredFormatDisplay
                | PushList l -> l :> obj
                | Operation o -> box ("\"" + o.DeclaringType.Name + "." + o.Name + "\"")
                | Exec e -> box ("\"" + e + "\"")
                | Identifier i -> box ("\"" + i + "\"")

            

