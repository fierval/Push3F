namespace push.parser
open System
open push.types
open push.stack

[<AutoOpen>]
module Eval =

    let internal evalCode (top : Push) exec = 
        let mapToPushTypeBaseStack (stack : Stack<Push>) =
            StackNode (stack.asList |> List.map (fun e -> makePushBaseType e exec))

        match top with
        | Value v -> pushResult v.Eval
        | Operation (nm, methodInfo) -> execOperation [|nm|] methodInfo 
        | PushList l -> 
            // push in the reverse order
            let updatedStack = 
                l |> List.rev
                |> List.fold (fun stack e -> pushStack e stack) empty
                    
            let newRunningStack = append (updatedStack |> mapToPushTypeBaseStack) (stockTypes.Stacks.[exec])
            stockTypes.Stacks <- stockTypes.Stacks.Replace(exec, newRunningStack)

    let mutable maxSteps = 100000

    let internal eval exec =
        let mutable executionSteps = 0

        while not (isEmptyStack exec || executionSteps >= maxSteps) do
            executionSteps <- executionSteps + 1
            let top = (processArgs1 exec).Raw<Push>()
            evalCode top exec
        
    let internal pushToStack name (pushObj : Push) = 
        let execObj = makePushBaseType pushObj name 
        pushResult execObj

    // pushes an item on to the EXEC stack to be evaluated
    let internal pushToExec = pushToStack "EXEC"

    let internal pushAndEval (code : Push) =
        pushToExec code
        eval "EXEC"

    let makeOperation typeName operationName =
        Operation(typeName, stockTypes.Operations.[typeName].[operationName])
