namespace push.types.stock

[<AutoOpen>]
module StockTypesInteger =
    open push.types
    open System

    open System.Reflection

    [<PushType("INTEGER")>]
    type Integer  =
        inherit PushTypeBase

        new () = {inherit PushTypeBase() }
        new (n : int64) = {inherit PushTypeBase(n)}

        static member Me = Integer()
        static member simpleOp (f : int64 -> int64 -> int64) = simpleOp f Integer.Me.MyType
        static member monoOp f stack = monoOp f stack Integer.Me.MyType
            
        // custom parsing
        static member parse s =
            let result = ref Unchecked.defaultof<int64>
            if not (System.Int64.TryParse(s, result)) 
            then 
                Unchecked.defaultof<PushTypeBase> 
            else 
                new Integer(!result) :> PushTypeBase

        override t.Parser 
            with get() = 
                ExtendedTypeParser(Integer.parse)
        
        override t.ToString() =
            base.ToString()

        [<PushOperation("+")>]
        static member Add() =
            Integer.simpleOp (fun a b -> a + b)
            
        [<PushOperation("*")>]
        static member Multiply() =
           Integer.simpleOp (fun a b -> a * b)

        [<PushOperation("-")>]
        static member Subtract() =
            Integer.simpleOp (fun a b -> a - b)

        [<PushOperation("/")>]
        static member Divide() =
            let divide stack = 
                push {
                    let! right = popOne stack
                    let! left = popOne stack
                    if right <> 0L then
                        return! (result stack (left / right))
                }
            divide Integer.Me.MyType

        [<PushOperation("%")>]
        static member Mod() =
            let getMod stack = 
                push {
                    let! right = popOne stack
                    let! left = popOne stack
                    if right <> 0L then
                        return! (result stack (left % right))
                }
            getMod Integer.Me.MyType

        [<PushOperation("FROMFLOAT", Description = "Pushes int64 converted into int64 onto INTEGER stack")>]
        static member fromFloat() =
            Integer.monoOp (fun (f:float) -> int64 f) "FLOAT"

        [<PushOperation("FROMBOOLEAN", Description = "Pushes 1 onto INTEGER stack if top boolean is true, 0 otherwise")>]
        static member fromBool() =
            Integer.monoOp (fun b -> if b then 1L else 0L) "BOOLEAN"

        [<PushOperation("MAX")>]
        static member Max () =
            Integer.simpleOp (fun a b -> max a b)

        [<PushOperation("MIN")>]
        static member Min () =
            Integer.simpleOp (fun a b -> min a b)
    
        [<PushOperation("RAND", Description = "Pushes a random int64. Range is determined by MIN-RANDOM-INTEGER and MAX-RANDOM-INTEGER")>]
        static member Rand () =
            push {
                return! result Integer.Me.MyType (int64 (Type.Random.Next(0, Int32.MaxValue)))
            }

