namespace push.parser

module ParserShared =

    open FParsec

    open push.types
    open push.types.stock

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

    let internal isAllowedCharNoDot c = 
        let num = System.Char.ConvertToUtf32(c.ToString(), 0)
        if c = '.' || c = '(' || c= ')' then false
        else 
            isAsciiLetter c || isDigit c || c = '_' || System.Char.IsPunctuation(c) || (num >= 33 && num <= 64)

    let isDot c = if c = '.' then true else false

    let isAllowedChar c = isDot c || isAllowedCharNoDot c

    let internal stringTokenNoDot : PushParser<string> = manySatisfy isAllowedCharNoDot
    let internal stringToken : PushParser<string> = manySatisfy isAllowedChar 
      
    let (|FindType|_|) t = 
        match stockTypes.Types.TryFind(t) with
        | Some s -> Some t
        | None -> None

    let (|FindOperation|_|) (tp : string) op =
        stockTypes.Operations.[tp].TryFind(op)
     
    let createIdentifier ident = new Name(ident) :> PushTypeBase

    let createValue (value : #PushTypeBase) =
        fun stream ->
            let mutable reply = new Reply<Push>()
            if Unchecked.defaultof<#PushTypeBase> = value
            then 
                reply.Status <- Error
                reply.Error <- messageError("Delegate parser returned null")
            else
                reply.Status <- Ok
                reply.Result <- Value(value)
            reply    

    let openList : PushParser<string> = str "(" .>> ws
    let closeList : PushParser<string> = ws >>. str ")"

    let validIdentifier t =
        fun stream ->
            let mutable reply = new Reply<string>()
            match t with
            | FindType res -> 
                reply.Status <- Error
                reply.Error <- messageError("Reserved keyword " + t)
            | _ -> 
                reply.Status <- Ok
                reply.Result <- t
            reply
        
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
            let mutable reply = new Reply<string * MethodInfo>()
            match op with
            | FindOperation tp res -> 
                reply.Status <- Ok
                reply.Result <- tp, res
            | _ -> 
                reply.Status <- Error
                reply.Error <- messageError("Unknown operation: " + tp)
            reply

    // takes the map of types, returns the parsers for these types
    let discoverParsers =
        stockTypes.Types
        |> Map.map (fun key value -> 
            value.GetType().GetProperties(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance)
            |> Array.find (fun p -> p.PropertyType = typeof<ExtendedTypeParser>))
    
    let internal pushExtended (dlgt : ExtendedTypeParser) = attempt (stringToken >>= (dlgt.Invoke >> createValue))

    // dynamically create the list of simple type parsers
    // also, filter out types that do not implement a parser
    let pushSimpleTypes =
        let parsers = 
            discoverParsers 
            |> Map.fold(fun lst key value -> value.GetValue(stockTypes.Types.[key], null) :?> ExtendedTypeParser :: lst) List.empty
            |> List.filter(fun callback -> not (callback = Unchecked.defaultof<ExtendedTypeParser>))
            |> List.map(fun callback -> pushExtended callback)
        choice parsers