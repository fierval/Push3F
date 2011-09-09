namespace push.types

module GenericOperations =

    open TypesShared
    open TypeAttributes
    open System.Reflection
    open push.exceptions.PushExceptions
    open Type
    open TypeFactory
    open push.stack
    open Stack
    open push.types.stock

    type Ops ()=

        static member private Replace key value =
            stockTypes.Stacks <- stockTypes.Stacks.Replace(key, value)

        static member private getIntArgument tp =
            processArgs1 "INTEGER"
          
        [<PushOperation("FLUSH")>]
        static member flush tp =
            stockTypes.Stacks <- stockTypes.Stacks.Replace(tp, empty)
        
        [<PushOperation("DEFINE")>]
        static member define tp =
            let stack = stockTypes.Stacks.["NAME"]
            if stack.length = 0 then ()
            else
                let name = (peek stack).Value :?> string
                stockTypes.Bindings <- stockTypes.Bindings.Add(name, (peek stockTypes.Stacks.[tp]))

        [<PushOperation("DUP")>]
        static member dup tp =
            if stockTypes.Stacks.[tp].length = 0 then ()
            else stockTypes.pushResult (peek stockTypes.Stacks.[tp])

        [<PushOperation("POP")>]
        static member pop tp =
            processArgs1 tp |> ignore

        [<PushOperation("ROT")>]
        static member rot tp =
            pushResult (new StockTypesInteger.Integer(2L))
            Ops.yank tp

        [<PushOperation("SHOVE")>]
        static member shove tp =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let value = processArgs1 tp
            if value = Unchecked.defaultof<PushTypeBase> then ()
            Ops.Replace tp (shove (int (arg.Raw<int64>())) value stockTypes.Stacks.[tp])

        [<PushOperation("SWAP")>]
        static member swap tp =
            let args = processArgs2 tp
            if args.Length < 2 then ()
            let hd, tl = args.Head, (args.Tail).Head
            stockTypes.pushResult tl
            stockTypes.pushResult hd

        [<PushOperation("STACKDEPTH")>]
        static member stackdepth tp =
            let preLen = int64 (stockTypes.Stacks.[tp].length)
            let len = 
                match tp with
                | "INTEGER" ->  preLen + 1L
                | _ -> preLen

            stockTypes.pushResult (new StockTypesInteger.Integer(len))

        [<PushOperation("YANK")>]
        static member yank tp =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let newStack = yank (int (arg.Raw<int64>())) stockTypes.Stacks.[tp]
            Ops.Replace tp newStack 
            |> ignore

        [<PushOperation("YANKDUP")>]
        static member yankdup tp =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let newStack = yankdup (int (arg.Raw<int64>())) stockTypes.Stacks.[tp]
            Ops.Replace tp newStack
        