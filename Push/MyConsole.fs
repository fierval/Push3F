namespace Push

open System
open System.Collections.Generic
open Arguments

module MyConsole =
        
    let Run args =

        // Define what arguments are expected
        let defs = [
            {ArgInfo.Command="a1"; Description="Argument 1"; Required=true };
            {ArgInfo.Command="a2"; Description="Argument 2"; Required=false } ]

        // Parse Arguments into a Dictionary
        let parsedArgs = Arguments.ParseArgs args defs
        
        // TODO add your code here 
        Arguments.DisplayArgs parsedArgs
        Console.ReadLine() |> ignore