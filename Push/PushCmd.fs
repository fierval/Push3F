namespace Push

open System
open System.Collections.Generic
open Arguments
open push.core

module PushCmd =
    
    let Run args =

        // Define what arguments are expected
        let defs = [
            {ArgInfo.Command="detailedError"; Description="Whether a detailed error report is required"; Required=false; DefaultValue = null };
            {ArgInfo.Command="pushCode"; Description="Should push the code on top of code stack"; Required=false; DefaultValue = null } 
            {ArgInfo.Command="startFile"; Description="A push file to execute before everything else"; Required=false; DefaultValue=null} ]

        // Parse Arguments into a Dictionary
        let parsedArgs = Arguments.ParseArgs args defs

        if parsedArgs.Count < 3 then 
            Console.Error.Flush()
        else
            let mutable startFile = Unchecked.defaultof<string>

            let mutable flags = ExecutionFlags.None
            if parsedArgs.["detailedError"] <> null then 
                if bool.Parse(parsedArgs.["detailedError"] :?> string) then flags <- flags ||| ExecutionFlags.FullErrorReport

            if parsedArgs.["pushCode"] <> null then 
                if bool.Parse(parsedArgs.["pushCode"] :?> string) then flags <- flags ||| ExecutionFlags.ShouldPushCode

            if parsedArgs.["startFile"] <> null then startFile <- (parsedArgs.["startFile"] :?> string)

            if not (String.IsNullOrEmpty startFile) then
                try
                    Program.ExecPushProgram("(\"" + startFile + "\" EXEC.OPEN)", flags)
                with
                | e -> Console.WriteLine ("{0}", e.Message)

            let mutable stop = false
            while (not stop) do
                let mutable runIt = false
                let mutable str = String.Empty
            
                while(not runIt && not stop) do
                    Console.Write(">")
                    let curStr = Console.ReadLine()
                    if curStr.Trim() = "quit" then stop <- true
                    else
                        str <- str + " " + curStr.TrimEnd()
                        if str.[str.Length - 1] = ';' then str <- str.Substring(0, str.Length - 1); runIt <- true
                        Console.WriteLine(str.Trim())
                if (not stop) then
                    try
                        Program.ExecPushProgram (str, flags)
                    with
                    | e -> Console.WriteLine ("{0}", e.Message)
        0