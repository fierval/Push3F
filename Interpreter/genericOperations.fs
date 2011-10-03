namespace push.types

open System.Reflection
open push.exceptions
open push.stack
open push.parser
open push.types.stock
open System
open System.IO

[<AutoOpen>]
module GenericOperations =

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
            if areAllStacksNonEmpty ["NAME"; tp] && (tp <> "NAME" || stockTypes.Stacks.["NAME"].length > 1)
            then
                let name = (processArgs1 "NAME").Raw<string>()
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
            if isEmptyStack tp || isEmptyStack "INTEGER" then ()
            else
                let arg = Ops.getIntArgument tp
                let argVal = int (arg.Raw<int64>())
                if argVal >= 0 && stockTypes.Stacks.[tp].length >= argVal
                then
                    let value = processArgs1 tp
                    match argVal with
                    | 0 -> pushResult value
                    | _ -> 
                        Ops.Replace tp (shove argVal value stockTypes.Stacks.[tp])
                else //restore the state of the integer stack
                    pushResult arg

        [<GenericPushOperation("SWAP", Description = "Swaps two top values of the stack")>]
        static member swap tp =
            let args = processArgs2 tp
            if args.Length < 2 then ()
            else
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
            else
                let newStack = yank (int (arg.Raw<int64>())) stockTypes.Stacks.[tp]
                Ops.Replace tp newStack 

        [<GenericPushOperation("YANKDUP", Description = "Yanks an item out of the stack and pushes it on top of the stack")>]
        static member yankdup tp =
            if isEmptyStack Integer.Me.MyType then () 
            else
                let arg = Ops.getIntArgument tp
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
            //things are different for CODE and EXEC:
            //(5 3 INTEGER.< EXEC.IF 1 0) - should push the top item of the EXEC to the INTEGER stack
            // while (5 3 INTEGER.< CODE.QUOTE 1 CODE.QUOTE 0 CODE.IF) should push the second item of 
            // the CODE stack in order to conform to the "readability" of programs. 
            if isEmptyStack Bool.Me.MyType then ()
            else
                match processArgs2 tp with
                | [a1; a2] ->
                    let isCodeStack = tp = "CODE"
                    let isTrue = (processArgs1 Bool.Me.MyType).Raw<bool>()
                    let a1 = a1.Raw<Push>()
                    let a2 = a2.Raw<Push>()
                    match (isCodeStack, isTrue) with
                    | (true, true) | (false, false) -> pushToExec a1
                    | (true, false) | (false, true) -> pushToExec a2
                | _ -> ()


        static member private pushCodeToExec =
            let code = processArgs1 "CODE"
            pushToExec (code.Raw<Push>())
 
        [<GenericPushOperation("DO", Description = "Pop the CODE stack & execute the top", AppliesTo=[|"EXEC"; "CODE"|])>]
        static member Do tp =
            if not (isEmptyStack tp) then
                if tp = "CODE" then
                    Ops.pushCodeToExec

        [<GenericPushOperation("DO*", Description = "Peek the CODE stack & execute the top. Then pop the CODE stack.", AppliesTo=[|"EXEC"; "CODE"|])>]
        static member DoStar tp =
            if not (isEmptyStack tp) then
                Ops.dup tp
                if tp = "CODE" then
                    Ops.pushCodeToExec

        static member internal doRange start finish (code : PushTypeBase) =
            let next = 
                if start < finish then start + 1L
                elif start > finish then start - 1L
                else start

            if start <> finish then
                // push the code first:
                // if it goes to the CODE stack - the order makes no difference.
                // if it goes to the EXEC stack - it should be preserved before other stuff
                // is pushed on that stack.
                pushResult code
                pushToExec (Operation(code.MyType, stockTypes.Operations.[code.MyType].["DO*RANGE"]))
                pushToExec (Value(Integer(finish)))
                pushToExec (Value(Integer(next)))

            pushResult (Integer(start))
            pushToExec (code.Raw<Push>())

        static member internal doTimes pushIndex tp =
            // creates a new item consiting of the code item preceded by INTEGER.POP
            let concatPopIntegerAndCode (code : #PushTypeBase) =
                fst (createPushObject (stockTypes.Types.[code.MyType].GetType())
                    [|PushList((Operation(Integer.Me.MyType, stockTypes.Operations.[Integer.Me.MyType].["POP"])::code.Raw<Push>().toList))|])

            if areAllStacksNonEmpty [tp; Integer.Me.MyType] 
            then
                match (processArgs1 Integer.Me.MyType).Raw<int64>(), (processArgs1 tp) with
                | a1, code -> 
                        if not pushIndex then
                            Ops.doRange (1L - a1) 0L (concatPopIntegerAndCode code)
                        else
                            Ops.doRange (1L - a1) 0L code
            
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
            if (peekStack2 Integer.Me.MyType).Length < 2 || isEmptyStack tp then ()
            else
                match (processArgs2 Integer.Me.MyType), (processArgs1 tp) with
                | [a1; a2], code ->
                    let start = a1.Raw<int64>()
                    let finish = a2.Raw<int64>() 
                    Ops.doRange start finish code
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
                stockTypes.Stacks.[tp] |> Seq.iter (fun e -> Console.WriteLine(e))
            Console.WriteLine()

        [<GenericPushOperation("OPEN", 
            Description= "Opens a file, the name of which is given by the top of the LITERAL stack, pushes the result to the EXEC (or CODE) stack.",
            AppliesTo=[|"EXEC"; "CODE"|])>]
        static member OpenFile (tp : string) =
            if not (isEmptyStack "LITERAL") then
                let file = (processArgs1 "LITERAL").Raw<string>()
                if File.Exists (file) then
                    try
                        pushResult (createPushObjectOfType tp [|Push.Deserialize(file)|])
                    with
                    | e -> ()


        [<GenericPushOperation("SAVE", 
            Description = "Saves top of the EXEC (or CODE) stack in a file",
            AppliesTo=[|"EXEC"; "CODE"|])>]
        static member SaveFile (tp : string) =
            if areAllStacksNonEmpty ["LITERAL"; tp] then
                let file = (processArgs1 "LITERAL").Raw<string>()
                try
                    (processArgs1 tp).Raw<Push>().Serialize(file)
                with
                | e -> ()
