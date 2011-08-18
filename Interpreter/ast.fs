namespace push.parser

module Ast = 

    let stockTypes = ["BOOLEAN"; "CODE"; "EXEC";  "FLOAT"; "INTEGER"; "NAME"]

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    type Push = 
        Code of string
        | Integer of int64
        | Bool of bool
        | Float of float
        | PushList of Push list
        | Operation of string * string
        | Exec of string
        | Name of Map<string, Push>
        | Identifier of string

        with 
            member private t.StructuredFormatDisplay = 
                match t with
                | Code c -> box ("\"" + c + "\"")
                | Integer i -> box i
                | Bool b -> box b
                | Float f -> box f
                | PushList l -> l :> obj
                | Operation (t, o) -> box ("\"" + t + "." + o + "\"")
                | Exec e -> box ("\"" + e + "\"")
                | Name n -> Map.toList n :> obj
                | Identifier i -> box ("\"" + i + "\"")

            

