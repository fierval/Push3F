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
    let pushType s = 
        let findType t = 
            let mutable reply = new Reply<string>()
            match t with
            | FindType stockTypes res -> 
                reply.Status <- Ok
                reply.Result <- res
            | _ -> 
                reply.Status <- Error
                reply.Error <- messageError("Unknown type: " + t)
            reply

        let identResult = commonIdentifier s
        if identResult.Status <> Ok 
        then 
            identResult
        else
            findType identResult.Result

        
                    
                    
    // TODO: This is stubbed out for now.
    let op : PushParser<string> = choice [str "*" ; str "+"]
    let pushIdentifier = commonIdentifier .>> nodot |>> Identifier

    // operation: identifier.op
    let pushOperation  = (tuple2 pushType (str "." >>. op)) |>> Operation
    
    // values of simple types
    let pushFloat = pfloat |>> Float
    let pushInt  = pint64 .>> nodot |>> Integer
    let pushTrue = pstringCI "true" .>> nodot >>% Bool true 
    let pushFalse = pstringCI "false" .>> nodot >>% Bool false
    
    //need to try integer first, as pfloat parses integers!
    let pushSimpleValue = choice [attempt pushInt;  pushFloat; attempt pushTrue; attempt pushFalse; attempt pushIdentifier; pushOperation]

    // pushProgram must be defined now, so we could use it inside the pushList definition.
    // however, pushList definition is part of defining pushProgram. To solve this catch 22,
    // FParsec provides createParserForwardedToRef function.
    let pushProgram, pushProgramRef = createParserForwardedToRef()

    let commonListParser sOpen sClose pElement f =
        between sOpen sClose
            (ws >>. sepBy pElement ws |>> f)    
    
    let pushList = commonListParser openList closeList pushProgram PushList

    do pushProgramRef := choice [pushSimpleValue
                                 pushList]

    let push = ws >>. pushProgram .>> ws .>> eof
    
    // UTF8 is the default, but it will detect UTF16 or UTF32 byte-order marks automatically
    let parsePushFile fileName encoding =
        runParserOnFile push () fileName System.Text.Encoding.UTF8

    let parsePushStream stream encoding =
        runParserOnStream push () "" stream System.Text.Encoding.UTF8
   
    let parsePushString str = run push str
    
    let extractResult = function  
                            | Success(r,_,_) -> 
                                printf "The AST is:\n%A\n" r
                                box(r)
                            | _ -> box(System.Int32.MinValue)
                               