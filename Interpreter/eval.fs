namespace push.parser

[<AutoOpen>]
module Eval =
    open System
    open push.types
    open push.stack

    let internal (|FindStack|_|) str = 
        match stockTypes.Stacks.TryFind(str), str with
        | (Some stack, name) -> Some(stack, name)
        | _ -> None


    let makePushBaseType e name =
        let execType = stockTypes.Types.[name].GetType()
        fst (createPushObject execType [|e|])

    let mapToPushTypeStack (stack : Stack<#PushTypeBase>) =
        StackNode(stack.asList |> List.map (fun e -> e.Value :?> Push))

    let internal evalStack name shouldPopFirst topOnly=
        let mapToPushTypeBaseStack (stack : Stack<Push>) =
            StackNode (stack.asList |> List.map (fun e -> makePushBaseType e name))

        let stack = stockTypes.Stacks.[name]
        let startingLength = stack.length

        let preserveTop = 
            match peekStack name with
            | v when startingLength = 0 -> None
            | s -> Some s

        while ((topOnly && stockTypes.Stacks.[name].length >= startingLength && startingLength <> 0) || (not topOnly && not (isEmptyStack name))) do
            let top = (processArgs1 name).Raw<Push>()
            if getState name = State.Quote then ()
            else
                match top with
                | Value v -> 
                    if getState v.MyType = State.Quote
                    then
                        pushResult v
                        setState v.MyType State.Exec |> ignore
                    else
                        pushResult v.Eval

                | Operation (nm, methodInfo) -> execOperation [|name; nm|] methodInfo 
                | PushList l -> 
                    // push in the reverse order
                    let updatedStack = 
                        l |> List.rev
                        |> List.fold (fun stack e -> push e stack) empty
                    
                    let newRunningStack = append (updatedStack |> mapToPushTypeBaseStack) (stockTypes.Stacks.[name])
                    stockTypes.Stacks <- stockTypes.Stacks.Replace(name, newRunningStack)

        if not shouldPopFirst
        then 
            match preserveTop with
            | Some t -> pushResult t
            | _ -> ()
            

    // evaluates an object on the top of the stack
    let eval stackName topOnly =
        match stackName with
        | FindStack (stack, name) -> evalStack name true topOnly
        | _ -> ()
        
    // evaluates an object on top of the stack and pops
    // the stack after evaluating.
    let evalStar stackName topOnly=
        match stackName with
        | FindStack (stack, name) -> evalStack  name false topOnly
        | _ -> ()


    let internal pushToStack name (pushObj : Push) = 
        let execObj = makePushBaseType pushObj name 
        pushResult execObj

    // pushes an item on to the EXEC stack to be evaluated
    let internal pushToExec = pushToStack "EXEC"

    // pushes an item on to the CODE stack to be evaluated right away
    let internal pushToCode push = 
        let code = "CODE"
        pushToStack code push
        let op = Operation(code, stockTypes.Operations.[code].["DO"])
        pushToExec op