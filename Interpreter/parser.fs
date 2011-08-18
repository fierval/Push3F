namespace push.parser

module Parser =

    open FParsec
    open Ast
    open ParserShared

    // identifier
    let isAsciiIdStart c =    isAsciiLetter c || c = '_'

    let isAsciiIdContinue c =
        isAsciiLetter c || isDigit c || c = '_' || c = '\''

    // push identifier is a regular identifer
    let commonIdentifier = identifier (IdentifierOptions(isAsciiIdStart = isAsciiIdStart, 
                                                            isAsciiIdContinue = isAsciiIdContinue))

    // is this a type?
    let pushType = 
        let findType t = 
            match t with
            | FindType stockTypes res -> str res
            | _ -> failFatally ("unknown type: " + t)

        commonIdentifier >>= findType
                    
                    
    let op : PushParser<string> = choice [str "*" ; str "+"]
    let pushIdentifier = commonIdentifier |>> Identifier

    // operation: identifier.op
    let pushOperation  = (tuple2 pushType (str "." >>. op)) |>> Operation
    
    // values of simple types
    let pushFloat = pfloat |>> Float
    let pushInt  = pint64 |>> Integer
    let pushTrue = returnStringCI "true" (Bool true) 
    let pushFalse = returnStringCI "false" (Bool false)

    let pushSimpleValue = choice [pushFloat; pushInt; pushTrue; pushFalse; pushIdentifier; pushOperation]

    // pushProgram must be defined now, so we could use it inside the pushList definition.
    // however, pushList definition is part of defining pushProgram. To solve this catch 22,
    // FParsec provides createParserForwardedToRef function.
    let pushProgram, pushProgramRef = createParserForwardedToRef()

    let commonListParser sOpen sClose pElement f =
        between sOpen sClose
            (ws >>. sepBy pElement ws |>> f)    
    
    let pushList = commonListParser openList closeList pushProgram PushList

    do pushProgramRef := choice [pushSimpleValue
                                 pushOperation
                                 pushList]

    let push = ws >>. pushProgram .>> ws .>> eof
    
    // UTF8 is the default, but it will detect UTF16 or UTF32 byte-order marks automatically
    let parsePushFile fileName encoding =
        runParserOnFile push () fileName System.Text.Encoding.UTF8

    let parsePushStream stream encoding =
        runParserOnStream push () "" stream System.Text.Encoding.UTF8
   