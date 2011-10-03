namespace push.core

[<AutoOpen>]
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
    | None = 0u
    | FullErrorReport = 1u
    | ShouldPushCode = 2u

    let report (s, e) full =
        if full then 
            raise (PushException (e.ToString()))
        else 
            failwith s

    let internal execProgram program shouldPushCode =
        pushResult (Exec(program))
        if shouldPushCode then pushResult (Code(program))
        eval "EXEC"

    // the callback type for the function used
    type internal parseFunc = string -> ParserResult<Push, unit>

    let isFlagSet flag enumeration =
        flag &&& enumeration = flag

    let internal execPush parse str execParams =
        let fullErrorReport = isFlagSet ExecutionFlags.FullErrorReport execParams 
        let shouldPushCode = isFlagSet ExecutionFlags.ShouldPushCode execParams
        try
            let res = extractResult (parse str)
            if not (FSharpType.IsTuple(res.GetType()))
            then
                execProgram (unbox<Push> res) shouldPushCode
            else
                report(unbox<string*ParserError> res) fullErrorReport
        with
        | e -> raise e
        
    let ExecPushProgram (prog, execParams) = 
        execPush parsePushString prog execParams

    let ExecPush (prog) =
        execPush parsePushString prog (ExecutionFlags.FullErrorReport ||| ExecutionFlags.ShouldPushCode)

    // actual "entry point" to execute a file containing a program
    let ExecPushFromFile fileName execParams =
        if not (File.Exists(fileName)) then raise (PushException("File does not exist: " + fileName))
        execPush parsePushFile fileName execParams