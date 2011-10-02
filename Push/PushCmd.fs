namespace Push

open System
open System.Collections.Generic
open Arguments
open push.core

module PushCmd =
    
    let Run args =

        // Define what arguments are expected
        let defs = [
            {ArgInfo.Command="detailedError"; Description="Whether a detailed error report is required"; Required=false; DefaultValue = false };
            {ArgInfo.Command="pushCode"; Description="Should push the code on top of code stack"; Required=false; DefaultValue = true } ]

        // Parse Arguments into a Dictionary
        let parsedArgs = Arguments.ParseArgs args defs
        
        let mutable flags = ExecutionFlags.None
        if parsedArgs.["detailedError"] :?> bool then flags <- flags ||| ExecutionFlags.FullErrorReport
        if parsedArgs.["pushCode"] :?> bool then flags <- flags ||| ExecutionFlags.ShouldPushCode

        let mutable stop = false
        while (not stop) do
            let mutable runIt = false
            let mutable str = String.Empty
            
            while(not runIt) do
                let curStr = Console.ReadLine()
                if curStr.Trim() = "quit" then stop <- true
                else
                    str <- str + curStr.TrimEnd()
                    if str.[str.Length - 1] = ';' then runIt <- true
            
            if (not stop) then
                Program.ExecPushProgram (str, flags)
        0