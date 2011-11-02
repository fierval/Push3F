namespace Push

open System
open System.Collections.Generic
open Arguments
open push.core
open push.parser

module PushCmd =
    
    type State =
    | Start
    | List
    | End

    let writeInitialHelp = 
        (fun _ ->
            let strList = 
                [
                    "Welcome to Push 3 Interpreter. Just start typing at the prompt:"
                    "Push instructions separated by whitespace are executed separately."
                    "A single Push program is contained withing a list."
                    " ';' at the end of the line resets the interpreter"
                ]
            Console.ForegroundColor <- ConsoleColor.Green
            strList |> List.iter (fun str -> Console.WriteLine(str))
            Console.WriteLine()
            Console.ResetColor()
         )
    let Run args =

        // Define what arguments are expected
        let defs = [
            {ArgInfo.Command="detailedError"; Description="Whether a detailed error report is required"; Required=false; DefaultValue = null };
            {ArgInfo.Command="pushCode"; Description="Should push the code on top of code stack"; Required=false; DefaultValue = null } 
            {ArgInfo.Command="startFile"; Description="A push file to execute before everything else"; Required=false; DefaultValue=null} 
            {ArgInfo.Command="genetic"; Description="Launch in the genetic mode with the specified configuration"; Required=false; DefaultValue=null}]

        // Parse Arguments into a Dictionary
        let parsedArgs = Arguments.ParseArgs args defs

        if parsedArgs.Count < 3 then 
            Console.Error.Flush()
        else
            writeInitialHelp ()

            let mutable startFile = Unchecked.defaultof<string>
            let mutable configFileName = Unchecked.defaultof<string>

            let mutable flags = ExecutionFlags.None
            if parsedArgs.["detailedError"] <> null then 
                if bool.Parse(parsedArgs.["detailedError"] :?> string) then flags <- flags ||| ExecutionFlags.FullErrorReport

            if parsedArgs.["pushCode"] <> null then 
                if bool.Parse(parsedArgs.["pushCode"] :?> string) then flags <- flags ||| ExecutionFlags.ShouldPushCode

            if parsedArgs.["startFile"] <> null then startFile <- (parsedArgs.["startFile"] :?> string)

            if parsedArgs.["genetic"] <> null then configFileName <- (parsedArgs.["genetic"] :?> string)

            if not (String.IsNullOrEmpty startFile) then
                try
                    Program.ExecPushProgram("(\"" + startFile + "\" EXEC.OPEN)", flags)
                with
                | e -> Console.WriteLine ("{0}", e.Message)

            if parsedArgs.["genetic"] <> null then launchGeneticMode configFileName
            else
                let mutable stop = false
                while (not stop) do
                    let mutable runIt = false
                    let mutable str = String.Empty
                    let mutable state = State.Start
                    let flags = flags // mutable variables cannot be part of closures, so assigning to the immubable
                        
                    while(state <> State.End && not stop) do
                        let continueParsing st = 
                            (fun s -> 
                                let prog = st + " " + s
                                if parsingSucceeded prog then Program.ExecPushProgram(prog, flags); String.Empty
                                else
                                    prog
                                )
                             
                        Console.Write(">")
                        let curStr = Console.ReadLine().Trim()
                        if curStr.Trim() = "quit" then stop <- true
                        if curStr.EndsWith(";") then state <- State.End

                        let startOfList = (curStr.StartsWith("("))
                        if not stop then
                            match startOfList, state with
                            | (false, State.Start) ->
                                let strings = curStr.Split([|'\t'; ' '|])
                                try
                                    strings |> Array.iter (fun e -> Program.ExecPushProgram(e, flags))
                                    state <- State.End
                                with
                                | e -> Console.WriteLine(e.Message)
                            |  (_, (State.List | State.Start)) ->
                                try
                                    str <- continueParsing str curStr
                                    state <- if String.IsNullOrEmpty(str) then State.End else State.List

                                with
                                | e -> 
                                    Console.WriteLine (e.Message)
                                    state <- State.End
                            | _ -> ()
        0