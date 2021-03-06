﻿namespace push.types.stock

open push.types
open push.parser
open push.stack
open System.Reflection
open System

// emumeration used in random code generation
type Types =
| Const = 1
| Code = 2
| Max = 2

[<PushType("CODE")>]
type Code =
    inherit PushTypeBase

    [<DefaultValue>] val mutable private maxCodePoints : int
    [<DefaultValue>] val mutable setMaxCodePoints : Event<unit>

    new () as t= {inherit PushTypeBase ()} then t.setMaxCodePoints <- new Event<unit>()
    new (p : Push) as t = {inherit PushTypeBase(p)} then t.setMaxCodePoints <- new Event<unit>() 

    static member Me = new Code()
    static member simpleOp (f : Push -> Push -> Push) = simpleOp f Code.Me.MyType

    override t.ToString() =
        base.ToString()

    override t.isQuotable with get () = true

    // custom parsing.
    // in this case custom parsing is disabled.
    // Push will parse these values
    override t.Parser 
        with get() = 
            Unchecked.defaultof<ExtendedTypeParser>

    member t.SetMaxCodePoints = t.setMaxCodePoints.Publish

    member t.MaxCodePoints 
        with get () = 
            if (t.maxCodePoints = Unchecked.defaultof<int>) 
            then 
                let maxCodePoints = t.setMaxCodePoints.Trigger()  // try to retrieve the number of code points from somebody else
                if (t.maxCodePoints = Unchecked.defaultof<int>) then
                    t.maxCodePoints <- 200
            t.maxCodePoints 
        and set value = t.maxCodePoints <- value

    static member internal pushArgsBack (args : PushTypeBase list) =
        pushResult args.Head
        pushResult args.Tail.Head

    // this function collects all of the containers ofA that are inB
    static member internal getContainers (ofA : Push) (stackOfInB : Stack<Push list>) topLevelOnly =
        let containers : Stack<Push> ref = ref empty
        let stackOfInB = ref stackOfInB
        match ofA with
        | PushList [] -> containers := pushStack (PushList (peek !stackOfInB)) !containers; !containers
        | _ ->
            let stop = ref false
            while not (!stackOfInB).isEmpty  && not !stop do
                let topInB = peek !stackOfInB
                for b in topInB do
                    if b.Equals(ofA) then containers := pushStack (PushList topInB) !containers
                    else
                        match b with
                        | PushList blist -> if blist.Length < ofA.toList.Length then () else stackOfInB := shove (!stackOfInB).length blist !stackOfInB
                        | _ -> ()
                    stop := topLevelOnly
                stackOfInB := (snd (pop !stackOfInB))
            !containers

    [<PushOperation("APPEND", Description = "Appends two top pieces of code. Converts either one to list if necessary")>]
    static member Append() =
        Code.simpleOp (fun a b -> PushList(a.toList @ b.toList))

    [<PushOperation("ATOM", Description = "TRUE if the top item is atomic, FALSE otherwise")>]
    static member Atom() =
        monoOp (fun (a : Push) -> not a.isList) Code.Me.MyType Bool.Me.MyType
                   
    [<PushOperation("CAR", Description = "Pushes the first item of the top of the stack. If top of the stack is an atom - no effect")>]
    [<PushOperation("FIRST", Description = "This is a more explicit name for the CAR operation")>]
    static member First() =
        push {
            let! code = popOne<Push> Code.Me.MyType
            if code.isList && not code.toList.IsEmpty then
                return! result Code.Me.MyType code.toList.Head
        }


    [<PushOperation("CDR", Description = "Pushes the \"rest\" of the top of the stack. If top of the stack is an atom pushes ()")>]
    [<PushOperation("REST", Description = "This is a more explicit name for the CDR operation")>]
    static member Rest() =
        push {
            let! code = popOne<Push> Code.Me.MyType
            if code.isList && not code.toList.IsEmpty then
                return! result Code.Me.MyType (PushList(code.toList.Tail))
        }

    [<PushOperation("CONS", Description = "if fst is on top of the stack, and snd right udner: (CONS snd fst) -> (snd fst)")>]
    static member Cons() =
        Code.simpleOp(fun a b -> PushList(a::b.toList))

    [<PushOperation("CONTAINER", Description = "Returns the container of the second item in the first")>]
    static member Container() =
        push {
            let! aSnd = popOne<Push> Code.Me.MyType
            let! aFst = popOne<Push> Code.Me.MyType
            if aFst.isList then
                let res = Code.getContainers aSnd (pushStack aFst.toList empty) false
                if res.isEmpty then return! result Code.Me.MyType (PushList [])
                else
                    let containerIndex = 
                        res.asList 
                        |> 
                        List.mapi (fun i e -> 
                            match e with 
                            | PushList l -> i, l.Length
                            | _ -> i, -1) 
                        |> List.minBy (fun (index, value) -> value) 
                        |> fst

                    return! result Code.Me.MyType (res.asList.[containerIndex]) 
        }
            

    static member isMember (aFst : Push) (aSnd : Push) digDeeper =
        match aFst with
        | PushList l -> 
                let res = Code.getContainers aSnd (pushStack l empty) (not digDeeper)
                not res.isEmpty
        | _ -> false

    [<PushOperation("CONTAINS", Description = "Returns true if the second item contains the first")>]
    static member Contains() =
        dualOp (fun a b -> Code.isMember a b true) Code.Me.MyType Bool.Me.MyType
             
    [<PushOperation("DEFINITION", Description = "Pushes the definition of the name on top of the NAME stack onto the code stack.")>]
    static member Definition() =
        push {
            let! name = popOne "NAME"
            match tryGetBinding name with
            | Some definition -> pushResult definition
            | None -> ()
            return! zero
        }

    [<PushOperation("DISCREPANCY", Description = "Pushes the measure of discrepancy between to code items on the INTEGER stack.")>]
    static member Discrepancy () =
        dualOp (fun (a : Push) (b : Push) -> Push.discrepancy a b) Code.Me.MyType Integer.Me.MyType

            
    static member getIndex (index : int64) ofBase =
        int (Math.Abs(index) % ofBase)

    [<PushOperation("DO", Description = "Pop the CODE stack & execute the top")>]
    static member Do () =
        monoOp (fun (code : Push) -> code) Code.Me.MyType "EXEC"

    [<PushOperation("DO*", Description = "Peek the CODE stack & execute the top. Then pop the CODE stack.")>]
    static member DoStar () =
        push {
            let! code = peekOne<Push> Code.Me.MyType
            let pop = makeOperation Code.Me.MyType "POP"
            return! result "EXEC" (PushList[code; pop])
        }

    static member extractShallow (code : Push) (index : int) =
        match code with
        | PushList l -> 
            let index = Code.getIndex (int64 index) (int64 l.Length)
            if index = 0 then code else l.[index - 1]
        | _ -> code
            

    [<PushOperation("EXTRACT", Description = "Extract from the top code item a sub-item indexed by the top of INTEGER stack")>]
    static member ExtractSubItem() =
        push {
            if not (isEmptyStack Integer.Me.MyType) then 
                let! topCode = popOne Code.Me.MyType
                let! index = popOne Integer.Me.MyType
                let res = 
                    match topCode with
                    | PushList l -> 
                        let index = Code.getIndex index (Code.getSize (PushList(l)))
                        Code.extract topCode index
                    | _ -> topCode
                return! result Code.Me.MyType res
        }


    static member toCode tp =
        if not (isEmptyStack tp) then 
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

    static member extract (code : Push) i =
        if i = 0 || List.isEmpty code.toList then code
        else
            let index = ref (i - 1)
            let rec extractInDepth (tail : Push list) =
                if !index = 0 && not tail.IsEmpty then List.head tail
                else
                    if List.isEmpty tail then PushList([])
                    else
                        index := !index - 1
                        let next = List.head tail
                        let tl = List.tail tail
                        match next with
                        | PushList l -> 
                            let res = extractInDepth l
                            if List.isEmpty res.toList then extractInDepth tl else res
                            //if we have reached the end of the list - just pop.
                        | _ -> extractInDepth tl


            extractInDepth code.toList

    static member internal makePushList3 l1 (l2 : Push option) l3 =
        match l2 with
        | Some p -> l1 @ p.toList @ l3
        | None -> l1 @ l3

    static member internal traverse (code : Push, i, ?x : Push) =
        let index = ref i
        let rec traverseCode (head : Push list) (tail : Push list) (code : Push option) =
            let tl = if tail.Length > 0 then List.tail tail else tail
            if !index = 0 
            then 
                match code with
                | Some c -> if head.IsEmpty && tail.IsEmpty then c.toList else head  @ c :: tl
                | _ -> head @ tl 
            else
                index := !index - 1
                if List.isEmpty tail then head
                else
                    let next = List.head tail
                    match next with
                    | PushList l -> 
                        let res = traverseCode List.empty l code
                        if !index > 0 then
                            Code.makePushList3 head (Some (PushList(res)))  (traverseCode List.empty tl code)
                            else 
                               head @ PushList(res) :: tl
                    | _ -> traverseCode (head @ next.toList) tl code
        
        traverseCode [] code.toList x
        
    static member insert (lst : Push) i x = 
        Code.traverse (lst, i,  x)

    static member remove (lst : Push) i =
        PushList(Code.traverse (lst, i))

    [<PushOperation("INSERT", Description = "Insert the second item of the code stack at the position of the first")>]
    static member Insert() =
         
        push {
            let! scnd = popOne Code.Me.MyType
            let! frst = popOne Code.Me.MyType
            let! index = popOne Integer.Me.MyType
            let res = 
                match scnd, frst with
                | (_, PushList top) -> 
                    let index = Code.getIndex index (Code.getSize (PushList(top)))
                    if index = 0 then scnd
                    else PushList(Code.insert frst (index - 1) scnd)
                | _ -> scnd
            return! result Code.Me.MyType res
        }
             
    [<PushOperation("INSTRUCTIONS", Description = "Pushes a list of all active instructions")>]
    static member Instructions() =
        stockTypes.Operations 
        |> Map.iter
            (fun key m -> 
                        m 
                        |> Map.iter (fun k elem -> pushResult (Code(Operation(key, elem)))))

    [<PushOperation("LENGTH", Description = "Pushes the length of the top item")>]
    static member Length() =
        push {
            let! top = popOne Code.Me.MyType
            let res =match top with
                        | PushList l -> int64 (l.Length)
                        | _ -> 1L
            return! result Integer.Me.MyType res
        }

    [<PushOperation("LIST", Description = "Makes a list out of the first two stack items")>]
    static member MakeList() =
        Code.simpleOp (fun a b -> PushList [a ; b])

    [<PushOperation("MEMBER", Description = "Pushes TRUE if the second is the member of the first")>]
    static member Member() =
        dualOp (fun a b -> Code.isMember a b false) Code.Me.MyType Bool.Me.MyType

    [<PushOperation("NOOP", Description = "Does nothing")>]
    static member Noop() =
        ()

    static member private getNth index (code : Push list) =
        if code.Length = 0 then PushList(code)
        else
            let index = Code.getIndex index (int64 code.Length)
            code.[index]

    [<PushOperation("NTH", Description = "Pushes the n-th member of the top element onto the stack")>]
    static member Nth() =
        push {
            let! index = popOne Integer.Me.MyType
            let! code = popOne<Push> Code.Me.MyType
            return! result Code.Me.MyType (Code.getNth index (code.toList))
        }

    [<PushOperation("NTHCDR", Description = "Pushes the nth \"rest\" of the top of the stack. If top of the stack is an atom pushes ()")>]
    [<PushOperation("NTHREST", Description = "This is a more explicit name for the NTHCDR operation")>]
    static member NthRest() =
        push {
            let! index = popOne Integer.Me.MyType
            let! arg = popOne Code.Me.MyType
            match arg with
            | PushList l ->
                    match l with
                    | hd::tl -> return! result Code.Me.MyType (Code.getNth index tl)
                    | _ -> return! result Code.Me.MyType (Code.getNth index l)

            | _ -> return! result Code.Me.MyType (PushList [])
        }

    [<PushOperation("NULL", Description = "Pushes TRUE into the BOOLEAN stack if the top code item is an empty list. FALSE otherwise")>]
    static member Null() =
        monoOp (fun (a : Push) -> a.isList && a.toList.IsEmpty) Code.Me.MyType Bool.Me.MyType

    [<PushOperation("POSITION", Description = "Pushes a position of the second item within the first items on top of the integer stack. -1 if no occurences")>]
    static member Position() =
        push {
            let! aSnd = popOne Code.Me.MyType
            let! aFst = popOne Code.Me.MyType : PushMonad<Push>
            if aFst.isList then
                let res =
                    match aFst.toList |> List.tryFindIndex (fun e -> e = aSnd) with
                        | Some i -> int64 i
                        | _ -> -1L
                return! result Integer.Me.MyType res
        }
                        
    [<PushOperation("QUOTE", Description = "Pushes top of the EXEC stack to the CODE stack")>]
    static member Quote () =
        monoOp (fun (e : Push) -> e) "EXEC" Code.Me.MyType

    static member getSize (top : Push) =
        let rec getSize (code : Push) =            
            match code with
            | PushList l -> 
                match l with
                | [] -> 1L
                | hd::tl -> getSize(hd) + getSize (PushList(tl))
            | _ -> 1L
        getSize top 

    [<PushOperation("SIZE", Description = "Pushes the number of 'points' to the integer stack.")>]
    static member Size() =

        monoOp Code.getSize Code.Me.MyType Integer.Me.MyType

        
    [<PushOperation("SUBST", Description = "Lisp \"subst\" function. Not implemented")>]
    static member Subst() = Code.Noop()
    
    static member rand (maxPoints, ?bias : string) =
        let initRandom = Type.Random
        let pickOpsFrom = ["EXEC"; "CODE"; "NAME"]
        let biasedOps =
            match bias with
            | Some s -> s::pickOpsFrom
            | _ -> pickOpsFrom

        // given a choice, generate a random name, random code, random const
        let pickRandomConst () = 
            let randomOpsSet = 
                match bias with
                | Some s -> stockTypes.RandomOps |> Map.filter(fun key value -> List.exists (fun e -> key = e ) biasedOps)
                | None -> stockTypes.RandomOps
            let keyFromIndex index map = (map |> Map.toList).[index]

            // generate a random operation
            let indexTypes = initRandom.Next(0, randomOpsSet.Count)
            let typeName = fst (randomOpsSet |> keyFromIndex indexTypes)
            let indexOps = initRandom.Next(0, (randomOpsSet.[typeName].Count))
            let op = snd (randomOpsSet.[typeName] |> keyFromIndex indexOps)
            Operation(typeName, op)


        // gets a list or random values
        let rec decompose num maxParts acc =
            if num = 1 || maxParts = 1 then 1::acc
            else
                let thisPart = initRandom.Next(1, num)
                decompose (num - thisPart) maxParts (thisPart::acc)

        let rec inputCodeMaxSize points =
            if points = 1 then pickRandomConst ()
                else
                    let sizesThisLevel = decompose (points - 1) (points - 1) List.empty
                    PushList(sizesThisLevel |> List.map(fun e -> inputCodeMaxSize e))
        
        inputCodeMaxSize maxPoints

    // random code generation
    [<PushOperation("RAND", Description = "Generates random code")>]
    static member Rand() = 
            
        let points = Type.Random.Next(1, Code.Me.MaxCodePoints)

        pushResult (Code (Code.rand points))
