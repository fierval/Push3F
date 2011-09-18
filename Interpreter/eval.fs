namespace push.parser

[<AutoOpen>]
module Eval =
    open System
    open push.types
    open push.stack

    let internal (|FindStack|_|) str = 
        match stockTypes.Stacks.TryFind(str), str with
        | (Some stack, name) ->
            if stack.GetType().GetGenericArguments().[0].GetType() <> typeof<Push> then None
            else
                Some(stack, name)
        | _ -> None

    let rec internal evalStack stack name shouldPopFirst =
        let rec evalStackTailRec (top : PushTypeBase) stack name = 
            match top.Raw<Push>() with
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
                let top, updatedStack = 
                    fst (popMany l.Length stack) 
                    |> List.fold (fun stack e -> push e stack) empty 
                    |> pop
                evalStackTailRec top updatedStack top.MyType
    
        let top = if shouldPopFirst then processArgs1 name else peekStack name
        if top = Unchecked.defaultof<PushTypeBase> 
        then 
            ()
        else
            evalStackTailRec top stack name
        if not shouldPopFirst 
        then 
            processArgs1 name |> ignore

    // evaluates an object on the top of the stack
    let eval =
        function
        | FindStack (stack, name) -> evalStack stack name true
        | _ -> ()
        
    // evaluates an object on top of the stack and pops
    // the stack after evaluating.
    let evalStar =
        function
        | FindStack (stack, name) -> evalStack stack name false
        | _ -> ()

    // pushes an item on to the EXEC stack to be evaluated
    let pushToExec (pushObj : Push) =
        let execType = stockTypes.Types.["EXEC"].GetType()
        let execObj = fst (createPushObject execType [|pushObj|])
        pushResult execObj
