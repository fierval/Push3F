namespace push.parser

module Parser =

    open FParsec
    open Ast
    open ParserShared
    open push.types.Type
    open push.types.stock.StockTypesInteger
    open push.types.stock.StockTypesBool
    open push.types.stock.StockTypesFloat
    open push.types.TypeFactory
    open System.Reflection
    open System.Runtime.CompilerServices

    // override this delegate to parse extended types
    type ExtendedTypeParser = delegate of string -> PushTypeBase

    // identifier
    // is this a type?
    let internal pushType s = 
        let findType t = 
            let mutable reply = new Reply<string>()
            match t with
            | FindType res -> 
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

    
    let internal pushIdentifier = commonIdentifier .>> nodot |>> Identifier
    
    // operation: identifier.op
    let internal pushOp s = 
        let findOp tp op =
            let mutable reply = new Reply<MethodInfo>()
            match op with
            | FindOperation tp res -> 
                reply.Status <- Ok
                reply.Result <- res
            | _ -> 
                reply.Status <- Error
                reply.Error <- messageError("Unknown operation: " + tp)
            reply

        let typeParser = tuple2 (pushType .>> str ".") commonIdentifier
        let typeResult = typeParser s
        if typeResult.Status <> Ok 
        then 
            let mutable reply = new Reply<MethodInfo>()
            reply.Status <- Error
            reply.Error <- typeResult.Error
            reply
        else
            findOp (fst typeResult.Result) (snd typeResult.Result)

    let internal pushOperation = pushOp |>> Operation

    // values of simple types
    let internal pushFloat = pfloat |>> createFloat |>> Value
    let internal pushInt  = pint64 .>> nodot |>> createInteger |>> Value
    let internal pushTrue = pstringCI "true" .>> nodot >>% Value (createBool true )
    let internal pushFalse = pstringCI "false" .>> nodot >>% Value (createBool false )
    
    //TODO: enable extending the parser with new tokens for new types
    let internal pushExtended (dlg : ExtendedTypeParser) = stringToken |>> (dlg.Invoke >> Value)

    let internal pushSimple = choice [
                                attempt pushInt
                                pushFloat
                                attempt pushTrue
                                attempt pushFalse
                                attempt pushIdentifier
                                attempt pushOperation
                                      ]

    // pushProgram must be defined now, so we could use it inside the pushList definition.
    // however, pushList definition is part of defining pushProgram. To solve this catch 22,
    // FParsec provides createParserForwardedToRef function.
    let internal pushProgram, internal pushProgramRef = createParserForwardedToRef()


    let internal listSeries = (sepBy pushProgram (spaces1 .>> notFollowedBy closeList)) |>> PushList

    let internal pushList = between openList closeList listSeries

    do pushProgramRef := choice [
                                 pushSimple
                                 pushList
                                ]

    let internal push = ws >>. pushProgram .>> ws .>> eof
    
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
                            | Failure(s, e, _) -> box((s,e))
                               