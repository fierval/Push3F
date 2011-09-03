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

    type Microsoft.FSharp.Collections.Map<'Key, 'Value when 'Key : comparison>  with
        member t.Replace (key, value) =
            t.Remove(key).Add(key, value)

    type Ops ()=
        static let stock = stockTypes

        static member private Replace key value =
            stock.Stacks.Replace(key, value)

        static member private getIntArgument (tp : System.Type) =
            if tp.Name = "Integer" then Ops.dup tp
            processArgs1 (stock.Types.["INTEGER"].GetType())
          
        [<PushOperation("FLUSH")>]
        static member flush (tp : System.Type) =
            stock.Stacks <- stock.Stacks.Replace(tp.Name, empty)
        
        [<PushOperation("DEFINE")>]
        static member define (tp : System.Type) =
            let stack = stock.Stacks.["Identifier"]
            if stack.length = 0 then ()
            else
                let name = (peek stack).Value :?> string
                stock.Bindings <- stock.Bindings.Add(name, tp.Name)

        [<PushOperation("DUP")>]
        static member dup (tp : System.Type) =
            if stock.Stacks.[tp.Name].length = 0 then ()
            else stock.pushResult (peek stock.Stacks.[tp.Name])

        [<PushOperation("POP")>]
        static member pop (tp : System.Type) =
            processArgs1 tp |> ignore

        [<PushOperation("ROT")>]
        static member rot (tp : System.Type) =
            pushResult (new StockTypesInteger.Integer(2L))
            Ops.yank

        [<PushOperation("SHOVE")>]
        static member shove (tp : System.Type) =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let value = processArgs1 tp
            if value = Unchecked.defaultof<PushTypeBase> then ()
            Ops.Replace tp.Name (shove (int (arg.Raw<int64>())) value stock.Stacks.[tp.Name])

        [<PushOperation("SWAP")>]
        static member swap (tp : System.Type) =
            let args = processArgs2 tp
            if args.Length < 2 then ()
            let hd, tl = args.Head, (args.Tail).Head
            stock.pushResult hd
            stock.pushResult tl

        [<PushOperation("STACKDEPTH")>]
        static member stackdepth (tp : System.Type) =
            stock.pushResult (new StockTypesInteger.Integer(int64 (stock.Stacks.[tp.Name].length)))

        [<PushOperation("YANK")>]
        static member yank (tp : System.Type) =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let newStack = yank (int (arg.Raw<int64>())) stock.Stacks.[tp.Name]
            Ops.Replace tp.Name newStack

        [<PushOperation("YANKDUP")>]
        static member yankdup (tp : System.Type) =
            let arg = Ops.getIntArgument tp
            if arg = Unchecked.defaultof<PushTypeBase> then ()
            let newStack = yankdup (int (arg.Raw<int64>())) stock.Stacks.[tp.Name]
            Ops.Replace tp.Name newStack
        