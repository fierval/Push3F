namespace push.types

[<AutoOpen>]
module PopModule =

    open System.Reflection
    open push.stack
    open push.exceptions

    type PushMonad<'a> = List<PushTypeBase> -> 'a * List<PushTypeBase>

    let popOne stack : PushMonad<'a> = 
        (fun state ->
            if isEmptyStack stack 
            then
                state |> List.iter(fun e -> pushResult e) 
                Unchecked.defaultof<'a>, Unchecked.defaultof<List<PushTypeBase>> 
            else 
                let arg = processArgs1 stack
                arg.Raw<'a>(), arg :: state
        )

    let result stack value : PushMonad<'a> = 
        (fun state -> 
            let resultingValue, stack = createPushObject (stockTypes.Types.[stack].GetType()) [|value|]
            value, resultingValue::state)

    type PushBuilder () =

        member this.Bind (p : PushMonad<'a>, f : 'a -> PushMonad<'b>) : PushMonad<'b> =
            (fun state -> 
                let value, state = p state
                if state = Unchecked.defaultof<List<PushTypeBase>> then
                    Unchecked.defaultof<'b>, state 
                else
                    f value state
            )
        
        member this.Return (x : 'a) : PushMonad<'a> =
            (fun state ->
                if state <> Unchecked.defaultof<List<PushTypeBase>> then 
                    pushResult state.Head
                x, [])

        member this.ReturnFrom (m : PushMonad<'a>) : PushMonad<'a> = 
            (fun state -> 
                let value, state = m state
                if state <> Unchecked.defaultof<List<PushTypeBase>> then 
                    pushResult state.Head
                    value, state.Tail
                else
                    value, state
            )

        member this.Zero () : PushMonad<'a> = (fun state -> state |> List.iter (fun e -> pushResult e); Unchecked.defaultof<'a>, [])

        member this.Delay (f : unit -> PushMonad<'a>) = f ()

        member this.Run (f : PushMonad<'a>) = f []

        member this.Combine (m1 : PushMonad<'a>, m2 : PushMonad<'a>) =
            (fun state -> 
                let value, state = m1 state
                m2 state)

    let push = new PushBuilder()

    let monoOp f stack resStack = push {
        let! arg = popOne stack
        let! res = result resStack (f arg)
        return res
    }

    let dualOp f stack resStack = push {
        let! right = popOne stack
        let! left = popOne stack
        return! result resStack (f left right)
        }

    let simpleOp f stack = dualOp f stack stack
