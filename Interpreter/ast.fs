namespace push.parser

module Ast = 
    open System.Reflection
    open System.Diagnostics
    open push.types.Type

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<DebuggerDisplay("{StructuredFormatDisplay}")>]
    type Push = 
        | Value of PushTypeBase
        | PushList of Push list
        | Operation of string * MethodInfo
        | Exec of string
        with 
            member private t.StructuredFormatDisplay = 
                match t with
                | Value i -> i.StructuredFormatDisplay
                | PushList l -> l :> obj
                | Operation (tp, mi) -> box ("\"" + mi.DeclaringType.Name + "." + tp + "\"")
                | Exec e -> box ("\"" + e + "\"")

            

