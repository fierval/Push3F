namespace push.types.stock

module StockTypesCode =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory
    open push.parser.Ast
    open push.stack.Stack
    open push.types.stock.StockTypesBool
    open System.Reflection


    [<PushType("CODE")>]
    type Code =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (p : Push) = {inherit PushTypeBase(p)}

        static member private Me = new Code()

        override t.ToString() =
          base.ToString()

        // custom parsing.
        // in this case custom parsing is disabled.
        // Push will parse these values
        override t.Parser 
            with get() = 
                Unchecked.defaultof<ExtendedTypeParser>

        [<PushOperation("=", Description = "Compares two top pieces of code")>]
        static member Eq() = 
            match processArgs2 Code.Me.MyType with
            | [a1; a2] -> 
                pushResult (new Bool (a1.Raw<Push>() = a2.Raw<Push>()))
            | _ -> ()

        [<PushOperation("APPEND", Description = "Appends two top pieces of code. Converts either one to list if necessary")>]
        static member Append() =
            match processArgs2 Code.Me.MyType with
            | [a1; a2] -> 
                let l1appendl2 = 
                    match (a1.Raw<Push>().toList), (a2.Raw<Push>().toList) with
                    | PushList lst1, PushList lst2 -> PushList (lst1 @ lst2)
                    | _ -> PushList []

                pushResult (new Code(l1appendl2))
            |_ -> ()

        [<PushOperation("ATOM", Description = "TRUE if the top item is atomic, FALSE otherwise")>]
        static member Atom() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> then ()
            pushResult (new Bool(a.Raw<Push>().isList))
                   
        [<PushOperation("CAR", Description = "Pushes the first item of the top of the stack. If top of the stack is an atom - no effect")>]
        [<PushOperation("FIRST", Description = "This is a more explicit name for the CAR operation")>]
        static member First() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> || not (a.Raw<Push>().isList) then ()
            let arg = (processArgs1 Code.Me.MyType).Raw<Push>()
            match arg with
            | PushList l -> pushResult (new Code(l.Head))
            | _ -> pushResult(new Code(arg))

        [<PushOperation("CDR", Description = "Pushes the \"rest\" of the top of the stack. If top of the stack is an atom pushes ()")>]
        [<PushOperation("REST", Description = "This is a more explicit name for the CDR operation")>]
        static member Rest() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> || not (a.Raw<Push>().isList) then pushResult (new Code(PushList []))
            let arg = (processArgs1 Code.Me.MyType).Raw<Push>()
            match arg with
            | PushList l -> pushResult (new Code(PushList l.Tail))
            | _ -> pushResult(new Code(PushList []))

        [<PushOperation("CONS", Description = "if fst is on top of the stack, and snd right udner: (CONS snd fst) -> (snd fst)")>]
        static member Cons() =
            match processArgs2 Code.Me.MyType with
            | [a1; a2] -> pushResult (new Code(PushList (a1.Raw<Push>() :: a2.Raw<Push>().asPushList)))
            | _ -> ()

        [<PushOperation("CONTAINER", Description = "if fst is on top of the stack, and snd right udner, returns the container of the second item in the first")>]
        static member Containter() =
            let ret = ref (PushList [])
            let haveResult = 
                match !ret with
                | PushList l -> not l.IsEmpty
                | _ -> false

            let rec container ofA stackOfInB=
                if haveResult then ()
                else
                    let topInB = peek stackOfInB
                    for b in topInB do
                        if not haveResult && b.Equals(ofA) then ret := PushList topInB 
                        else
                            match b with
                            | PushList blist -> container ofA (push blist stackOfInB)
                            | _ -> ()
                    
                    container ofA (snd (pop stackOfInB))
            
            match processArgs2 Code.Me.MyType with
            | [a1; a2] -> 
                    match a2.Raw<Push>() with
                    | PushList l -> 
                        container (a1.Raw<Push>()) (push l empty)
                        pushResult (new Code(!ret))
                    | _ -> pushResult (new Code(PushList []))
            | _ -> pushResult (new Code(PushList []))