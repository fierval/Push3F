namespace push.parser

module ParserShared =

    open FParsec

    open Ast
    open push.types.TypeFactory
    open push.types.Type
    open push.types.stock.StockTypesInteger
    open push.types.stock.StockTypesBool
    open push.types.stock.StockTypesFloat

    open System.Reflection

    type PushParser<'a> = Parser<'a, unit>

    let ws = spaces
    let str s = pstring s

    let nodot : PushParser<unit> = notFollowedByString "."

    let stringLiteral : PushParser<string> =
        let escape =  anyOf "\"\\/bfnrt"
                      |>> function
                          | 'b' -> "\b"
                          | 'f' -> "\u000C"
                          | 'n' -> "\n"
                          | 'r' -> "\r"
                          | 't' -> "\t"
                          | c   -> string c // every other char is mapped to itself

        let unicodeEscape =
            str "u" >>. pipe4 hex hex hex hex (fun h3 h2 h1 h0 ->
                let hex2int c = (int c &&& 15) + (int c >>> 6)*9 // hex char to int
                (hex2int h3)*4096 + (hex2int h2)*256 + (hex2int h1)*16 + hex2int h0
                |> char |> string
            )

        between (str "\"") (str "\"")
                (stringsSepBy (manySatisfy (fun c -> c <> '"' && c <> '\\'))
                              (str "\\" >>. (escape <|> unicodeEscape)))
  
    let internal isAsciiIdStart c =    isAsciiLetter c || c = '_'

    let internal isAsciiIdContinue c =
        isAsciiLetter c || isDigit c || c = '_' || c = '\''

    // push identifier is a regular identifer
    let internal commonIdentifier : PushParser<string> = identifier (IdentifierOptions(isAsciiIdStart = isAsciiIdStart, 
                                                                        isAsciiIdContinue = isAsciiIdContinue))

    let internal isAllowedChar c = isAsciiLetter c || isDigit c || c = '_'

    let internal stringToken = stringLiteral <|> identifier (IdentifierOptions(isAsciiIdStart = isAllowedChar , 
                                                                isAsciiIdContinue = isAllowedChar))
  
    let (|FindType|_|) t = 
        match stockTypes.Types.TryFind(t) with
        | Some s -> Some t
        | None -> None

    let (|FindOperation|_|) (tp : string) op =
        let pushType = stockTypes.Types.[tp]
        PushTypeBase.GetOperations(pushType).TryFind(op)
     
    let createInteger n = new Integer(n) :> PushTypeBase
    let createFloat f = new Float(f) :> PushTypeBase
    let createBool b = new Bool(b) :> PushTypeBase
    
    let createOperation (tp, op) = stockTypes.Types.[tp], PushTypeBase.GetOperations(stockTypes.Types.[tp]).[op]
            
    let returnStringCI s x = pstringCI s >>% x

    let openList : PushParser<string> = str "(" .>> ws
    let closeList : PushParser<string> = ws >>. str ")"

    let findType t = 
        fun stream ->
            let mutable reply = new Reply<string>()
            match t with
            | FindType res -> 
                reply.Status <- Ok
                reply.Result <- res
            | _ -> 
                reply.Status <- Error
                reply.Error <- messageError("Unknown type: " + t)
            reply

    let findOp (tp, op) =
        fun stream ->
            let mutable reply = new Reply<MethodInfo>()
            match op with
            | FindOperation tp res -> 
                reply.Status <- Ok
                reply.Result <- res
            | _ -> 
                reply.Status <- Error
                reply.Error <- messageError("Unknown operation: " + tp)
            reply
