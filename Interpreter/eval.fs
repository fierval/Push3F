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

    let internal evalStack stack name shouldPopFirst topOnly=
        let mapToPushTypeBaseStack (stack : Stack<Push>) =
            StackNode (stack.asList |> List.map (fun e -> makePushBaseType e name))

        let preserveTop = peekStack name

        let rec evalIt (top, (stack : Stack<Push>)) =
            if stack.length = 0 then ()
            match top with
            | Value v -> 
                if not v.isQuotable 
                then 
                    pushResult v.Eval
                elif getState v.MyType = State.Quote
                then
                    pushResult v
                    setState v.MyType State.Exec |> ignore

            | Operation (name, methodInfo) -> execOperation name methodInfo 
            | PushList l -> 
                // push in the reverse order
                let updatedStack = 
                    l |> List.rev
                    |> List.fold (fun stack e -> push e stack) empty
                
                evalIt (pop updatedStack)
            evalIt (pop stack)

        let stop = ref false
        while (not (isEmptyStack name || !stop)) do
            let top = (processArgs1 name).Raw<Push>()
            evalIt (top, stack)                        
            stop := topOnly

        if not shouldPopFirst 
        then 
            pushResult preserveTop |> ignore

            

    // evaluates an object on the top of the stack
    let eval stackName topOnly =
        match stackName with
        | FindStack (stack, name) -> evalStack (stack |> mapToPushTypeStack) name true topOnly
        | _ -> ()
        
    // evaluates an object on top of the stack and pops
    // the stack after evaluating.
    let evalStar stackName topOnly=
        match stackName with
        | FindStack (stack, name) -> evalStack (stack |> mapToPushTypeStack) name false topOnly
        | _ -> ()

    // pushes an item on to the EXEC stack to be evaluated
    let pushToExec (pushObj : Push) =
        let execObj = makePushBaseType pushObj "EXEC" 
        pushResult execObj
