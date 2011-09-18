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

    [<System.Flags>]
    type ExecutionFlags =
    | FullErrorReport = 1u
    | ShouldPushCode = 2u

    let internal report (s, e) full =
        if full then 
            printfn "%s" (e.ToString())
        else 
            printfn "%s" s

    let internal exceptionReport (e : Exception) =
        printfn "%s" (e.ToString())


    let internal execProgram program shouldPushCode =
        pushResult (Exec(program))
        if shouldPushCode then pushResult (Code(program))
        eval "EXEC"

    // the callback type for the function used
    type internal parseFunc = string -> ParserResult<Push, unit>

    let internal execPush parse str execParams =
        let fullErrorReport = (ExecutionFlags.FullErrorReport &&& execParams = ExecutionFlags.FullErrorReport)
        let shouldPushCode = (ExecutionFlags.ShouldPushCode &&& execParams = ExecutionFlags.ShouldPushCode)
        try
            let res = extractResult (parse str)
            if not (FSharpType.IsTuple(res.GetType()))
            then
                execProgram (unbox<Push> res) shouldPushCode
            else
                report(unbox<string*ParserError> res) fullErrorReport
        with
        | e -> exceptionReport e
        
    let ExecPushProgram prog fullErrorReport execParams = 
        execPush parsePushString prog execParams

    // actual "entry point" to execute a file containing a program
    let ExecPushFromFile fileName execParams =
        if not (File.Exists(fileName)) then raise (PushException("File does not exist: " + fileName))
        execPush parsePushFile fileName execParams