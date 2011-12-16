namespace push.types

open System.Reflection
open push.exceptions
open push.stack
open push.parser
open push.types.stock
open push.types
open System
open System.IO

[<AutoOpen>]
module GenericOperations =

    [<GenericPushType>]
    type Ops ()=
        static member private Replace key value =
            stockTypes.Stacks <- stockTypes.Stacks.Replace(key, value)

        static member private getIntArgument tp =
            processArgs1 Integer.Me.MyType

        [<GenericPushOperation("<", AppliesTo=[|"INTEGER"; "FLOAT"|])>]
        static member Lt tp =
            dualOp (<) tp Bool.Me.MyType

        [<GenericPushOperation(">", AppliesTo=[|"INTEGER"; "FLOAT"|])>]
        static member Gt tp =
            dualOp (>) tp Bool.Me.MyType

        [<GenericPushOperation("=")>]
        static member Eq tp =
            dualOp (=) tp Bool.Me.MyType
          
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
            else 
                stockTypes.pushResult (peek stockTypes.Stacks.[tp])

        [<GenericPushOperation("POP", Description = "Pops the top of the stack")>]
        static member pop tp =
            processArgs1 tp |> ignore

        [<GenericPushOperation("ROT", Description = "Rotates 3 top stack entries")>]
        static member rot tp =
            pushResult (new Integer(2L))
            Ops.yank tp

        [<GenericPushOperation("SHOVE", Description = "Pushes the top of the stack deep into the stack. The depth is designated by the top of INTEGER stack")>]
        static member shove tp =
            if (isEmptyStack tp || (tp <> "INTEGER" && (isEmptyStack "INTEGER"))
                || (stockTypes.Stacks.["INTEGER"].length <= 1)) then ()
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
            match processArgs2 tp with
            | [a1; a2] ->
                stockTypes.pushResult a2
                stockTypes.pushResult a1
            | _ -> ()

        [<GenericPushOperation("STACKDEPTH", Description = "Pushes the number of items of the stack on top of the INTEGER stack.")>]
        static member stackdepth tp =
            let len = int64 (stockTypes.Stacks.[tp].length)
            stockTypes.pushResult (new Integer(len))

        [<GenericPushOperation("YANK", Description = "Tears an item out of the stack. The index is designated by the top of the INTEGER stack")>]
        static member yank tp =
            push {
                let! arg = popOne Integer.Me.MyType
                if arg >= 0L && int64 (stockTypes.Stacks.[tp].length) > arg then
                    let newStack = yank (int arg) stockTypes.Stacks.[tp]
                    Ops.Replace tp newStack 
                    return! zero
            }

        [<GenericPushOperation("YANKDUP", Description = "Yanks an item out of the stack and pushes it on top of the stack")>]
        static member yankdup tp =
            push {
                let! arg = popOne Integer.Me.MyType
                if arg >= 0L && int64 (stockTypes.Stacks.[tp].length) > arg then
                    let newStack = yankdup (int arg) stockTypes.Stacks.[tp]
                    Ops.Replace tp newStack 
                    return! zero
            }

        [<GenericPushOperation("IF", Description = "Execute either the first or the second item on top of the code stack", AppliesTo=[|"CODE"; "EXEC"|])>]
        static member If tp =
            //things are different for CODE and EXEC:
            //(5 3 INTEGER.< EXEC.IF 1 0) - should push the top item of the EXEC to the INTEGER stack
            // while (5 3 INTEGER.< CODE.QUOTE 1 CODE.QUOTE 0 CODE.IF) should push the second item of 
            // the CODE stack in order to conform to the "readability" of programs. 
            push {
                let! a2 = popOne tp : PushMonad<Push>
                let! a1 = popOne tp : PushMonad<Push>
                let! isTrue = popOne Bool.Me.MyType
                let res = 
                    match (tp = "CODE", isTrue) with
                    | (true, true) | (false, false) -> a1
                    | (true, false) | (false, true) -> a2

                return! result "EXEC" res
            }

        // everything bottle-necsk through here
        static member internal doRange start finish (code : Push) tp =
            let quote = Operation("CODE", stockTypes.Operations.["CODE"].["QUOTE"])
            let op = Operation(tp, stockTypes.Operations.[tp].["DO*RANGE"])

            let next = 
                if start < finish then start + 1L
                elif start > finish then start - 1L
                else start

            push {
                if start <> finish then
                    let valFinish = Value(Integer(finish))
                    let valNext = Value(Integer(next))
                    let newLoopList = 
                        if tp <> "CODE" 
                        then 
                            PushList([valNext; valFinish; op; code]) 
                        else 
                            PushList([valNext; valFinish; quote; code; op])
                
                    return! result "EXEC" newLoopList

                return! result "INTEGER" start
                return! result "EXEC" code
            }

        static member internal doTimes pushIndex tp =
            // creates a new item consiting of the code item preceded by INTEGER.POP
            let concatPopIntegerAndCode (code : Push) =
                PushList((makeOperation Integer.Me.MyType "POP")::(code.toList))
            
            push {
                let! code = popOne tp
                let! range = popOne Integer.Me.MyType
                if range > 0L then
                    let code = if not pushIndex then (concatPopIntegerAndCode code) else code
                    Ops.doRange  0L (1L - range) code tp |> ignore
                    return! zero
            }

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
            push {
                let! finish = popOne Integer.Me.MyType
                let! start = popOne Integer.Me.MyType
                let! code = popOne tp : PushMonad<Push>
                Ops.doRange start finish code tp |> ignore
                return! zero
            }

        [<GenericPushOperation("WRITE", Description="Write the top value out to the console", ShouldPickAtRandom = false)>]
        static member Write tp =
            if isEmptyStack tp then ()
            else
                Console.WriteLine(peekStack tp)

        [<GenericPushOperation("DUMPSTACK", Description="", ShouldPickAtRandom = false)>]
        static member DumpStack (tp : string) =
            Console.WriteLine(tp)
            Console.WriteLine("-------------")
            if isEmptyStack tp then ()
            else
                stockTypes.Stacks.[tp] |> Seq.iter (fun e -> Console.WriteLine(e))
            Console.WriteLine()

        [<GenericPushOperation("OPEN", 
            Description= "Opens a file, the name of which is given by the top of the LITERAL stack, pushes the result to the EXEC (or CODE) stack.",
            AppliesTo=[|"EXEC"; "CODE"|],
            ShouldPickAtRandom = false)>]
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
            AppliesTo=[|"EXEC"; "CODE"|],
            ShouldPickAtRandom = false)>]
        static member SaveFile (tp : string) =
            if areAllStacksNonEmpty ["LITERAL"; tp] then
                let file = (processArgs1 "LITERAL").Raw<string>()
                try
                    (processArgs1 tp).Raw<Push>().Serialize(file)
                with
                | e -> ()
