namespace push.core

module Program =
    open System
    open System.IO
    
    open push.parser
    open push.types
    open push.types.stock
    open push.exceptions
    open Microsoft.FSharp.Reflection
    open FParsec

    let internal report (s, e) full =
        if full then 
            printfn "%s" (e.ToString())
        else 
            printfn "%s" s

    let internal exceptionReport (e : Exception) =
        printfn "%s" (e.ToString())


    let execProgram program =
        pushResult (Exec(program))
        eval "EXEC"

    // the callback type for the function used
    type parseFunc = string -> ParserResult<Push, unit>

    let execPush parse str fullErrorReport =
        try
            let res = extractResult (parse str)
            if not (FSharpType.IsTuple(res.GetType()))
            then
                execProgram (unbox<Push> res)
            else
                report(unbox<string*ParserError> res) fullErrorReport
        with
        | e -> exceptionReport e
        
    let ExecPushProgram prog fullErrorReport = 
        execPush parsePushString prog fullErrorReport

    // actual "entry point" to execute a file containing a program
    let ExecPushFromFile fileName fullErrorReport =
        if not (File.Exists(fileName)) then raise (PushException("File does not exist: " + fileName))
        execPush parsePushFile fileName fullErrorReport