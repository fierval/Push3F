﻿namespace Push

open System
open System.Collections.Generic
open System.Text.RegularExpressions

module Arguments =

    // Type for simple argument checking
    type ArgInfo = { Command:string; Description:string; Required:bool; DefaultValue : obj }

    // Displays the help arguments
    let DisplayHelp (defs:ArgInfo list) =
        match defs with
        | [] -> Console.WriteLine "No help text defined."
        | _ ->
            Console.WriteLine "Command Arguments:"
            defs
            |> List.iter (fun def ->
                let helpText = sprintf "-%s (Required=%b) : %s" def.Command def.Required def.Description
                Console.WriteLine helpText )

    // Displays the found arguments
    let DisplayArgs (args:Dictionary<string, obj>) =
        match args.Keys.Count with
        | 0 -> Console.WriteLine "No arguments found."
        | _ ->
            Console.WriteLine "Arguments Found:"
            for arg in args.Keys do
                if null = args.[arg] then
                    Console.WriteLine (sprintf "-%s" arg)
                else
                    Console.WriteLine (sprintf "-%s '%s'" arg (args.[arg].ToString()))

    // Parse the input arguments
    let ParseArgs (args:string array) (defs:ArgInfo list) =

        let parsedArgs = new Dictionary<string, obj>(StringComparer.InvariantCultureIgnoreCase)

        // Ensure help is supported if defintions provided
        let fullDefs = 
            if not (List.exists (fun def -> String.Equals(def.Command, "help")) defs) then
                {ArgInfo.Command="help"; Description="Display Help Text"; Required=false; DefaultValue = null } :: defs
            else
                defs

        // Report errors
        let reportError errorText =        
            DisplayArgs parsedArgs
            DisplayHelp fullDefs
            let errMessage = sprintf "Error occured: %A" errorText
            Console.Error.WriteLine errMessage
            Console.Error.Flush()
            Environment.Exit(1)

        // Capture variables
        let captureArg command value =
            match defs with
            | [] -> parsedArgs.Add(command, value)
            | _ -> 
                if not (List.exists (fun def -> String.Equals(def.Command, command, StringComparison.InvariantCultureIgnoreCase)) fullDefs) then
                    reportError (sprintf "Command '%s' Not in definition list." command)
                else
                    parsedArgs.Add(command, value)            

        let (|IsCommand|_|) (command:string) =            
            let m = Regex.Match(command, "^(?:-{1,2}|\/)(?<command>.*)$", RegexOptions.IgnoreCase)
            if m.Success then Some(m.Groups.["command"].Value.ToLower()) else None

        let rec loop (argList:string list) =
            match argList with
            | [] -> ()
            | head::tail ->
                match head with
                | IsCommand command ->
                    match tail with
                    | [] -> captureArg command (box(String.Empty))
                    | iHead::iTail -> 
                        match iHead with
                        | IsCommand iCommand ->
                            captureArg command (box(String.Empty))
                            loop tail
                        | _ ->
                            captureArg command (box(iHead))
                            loop iTail
                | _ -> reportError (sprintf "Expected a command but got '%s'" head)
        loop (Array.toList args)
        
        // Look to see if help has been requested if not check for required
        if (parsedArgs.ContainsKey("help")) then
            DisplayHelp defs
        else
            defs
            |> List.filter (fun def -> def.Required) 
            |> List.iter ( fun def ->
                if not (parsedArgs.ContainsKey(def.Command)) then
                    reportError (sprintf "Command '%s' found but in argument list." def.Command))
            defs
            |> List.filter (fun def -> not def.Required)
            |> List.iter (fun def -> 
                if not (parsedArgs.ContainsKey(def.Command)) then
                    parsedArgs.Add(def.Command, def.DefaultValue))

        parsedArgs