namespace push.parser
open System
open push.types
open push.stack

[<AutoOpen>]
module Eval =
    let internal eval () =
        let exec = "EXEC"
        let mapToPushTypeBaseStack (stack : Stack<Push>) =
            StackNode (stack.asList |> List.map (fun e -> makePushBaseType e exec))

        let mutable executionSteps = 0

        while not (isEmptyStack exec || executionSteps >= 100000) do
            executionSteps <- executionSteps + 1
            let top = (processArgs1 exec).Raw<Push>()
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

    let internal pushToStack name (pushObj : Push) = 
        let execObj = makePushBaseType pushObj name 
        pushResult execObj

    // pushes an item on to the EXEC stack to be evaluated
    let internal pushToExec = pushToStack "EXEC"
