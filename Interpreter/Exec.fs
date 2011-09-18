namespace push.types.stock

[<AutoOpen>]
module StockTypesExec =
    open push.types
    open push.parser
    open push.stack
    open push.types.stock
    open System.Reflection
    open System

    [<PushType("EXEC", Description = "The heart of the language")>]
    type Exec =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()} 
        new (p : Push) = {inherit PushTypeBase(p)} 

        static member private Me = new Exec()

        override t.ToString() =
          base.ToString()

        // custom parsing.
        // in this case custom parsing is disabled.
        // Push will parse these values
        override t.Parser 
            with get() = 
                Unchecked.defaultof<ExtendedTypeParser>

        // code type is "quoatable", however CODE.QUOTE is implemented
        // by simply pushing the next item from the EXEC stack to the code stack
        override t.isQuotable with get() = false

        [<PushOperation("K", Description = "Removes the second item on the exec stack")>]
        static member K () =
            pushResult (Integer(2L))
            Ops.yank Exec.Me.MyType

        [<PushOperation("S", Description = "Pops A, B, C, then push (B C), then push C, then push A")>]
        static member S () =
            Ops.stackdepth Exec.Me.MyType
            match processArgs1 Integer.Me.MyType with
            | a when a <> Unchecked.defaultof<PushTypeBase> && a.Raw<int64>() = 3L ->
                let a = processArgs1 Exec.Me.MyType
                let b = processArgs1 Exec.Me.MyType
                let c = processArgs1 Exec.Me.MyType

                pushResult (Exec(PushList [b.Raw<Push>(); c.Raw<Push>()]))
                pushResult c
                pushResult a

            | _ -> ()

        [<PushOperation("Y", Description = "Inserts below the top: (EXEC.Y <top>)")>]
        static member Y () =
            if isEmptyStack Exec.Me.MyType then ()

            // duplicate top of the stack
            Ops.dup Exec.Me.MyType

            // push this operation on top of the stack
            pushResult (Exec(Operation("Y", Exec.Me.GetType().GetMethod("Y"))))

            // and then shove it underneath the second item of the stack
            pushResult (Integer(2L))
            Ops.shove Exec.Me.MyType

        [<PushOperation("DUMPALLSTACKS", Description= "Writes out all the stacks to the console")>]
        static member DumpAllStacks () =
            stockTypes.Stacks |> Map.iter (fun key value -> Ops.DumpStack key)