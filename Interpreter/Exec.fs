namespace push.types.stock

[<AutoOpen>]
module StockTypesExec =
    open push.types
    open push.parser
    open push.stack
    open push.types.stock
    open System.Reflection
    open System

    [<PushType("EXEC", Description = "Actual execution stack")>]
    type Exec =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()} 
        new (p : Push) = {inherit PushTypeBase(p)} 

        [<DefaultValue>] static val mutable private yCount : int64

        static member private Me = new Exec()

        override t.ToString() =
          base.ToString()

        // custom parsing.
        // in this case custom parsing is disabled.
        // Push will parse these values
        override t.Parser 
            with get() = 
                Unchecked.defaultof<ExtendedTypeParser>

        override t.isQuotable with get() = false

        [<PushOperation("K", Description = "Removes the second item on the exec stack")>]
        static member K () =
            pushResult (Integer(1L))
            Ops.yank Exec.Me.MyType |> ignore
            Ops.pop Exec.Me.MyType

        [<PushOperation("S", Description = "Pops A, B, C, then push (B C), then push C, then push A")>]
        static member S () =
            push {
                let! a = popOne<Push> Exec.Me.MyType
                let! b = popOne<Push> Exec.Me.MyType
                let! c = popOne<Push> Exec.Me.MyType

                return! result Exec.Me.MyType (PushList[b; c])
                return! result Exec.Me.MyType c
                return! result Exec.Me.MyType a
            }

        [<PushOperation("Y", Description = "Inserts below the top: (EXEC.Y <top>)")>]
        static member Y () =
            if not (isEmptyStack Exec.Me.MyType)
            then
                let op = Operation("EXEC",stockTypes.Operations.["EXEC"].["Y"])
                let code = (peekStack "EXEC").Raw<Push>()

                // push this operation on top of the stack
                PushList ([op; code]) |> pushToExec 
                Integer(1L) |> pushResult
                Ops.shove "EXEC"

        [<PushOperation("DUMPALLSTACKS", Description= "Writes out all the stacks to the console")>]
        static member DumpAllStacks () =
            stockTypes.Stacks |> Map.iter (fun key value -> Ops.DumpStack key)
