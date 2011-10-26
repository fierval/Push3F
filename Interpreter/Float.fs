namespace push.types.stock

open push.types

open System.Reflection
open System

[<PushType("FLOAT")>]
type Float =
    inherit PushTypeBase

    new () = {inherit PushTypeBase ()}
    new (f : float) = {inherit PushTypeBase(f)}

    static member Me = Float()
    static member simpleOp (f : float -> float -> float) = simpleOp f Float.Me.MyType
    static member monoOp f stack = monoOp f stack Float.Me.MyType
    static member singleOp f = Float.monoOp f Float.Me.MyType

    override t.ToString() =
        t.Raw<float>().ToString("F")

    // custom parsing
    static member parse s =
        let result = ref Unchecked.defaultof<float>
        if not (System.Double.TryParse(s, result)) 
        then 
            Unchecked.defaultof<PushTypeBase> 
        else 
            new Float(!result) :> PushTypeBase

    override t.Parser 
        with get() = 
            ExtendedTypeParser(Float.parse)

    [<PushOperation("+")>]
    static member Add() =
        Float.simpleOp (fun a b -> a + b)
            
    [<PushOperation("*")>]
    static member Multiply() =
        Float.simpleOp (fun a b -> a * b)

    [<PushOperation("-")>]
    static member Subtract() =
        Float.simpleOp (fun a b -> a - b)

    [<PushOperation("/")>]
    static member Divide() =
        let divide stack = 
            push {
                let! right = popOne stack
                let! left = popOne stack
                if right <> 0. then
                    return! (result stack (left / right))
            }
        divide Float.Me.MyType

    [<PushOperation("%")>]
    static member Mod() =
        let getMod stack = 
            push {
                let! right = popOne stack
                let! left = popOne<float> stack
                if right <> 0. then
                    let quot = Math.Floor(Math.Floor(left) / Math.Floor(right))
                    let! res = result stack (left - quot * right)
                    return res                        
            }
        getMod Float.Me.MyType


    [<PushOperation("FROMINTEGER", Description = "Pushes int64 converted into float onto FLOAT stack")>]
    static member fromInteger() =
        Float.monoOp (fun (f:int64) -> float f) "INTEGER"

    [<PushOperation("FROMBOOLEAN", Description = "Pushes 1 onto INTEGER stack if top boolean is true, 0 otherwise")>]
    static member fromBool() =
        Float.monoOp (fun b -> if b then 1. else 0.) "BOOLEAN"

    [<PushOperation("COS", Description = "Pushes the cos of the top item")>]
    static member Cos() =
        Float.singleOp(fun t -> Math.Cos(t))

    [<PushOperation("MAX")>]
    static member Max () =
        Float.simpleOp (fun a b -> max a b)

    [<PushOperation("MIN")>]
    static member Min () =
        Float.simpleOp (fun a b -> min a b)
    
    [<PushOperation("RAND", Description = "Pushes a random float. Range is determined by MIN-RANDOM-FLOAT and MAX-RANDOM-FLOAT")>]
    static member Rand () =
        push {
            return! result Float.Me.MyType (Type.Random.NextDouble())
        }

    [<PushOperation("SIN", Description = "Pushes the sin of the top item")>]
    static member Sin() =
        Float.singleOp(fun t -> Math.Sin(t))

    [<PushOperation("TAN", Description = "Pushes the tangent of the top item")>]
    static member Tan() =
        push {
            let! arg = popOne Float.Me.MyType
            if Math.Cos(arg) <> 0. then
                return! result Float.Me.MyType (Math.Tan(arg))
        }
        