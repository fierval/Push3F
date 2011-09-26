namespace push.types.stock

[<AutoOpen>]
module StockTypesCode =
    open push.types
    open push.parser
    open push.stack
    open System.Reflection
    open System

    // emumeration used in random code generation
    type Types =
    | Const = 1
    | Name = 2
    | Code = 3
    | Max = 3

    [<PushType("CODE")>]
    type Code =
        inherit PushTypeBase

        [<DefaultValue>]val mutable private maxCodePoints : int

        new () as t = {inherit PushTypeBase ()} then t.maxCodePoints <- 20
        new (p : Push) as t = {inherit PushTypeBase(p)} then t.maxCodePoints <- 20

        static member private Me = new Code()

        override t.ToString() =
          base.ToString()

        override t.isQuotable with get () = true

        // custom parsing.
        // in this case custom parsing is disabled.
        // Push will parse these values
        override t.Parser 
            with get() = 
                Unchecked.defaultof<ExtendedTypeParser>

        static member internal pushArgsBack (args : PushTypeBase list) =
            pushResult args.Head
            pushResult args.Tail.Head

        // this function collects all of the containers ofA that are inB
        static member internal getContainers (ofA : Push) (stackOfInB : Stack<Push list>) topLevelOnly =
            let containers : Stack<Push> ref = ref empty
            let stackOfInB = ref stackOfInB
            match ofA with
            | PushList [] -> containers := push (PushList (peek !stackOfInB)) !containers; !containers
            | _ ->
                let stop = ref false
                while not (!stackOfInB).isEmpty  && not !stop do
                    let topInB = peek !stackOfInB
                    for b in topInB do
                        if b.Equals(ofA) then containers := push (PushList topInB) !containers
                        else
                            match b with
                            | PushList blist -> if blist.Length < ofA.toList.Length then () else stackOfInB := shove (!stackOfInB).length blist !stackOfInB
                            | _ -> ()
                        stop := topLevelOnly
                    stackOfInB := (snd (pop !stackOfInB))
                !containers

        [<PushOperation("APPEND", Description = "Appends two top pieces of code. Converts either one to list if necessary")>]
        static member Append() =
            match processArgs2 Code.Me.MyType with
            | [a1; a2] -> 
                let l1appendl2 = 
                    match (a1.Raw<Push>().toList), (a2.Raw<Push>().toList) with
                    | lst1, lst2 -> PushList (lst1 @ lst2)

                pushResult (Code(l1appendl2))
            |_ -> ()

        [<PushOperation("ATOM", Description = "TRUE if the top item is atomic, FALSE otherwise")>]
        static member Atom() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> then ()
            else
                pushResult (Bool(not (a.Raw<Push>().isList)))
                   
        [<PushOperation("CAR", Description = "Pushes the first item of the top of the stack. If top of the stack is an atom - no effect")>]
        [<PushOperation("FIRST", Description = "This is a more explicit name for the CAR operation")>]
        static member First() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> then ()
            else
                let arg = (processArgs1 Code.Me.MyType).Raw<Push>()
                match arg with
                | PushList l when not l.IsEmpty -> pushResult (Code(l.Head))
                | _ -> pushResult(Code(arg))

        [<PushOperation("CDR", Description = "Pushes the \"rest\" of the top of the stack. If top of the stack is an atom pushes ()")>]
        [<PushOperation("REST", Description = "This is a more explicit name for the CDR operation")>]
        static member Rest() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> || not (a.Raw<Push>().isList) then pushResult (Code(PushList []))
            else
                let arg = (processArgs1 Code.Me.MyType).Raw<Push>()
                match arg with
                | PushList l when not l.IsEmpty -> pushResult (Code(PushList l.Tail))
                | _ -> pushResult(Code(PushList []))

        [<PushOperation("CONS", Description = "if fst is on top of the stack, and snd right udner: (CONS snd fst) -> (snd fst)")>]
        static member Cons() =
            match processArgs2 Code.Me.MyType with
            | [a1; a2] -> pushResult (Code(PushList (a1.Raw<Push>() :: a2.Raw<Push>().toList)))
            | _ -> ()

        [<PushOperation("CONTAINER", Description = "Returns the container of the second item in the first")>]
        static member Container() =
            match peekStack2 Code.Me.MyType with
            | [aFst; aSnd] -> 
                match aFst.Raw<Push>() with
                | PushList l -> 
                        let res = Code.getContainers (aSnd.Raw<Push>()) (push l empty) false
                        if res.isEmpty then pushResult (Code (PushList []))
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

                            pushResult (Code(res.asList.[containerIndex]))
                | _ -> pushResult (Code(PushList []))
            | _ -> pushResult (Code(PushList []))
            

        static member isMember (aFst : PushTypeBase) (aSnd : PushTypeBase) digDeeper =
            match aFst.Raw<Push>() with
            | PushList l -> 
                    let res = Code.getContainers (aSnd.Raw<Push>()) (push l empty) (not digDeeper)
                    pushResult (Bool (not res.isEmpty))
            | _ -> pushResult (Bool (false))

        [<PushOperation("CONTAINS", Description = "Returns true if the second item contains the first")>]
        static member Contains() =
            match peekStack2 Code.Me.MyType with
            | [aFst; aSnd] -> Code.isMember aFst aSnd true
            | _ -> pushResult (Bool (false))
             
        [<PushOperation("DEFINITION", Description = "Pushes the definition of the name on top of the NAME stack onto the code stack.")>]
        static member Definition() =
            let arg = peekStack "NAME"
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            else
                match arg.Raw<string>() with
                | s when not (System.String.IsNullOrEmpty(s)) -> 
                    match tryGetBinding s with
                    | Some definition -> pushResult definition
                    | None -> ()
                | _ -> ()
 
        [<PushOperation("DISCREPANCY", Description = "Pushes the measure of discrepancy between to code items on the INTEGER stack.")>]
        static member Discrepancy () =
            match peekStack2 Code.Me.MyType with
            | [a1; a2] -> pushResult (Integer (int64 (Push.discrepancy (a1.Raw<Push>()) (a2.Raw<Push>()))))
            | _ -> ()

        static member getIndex ofBase =
            let topInt = processArgs1 Integer.Me.MyType
            match topInt.Raw<int64>() with
            | v when v = 0L -> 0
            | x ->  Math.Abs(int x) % (ofBase + 1)

        [<PushOperation("EXTRACT", Description = "Extract from the top code item a sub-item indexed by the top of INTEGER stack")>]
        static member ExtractSubItem() =
            if isEmptyStack Code.Me.MyType || isEmptyStack Integer.Me.MyType then ()
            else
                let topCode = peekStack Code.Me.MyType
                match topCode.Raw<Push>() with
                | PushList l -> 
                    let index = Code.getIndex (l.Length)
                    if index = 0 then pushResult topCode else
                        pushResult (Code(l.[index - 1]))
                | _ -> pushResult topCode


        static member toCode tp =
            if isEmptyStack tp then ()
            else
                let top = processArgs1 tp
                pushResult (Code(Value(top)))

        [<PushOperation("FROMINTEGER", Description = "Converts an INTEGER into a CODE item")>]
        static member FromInteger() =
            Code.toCode Integer.Me.MyType

        [<PushOperation("FROMBOOLEAN", Description = "Converts a BOOLEAN into a CODE item")>]
        static member FromBool() =
            Code.toCode Bool.Me.MyType

        [<PushOperation("FROMFLOAT", Description = "Converts a FLOAT into a CODE item")>]
        static member FromFloat() =
            Code.toCode Float.Me.MyType

        [<PushOperation("FROMNAME", Description = "Converts a FLOAT into a CODE item")>]
        static member FromName() =
            Code.toCode Name.Me.MyType

        [<PushOperation("INSERT", Description = "Insert the second item of the code stack at the position of the first")>]
        static member Insert() =
            let insert lst i x = 
                let rec insert lst acc = 
                    match lst with 
                    | []   -> acc 
                    | h::t -> if acc |> List.length = i then 
                                  acc @ [x] @ lst 
                              else 
                                  insert t (acc @ [h]) 
                insert lst [] 

            if isEmptyStack Integer.Me.MyType then ()
            else
                match processArgs2 Code.Me.MyType with
                | [aFst; aSnd] ->
                    let scnd, frst = aSnd.Raw<Push>(), aFst.Raw<Push>()
                    match scnd, frst with
                    | (_, PushList top) -> 
                        let index = Code.getIndex (top.Length)
                        if index = 0 then pushResult aSnd
                        else
                            let index = index - 1
                            let newTop = PushList(insert top index scnd)
                                                
                            pushResult (Code(newTop))
                    | _ -> pushResult aSnd
                | _ -> ()    
             
        [<PushOperation("INSTRUCTIONS", Description = "Pushes a list of all active instructions")>]
        static member Instructions() =
            stockTypes.Operations 
            |> Map.iter
                (fun key m -> 
                            m 
                            |> Map.iter (fun key elem -> pushResult (Code(Operation(key, elem)))))

        [<PushOperation("LENGTH", Description = "Pushes the length of the top item")>]
        static member Length() =
            if isEmptyStack Code.Me.MyType then pushResult (Integer(0L))
            else
                match (peekStack Code.Me.MyType).Raw<Push>() with
                | PushList l -> pushResult (Integer (int64 (l.Length)))
                | _ -> pushResult (Integer (1L))

        [<PushOperation("LIST", Description = "Makes a list out of the first two stack items")>]
        static member MakeList() =
            match processArgs2 Code.Me.MyType with
            | [aFst; aSnd] -> pushResult (Code(PushList[aFst.Raw<Push>(); aSnd.Raw<Push>()]))
            | _ -> ()

        [<PushOperation("MEMBER", Description = "Pushes TRUE if the second is the member of the first")>]
        static member Member() =
            match peekStack2 Code.Me.MyType with
            | [aFst; aSnd] -> Code.isMember aSnd aFst false
            | _ -> pushResult (Bool (false))

        [<PushOperation("NOOP", Description = "Does nothing")>]
        static member Noop() =
            ()

        [<PushOperation("NTH", Description = "Pushes the n-th member of the top element onto the stack")>]
        static member Nth() =
            if isEmptyStack Integer.Me.MyType then ()
            else          
                match peekStack Code.Me.MyType with
                | aTop when aTop <> Unchecked.defaultof<PushTypeBase> ->
                    let top = aTop.Raw<Push>()
                    match top with
                    | PushList top -> 
                        let index = Code.getIndex (top.Length)
                        if index = 0 then pushResult aTop
                        else
                            pushResult (Code(top.[index - 1]))
                    | _ -> pushResult (Code(PushList([aTop.Raw<Push>()]))) // coerce the expression to the list

                | _ -> ()

        [<PushOperation("NTHCDR", Description = "Pushes the nth \"rest\" of the top of the stack. If top of the stack is an atom pushes ()")>]
        [<PushOperation("NTHREST", Description = "This is a more explicit name for the NTHCDR operation")>]
        static member NthRest() =
            let a = peekStack Code.Me.MyType
            if a = Unchecked.defaultof<PushTypeBase> then pushResult (Code(PushList []))
            else
                let arg = (processArgs1 Code.Me.MyType).Raw<Push>()

                match arg with
                | PushList l ->
                    if isEmptyStack Integer.Me.MyType then ()
                    else 
                        let index = Code.getIndex l.Length
                        if index = 0 then pushResult a
                        else
                            let lst = [for i = index to l.Length - 1 do yield l.[i]]
                            pushResult (Code(PushList(lst)))

                | _ -> pushResult(Code(PushList []))

        [<PushOperation("NULL", Description = "Pushes TRUE into the BOOLEAN stack if the top code item is an empty list. FALSE otherwise")>]
        static member Null() =
            match peekStack Code.Me.MyType with
            | a when a <> Unchecked.defaultof<PushTypeBase> ->
                match a.Raw<Push>() with
                | PushList [] -> pushResult (Bool(true))
                | _ -> pushResult (Bool(false))
            | _ -> pushResult (Bool(false))

        [<PushOperation("POSITION", Description = "Pushes a position of the second item within the first items on top of the integer stack. -1 if no occurences")>]
        static member Position() =
            match peekStack2 Code.Me.MyType with
                | [aFst; aSnd] -> 
                    match aFst.Raw<Push>() with
                    | PushList l -> 
                        let scnd = aSnd.Raw<Push>()
                        match l |> List.tryFindIndex (fun e -> e = scnd) with
                        | Some i -> pushResult (Integer (int64 i))
                        | _ -> pushResult (Integer (-1L))
                    | _ -> pushResult (Integer (-1L))
                | _ -> pushResult (Integer (-1L))
                        
        [<PushOperation("QUOTE", Description = "Pushes top of the EXEC stack to the CODE stack")>]
        static member Quote (context:string) (tp : string) =
            if isEmptyStack context then ()
            else
                Code((processArgs1 context).Raw<Push>()) |> pushResult

        [<PushOperation("SIZE", Description = "Pushes the number of 'points' to the integer stack.")>]
        static member Size() =
            match peekStack Code.Me.MyType with
            |a when a <> Unchecked.defaultof<PushTypeBase> ->
                match a.Raw<Push>() with
                | PushList l -> pushResult (Integer(int64 (l.Length)))
                | _ -> pushResult (Integer(0L))
            | _ -> pushResult (Integer(0L))
        
        [<PushOperation("SUBST", Description = "Lisp \"subst\" function. Not implemented")>]
        static member Subst() = Code.Noop()

        // random code generation
        [<PushOperation("RAND", Description = "Generates random code")>]
        static member Rand() = 
            let initRandom = Random(int DateTime.UtcNow.Ticks)

            // given a choice, generate a random name, random code, random integer
            let pickRandomConst = 
                let randomType = initRandom.Next(1, int Types.Max)
            
                let keyFromIndex index map = (map |> Map.toList).[index]

                // generate a random operation
                match enum<Types>(randomType) with 
                | Types.Code | Types.Max -> 
                    let indexTypes = initRandom.Next(0, stockTypes.Operations.Count)
                    let typeName = fst (stockTypes.Operations |> keyFromIndex indexTypes)
                    let indexOps = initRandom.Next(0, (stockTypes.Operations.[typeName].Count))
                    let op = (stockTypes.Operations.[typeName] |> keyFromIndex indexOps)
                    Operation(fst op, snd op)

                // generate a random constant
                | Types.Const -> 
                    match initRandom.Next(0, 3) with
                    | 0 -> Value(Integer(int64(initRandom.Next())))
                    | 1 -> Value (Float(initRandom.NextDouble()))
                    | 2 -> Value (Bool(initRandom.Next(0, 2) = 0))
                    | _ -> failwith "cannot happen"
                    
                // either generates a random name or gets a random definition
                | Types.Name -> 
                    if stockTypes.Bindings.IsEmpty then Value(Name (Name.GetRandomString 15)) 
                    else
                        let index = initRandom.Next(0, stockTypes.Bindings.Count - 1)
                        let key = fst (stockTypes.Bindings |> keyFromIndex index)
                        stockTypes.Bindings.[key].Raw<Push>()
                | _ -> failwith "this is never reached"

            // gets a list or random values
            let rec decompose num maxParts acc =
                if num = 1 || maxParts = 1 then PushList ((Value(Integer(1L)))::acc)
                else
                    let thisPart = initRandom.Next(1, num - 1)
                    decompose (num - thisPart) (maxParts - 1) (Value(Integer(int64 thisPart))::acc)

            let rec inputCodeMaxSize points acc =
                if points = 1 then pickRandomConst :: acc
                    else
                        let sizesThisLevel = (decompose (points - 1) (points - 1) List.empty).toList |> 
                                                List.map(fun e -> 
                                                            match e with 
                                                            |Value v -> v.Raw<int64>()
                                                            | _ -> failwith "cannot happen")
                        [
                            for i in sizesThisLevel do
                                yield! inputCodeMaxSize (int i) acc
                        ] 
            
            let points = initRandom.Next(1, Code.Me.maxCodePoints)

            let res = inputCodeMaxSize points List.empty

            pushResult (Code(PushList res))
