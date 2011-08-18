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
    let pushIdentifier = identifier (IdentifierOptions(isAsciiIdStart = isAsciiIdStart, 
                                                            isAsciiIdContinue = isAsciiIdContinue))

    // is this a type?
    let pushType = 
        let findType t = 
            match t with
            | FindType stockTypes res -> str res
            | _ -> failFatally ("unknown type: " + t)

        pushIdentifier >>= findType
                    
                    
    let op : PushParser<string> = choice [str "*" ; str "+"]

    // operations of the same form as an identifier 
    let operation  = (tuple2 pushType (str "." >>. op)) |>> Operation
