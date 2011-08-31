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

    // parsing out an operation
    let internal pushType = commonIdentifier >>= findType
    let internal pushOp = tuple2 (pushType .>> str ".") stringToken >>= findOp
    let internal pushOperation = pushOp |>> Operation

    // values of simple types
    let internal pushFloat = pfloat |>> createFloat |>> Value
    let internal pushInt  = pint64 .>> nodot |>> createInteger |>> Value
    let internal pushTrue = pstring "TRUE" .>> nodot >>% Value (createBool true )
    let internal pushFalse = pstring "FALSE" .>> nodot >>% Value (createBool false )
    let internal pushIdentifier = commonIdentifier >>= validIdentifier .>> nodot |>> createIdentifier |>> Value
    
    //TODO: enable extending the parser with new tokens for new types
    let internal pushExtended (dlg : ExtendedTypeParser) = stringToken |>> (dlg.Invoke >> Value)

    let internal pushSimple = choice [
                                attempt pushInt
                                attempt pushFloat
                                attempt pushTrue
                                attempt pushFalse
                                attempt pushIdentifier
                                attempt pushOperation
                                      ]

    // pushProgram must be defined now, so we could use it inside the pushList definition.
    // however, pushList definition is part of defining pushProgram. To solve this catch 22,
    // FParsec provides createParserForwardedToRef function.
    let internal pushProgram, internal pushProgramRef = createParserForwardedToRef()


    let internal listSeries = (sepEndBy pushProgram spaces1) |>> PushList

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
                               