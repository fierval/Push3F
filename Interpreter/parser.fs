namespace push.parser

[<AutoOpen>]
module Parser =

    open FParsec
    open push.types
    open System.Reflection
    open System.Runtime.CompilerServices

    // parsing out an operation
    let internal pushType = commonIdentifier >>= findType
    let internal pushOp = tuple2 (pushType .>> str ".") stringTokenNoDot >>= findOp
    let internal pushOperation = pushOp |>> Operation

    // values of simple types
    let internal pushIdentifier = commonIdentifier >>= validIdentifier .>> nodot |>> createIdentifier >>= createValue
    
    let internal pushSimple = choice [
                                    pushSimpleTypes
                                    attempt pushIdentifier
                                    attempt pushOperation
                                    attempt pushLiteral
                                      ]

    // pushProgram must be defined now, so we could use it inside the pushList definition.
    // however, pushList definition is part of defining pushProgram. To solve this catch 22,
    // FParsec provides createParserForwardedToRef function.
    let internal pushProgram, internal pushProgramRef = createParserForwardedToRef()


    let internal listSeries = (sepEndBy pushProgram spaces) |>> PushList

    let internal pushList = between openList closeList listSeries

    do pushProgramRef := choice [
                                 pushSimple
                                 pushList
                                ]

    let internal push = ws >>. pushProgram .>> ws .>> eof
    
    // UTF8 is the default, but it will detect UTF16 or UTF32 byte-order marks automatically
    let parsePushFile fileName =
        runParserOnFile push () fileName System.Text.Encoding.UTF8

    let parsePushStream stream encoding =
        runParserOnStream push () "" stream System.Text.Encoding.UTF8
   
    let parsePushString str = run push str
    
    let extractResult = function  
                            | Success(r,_,_) -> 
                                box(r)
                            | Failure(s, e, _) -> box((s,e))
    
    let parsingSucceeded str = 
        match parsePushString str with
        | Success(_,_,_) -> true
        | _ -> false

    let parseGetCode str = (parsePushString >> extractResult) str :?> Push                                           