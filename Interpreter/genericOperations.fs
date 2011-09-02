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

    type Ops ()=
        let stock = stockTypes

        member t.flush (tp : System.Type) =
            stock.Stacks <- stock.Stacks.Remove(tp.Name)
            stock.Stacks <- stock.Stacks.Add(tp.Name, empty)

        member t.define (tp : System.Type) =
            let stack = stock.Stacks.["Identifier"]
            if stack.length = 0 then ()
            else
                let name = (peek stack).Value :?> string
                stock.Bindings <- stock.Bindings.Add(name, tp.Name)

        member t.dup (tp : System.Type) =
            if stock.Stacks.[tp.Name].length = 0 then ()
            else stock.pushResult (peek stock.Stacks.[tp.Name])
