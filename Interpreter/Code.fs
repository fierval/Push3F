namespace push.types.stock

module StockTypesCode =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory
    open push.parser.Ast
    open push.parser.Eval
    open push.stack.Stack
    open push.types.stock.StockTypesBool
    open push.types.stock.StockTypesInteger
    open System.Reflection
    open System

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

        override t.isQuotable with get() = true

        static member internal pushArgsBack (args : PushTypeBase list) =
            pushResult args.Head
            pushResult args.Tail.Head

        // this function collects all of the containers ofA that are inB
        static member internal getContainers (ofA : Push) (stackOfInB : Stack<Push list>) =
            let containers : Stack<Push> ref = ref empty
            let stackOfInB = ref stackOfInB
            match ofA with
            | PushList [] -> containers := push (PushList (peek !stackOfInB)) !containers; !containers
            | _ ->
                while not (!stackOfInB).isEmpty do
                    let topInB = peek !stackOfInB
                    for b in topInB do
                        if b.Equals(ofA) then containers := push (PushList topInB) !containers
                        else
                            match b with
                            | PushList blist -> if blist.Length < ofA.asPushList.Length then () else stackOfInB := shove (!stackOfInB).length blist !stackOfInB
                            | _ -> ()
                    stackOfInB := (snd (pop !stackOfInB))
                !containers

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
        static member Container() =
            match peekStack2 Code.Me.MyType with
            | [aSnd; aFst] -> 
                match aFst.Raw<Push>() with
                | PushList l -> 
                        let res = Code.getContainers (aSnd.Raw<Push>()) (push l empty)
                        if res.isEmpty then pushResult (new Code (PushList []))
                        else // if we have several containers, find the index of the one with min length
                            let containerIndex = 
                                res.asList 
                                |> 
                                List.mapi (fun i e -> 
                                    match e with 
                                    | PushList l -> i, l.Length
                                    | _ -> i, -1) 
                                |> List.minBy (fun (index, value) -> value) 
                                |> fst

                            pushResult (new Code(res.asList.[containerIndex]))
                | _ -> pushResult (new Code(PushList []))
            | _ -> pushResult (new Code(PushList []))
            

        [<PushOperation("CONTAINS", Description = "if fst is on top of the stack, and snd right udner, returns true if the second item contains the first")>]
        static member Contains() =
            match peekStack2 Code.Me.MyType with
            // the order of the arguments is the opposite of CONTAINER
            | [aFst; aSnd] -> 
                match aFst.Raw<Push>() with
                | PushList l -> 
                        let res = Code.getContainers (aSnd.Raw<Push>()) (push l empty)
                        pushResult (new Bool (res.isEmpty))
                | _ -> pushResult (new Bool (false))
            | _ -> pushResult (new Bool (false))
 
        [<PushOperation("DEFINITION", Description = "Pushes the definition of the name on top of the NAME stack onto the code stack.")>]
        static member Definition() =
            let arg = peekStack "NAME"
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            match arg.Raw<string>() with
            | s when not (System.String.IsNullOrEmpty(s)) -> 
                match tryGetBinding s with
                | Some definition -> pushResult definition
                | None -> ()
            | _ -> ()
 
        [<PushOperation("DISCREPANCY", Description = "Pushes the measure of discrepancy between to code items on the INTEGER stack.")>]
        static member Discrepancy () =
            match peekStack2 Code.Me.MyType with
            | [a1; a2] -> pushResult (new Integer (int64 (Push.discrepancy (a1.Raw<Push>()) (a2.Raw<Push>()))))
            | _ -> ()

        [<PushOperation("DO", Description = "Pop the CODE stack & execute the top")>]
        static member Do () =
            eval Code.Me.MyType

        [<PushOperation("DO*", Description = "Peek the CODE stack & execute the top. Then pop the CODE stack.")>]
        static member DoStar () =
            evalStar Code.Me.MyType

        static member internal doRange start finish (code : Push) pushIndex=
            let next = 
                if start < finish then start + 1L
                elif start > finish then start - 1L
                else start

            if start <> finish then
                pushToExec (Value(Integer(next)))
                pushToExec (Value(Integer(finish)))
                pushToExec (Operation("CODE", stockTypes.Operations.["CODE"].["DO*RANGE"]))
                pushResult (Code(code)) // don't forget to return the code to the code stack

            if pushIndex 
            then                
                pushResult (Integer(next))
            pushToExec code


        static member internal doTimes pushIndex =
            match (processArgs1 Integer.Me.MyType), (processArgs1 Code.Me.MyType) with
            | a1, c when a1 <> Unchecked.defaultof<PushTypeBase> && c <> Unchecked.defaultof<PushTypeBase> -> 
                Code.doRange (1L - a1.Raw<int64>()) 0L (c.Raw<Push>()) pushIndex
            | _ -> ()
            
        [<PushOperation("DO*COUNT", Description = "Executes the item on top of the CODE stack recursively, the number of times is set by the INTEGER stack")>]
        static member DoCount() = 
            Code.doTimes true

        [<PushOperation("DO*TIMES", Description = "Executes the item on top of the CODE stack recursively, the number of times is set by the INTEGER stack")>]
        static member DoTimes() = 
            Code.doTimes false

        [<PushOperation("DO*RANGE", Description = "Executes the item on top of the CODE stack recursively, while iterating through the range arguments")>]
        static member DoRange() =
            match (processArgs2 Integer.Me.MyType), (processArgs1 Code.Me.MyType) with
            | [a1; a2], c when c <> Unchecked.defaultof<PushTypeBase> -> 
                Code.doRange (a1.Raw<int64>()) (a2.Raw<int64>()) (c.Raw<Push>()) true
            | _ -> ()
        