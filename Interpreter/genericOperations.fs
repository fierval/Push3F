namespace push.types

[<AutoOpen>]
module GenericOperations =

    open System.Reflection
    open push.exceptions
    open push.stack
    open push.parser
    open push.types.stock
    open System

    type Ops ()=

        static member private Replace key value =
            stockTypes.Stacks <- stockTypes.Stacks.Replace(key, value)

        static member private getIntArgument tp =
            processArgs1 Integer.Me.MyType
          
        [<GenericPushOperation("FLUSH", Description = "Empties the designated stack")>]
        static member flush tp =
            stockTypes.Stacks <- stockTypes.Stacks.Replace(tp, empty)
        
        [<GenericPushOperation("DEFINE", Description = "Binds the name to the current top of the designated stack")>]
        static member define tp =
            let stack = stockTypes.Stacks.["NAME"]
            if stack.length = 0 then ()
            else
                let name = (peek stack).Value :?> string
                let value = processArgs1 tp
                match stockTypes.Bindings.TryFind(name) with
                | Some v -> stockTypes.Bindings <- stockTypes.Bindings.Replace(name, value)
                | None -> stockTypes.Bindings <- stockTypes.Bindings.Add(name, value)

        [<GenericPushOperation("DUP", Description = "Pushes the duplicate of the top of the stack")>]
        static member dup tp =
            if stockTypes.Stacks.[tp].length = 0 then ()
            else stockTypes.pushResult (peek stockTypes.Stacks.[tp])

        [<GenericPushOperation("POP", Description = "Pops the top of the stack")>]
        static member pop tp =
            processArgs1 tp |> ignore

        [<GenericPushOperation("ROT", Description = "Rotates 3 top stack entries")>]
        static member rot tp =
            pushResult (new StockTypesInteger.Integer(2L))
            Ops.yank tp

        [<GenericPushOperation("SHOVE", Description = "Pushes the top of the stack deep into the stack. The depth is designated by the top of INTEGER stack")>]
        static member shove tp =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let value = processArgs1 tp
            if value = Unchecked.defaultof<PushTypeBase> then ()
            Ops.Replace tp (shove (int (arg.Raw<int64>())) value stockTypes.Stacks.[tp])

        [<GenericPushOperation("SWAP", Description = "Swaps two top values of the stack")>]
        static member swap tp =
            let args = processArgs2 tp
            if args.Length < 2 then ()
            let hd, tl = args.Head, (args.Tail).Head
            stockTypes.pushResult tl
            stockTypes.pushResult hd

        [<GenericPushOperation("STACKDEPTH", Description = "Pushes the number of items of the stack on top of the INTEGER stack.")>]
        static member stackdepth tp =
            let preLen = int64 (stockTypes.Stacks.[tp].length)
            let len = 
                match tp with
                | "INTEGER" ->  preLen + 1L
                | _ -> preLen

            stockTypes.pushResult (new StockTypesInteger.Integer(len))

        [<GenericPushOperation("YANK", Description = "Tears an item out of the stack. The index is designated by the top of the INTEGER stack")>]
        static member yank tp =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let newStack = yank (int (arg.Raw<int64>())) stockTypes.Stacks.[tp]
            Ops.Replace tp newStack 

        [<GenericPushOperation("YANKDUP", Description = "Yanks an item out of the stack and pushes it on top of the stack")>]
        static member yankdup tp =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let newStack = yankdup (int (arg.Raw<int64>())) stockTypes.Stacks.[tp]
            Ops.Replace tp newStack
        
        [<GenericPushOperation("=", Description = "Compares two top pieces of code", AppliesTo=[|"EXEC"; "CODE"|])>]
        static member Eq tp = 
            match processArgs2 tp with
            | [a1; a2] -> 
                pushResult (Bool (a1.Raw<Push>() = a2.Raw<Push>()))
            | _ -> ()

        [<GenericPushOperation("IF", Description = "Execute either the first or the second item on top of the code stack", AppliesTo=[|"CODE"; "EXEC"|])>]
        static member If tp =
            if isEmptyStack Bool.Me.MyType then ()
            match processArgs2 tp with
            | [a1; a2] ->
                if not ((processArgs1 Bool.Me.MyType).Raw<bool>())
                then 
                    pushToExec (a2.Raw<Push>())
                    pushResult a1
                else
                    pushToExec (a1.Raw<Push>())
                    pushResult a2
            | _ -> ()

 
        [<GenericPushOperation("DO", Description = "Pop the CODE stack & execute the top", AppliesTo=[|"EXEC"; "CODE"|])>]
        static member Do tp =
            eval tp (tp = "CODE")

        [<GenericPushOperation("DO*", Description = "Peek the CODE stack & execute the top. Then pop the CODE stack.", AppliesTo=[|"EXEC"; "CODE"|])>]
        static member DoStar tp =
            evalStar tp (tp = "CODE")

        static member internal doRange start finish (code : Push) pushIndex=
            let next = 
                if start < finish then start + 1L
                elif start > finish then start - 1L
                else start

            if start <> finish then
                pushToExec (Value(Integer(next)))
                pushToExec (Value(Integer(finish)))
                pushToExec (Operation("CODE", stockTypes.Operations.["CODE"].["DO*RANGE"]))

            if pushIndex 
            then                
                pushResult (Integer(next))
            pushToExec code

        static member internal doTimes pushIndex tp =
            match (processArgs1 Integer.Me.MyType), (peekStack tp) with
            | a1, c when a1 <> Unchecked.defaultof<PushTypeBase> && c <> Unchecked.defaultof<PushTypeBase> -> 
                Ops.doRange (1L - a1.Raw<int64>()) 0L (c.Raw<Push>()) pushIndex
            | _ -> ()
            
        [<GenericPushOperation("DO*COUNT", 
            Description = "Executes the item on top of the CODE stack recursively, the number of times is set by the INTEGER stack", 
            AppliesTo=[|"EXEC"; "CODE"|])>]
        static member DoCount tp = 
            Ops.doTimes true tp

        [<GenericPushOperation("DO*TIMES", 
            Description = "Executes the item on top of the CODE stack recursively, the number of times is set by the INTEGER stack", 
            AppliesTo=[|"EXEC"; "CODE"|])>]
        static member DoTimes tp = 
            Ops.doTimes false tp

        [<GenericPushOperation("DO*RANGE", 
            Description = "Executes the item on top of the CODE stack recursively, while iterating through the range arguments", 
            AppliesTo=[|"EXEC"; "CODE"|])>]
        static member DoRange tp =
            match (processArgs2 Integer.Me.MyType), (peekStack tp) with
            | [a1; a2], c when c <> Unchecked.defaultof<PushTypeBase> -> 
                Ops.doRange (a1.Raw<int64>()) (a2.Raw<int64>()) (c.Raw<Push>()) true
            | _ -> ()

        [<GenericPushOperation("WRITE", Description="Write the top value out to the console")>]
        static member Write tp =
            if isEmptyStack tp then ()
            else
                Console.WriteLine(peekStack tp)

        [<GenericPushOperation("DUMPSTACK", Description="")>]
        static member DumpStack (tp : string) =
            Console.WriteLine(tp)
            Console.WriteLine("-------------")
            if isEmptyStack tp then ()
            else
                stockTypes.Stacks.[tp].asList |> List.iter (fun e -> Console.WriteLine(e))
