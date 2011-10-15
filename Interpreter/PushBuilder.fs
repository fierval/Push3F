namespace push.types

[<AutoOpen>]
module PopModule =

    open System.Reflection
    open push.stack
    open push.exceptions

    type internal PushMonad<'a> = List<PushTypeBase> -> 'a * List<PushTypeBase>

    let internal makePushBaseType e name =
        let execType = stockTypes.Types.[name].GetType()
        fst (createPushObject execType [|e|])

    let internal topOne<'a> stack (getTop : string -> PushTypeBase) : PushMonad<'a> = 
        (fun state ->
            if isEmptyStack stack 
            then
                state |> List.iter(fun e -> pushResult e) 
                Unchecked.defaultof<'a>, List.empty 
            else 
                let arg = getTop stack
                arg.Raw<'a>(), arg :: state
        )

    let internal popOne<'a> stack : PushMonad<'a> = 
        topOne stack processArgs1

    let internal peekOne<'a> stack : PushMonad<'a> = 
        topOne stack peekStack

    let internal result stack value : PushMonad<'a> = 
        (fun state -> 
            let resultingValue = makePushBaseType value stack
            value, resultingValue::state)

    let internal resulList stack values : PushMonad<'a> =
        (fun state ->
            let newState = values |> List.fold(fun values e -> (makePushBaseType stack e)::state) state
            Unchecked.defaultof<'a>, newState
        )

    let internal zero : PushMonad<unit> =
        (fun state -> (), [])

    type internal PushBuilder () =

        member this.Bind (p : PushMonad<'a>, f : 'a -> PushMonad<'b>) : PushMonad<'b> =
            (fun state -> 
                let value, state = p state
                if state = List.empty then
                    Unchecked.defaultof<'b>, state 
                else
                    f value state
            )
        
        member this.Return (x : 'a) : PushMonad<'a> =
            (fun state ->
                if state <> List.empty then 
                    pushResult state.Head
                    x, state.Tail
                else
                    x, [])

        member this.ReturnFrom (m : PushMonad<'a>) : PushMonad<'a> = 
            (fun state -> 
                let value, state = m state
                if state <> List.empty then 
                    pushResult state.Head
                    value, state.Tail
                else
                    value, state
            )

        member this.While (cond, p : PushMonad<'a>) : PushMonad<'a> =
            let rec whileLoop cond value accum =
                match cond() with
                | true -> value, accum
                | _ ->
                    let newVal, accum = p accum
                    whileLoop cond newVal accum

            (fun state -> 
                let value, accum = p state
                whileLoop cond value accum
            )
                
        member this.Zero () : PushMonad<'a> = (fun state -> state |> List.iter (fun e -> pushResult e); Unchecked.defaultof<'a>, [])

        member this.Delay (f : unit -> PushMonad<'a>) = f ()

        member this.Run (f : PushMonad<'a>) = f []

        member this.Combine (m1 : PushMonad<'a>, m2 : PushMonad<'a>) =
            (fun state -> 
                let value, state = m1 state
                m2 state)

    let internal push = new PushBuilder()

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
