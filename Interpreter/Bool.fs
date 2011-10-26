namespace push.types.stock

open push.types
open System

open System.Reflection

[<PushType("BOOLEAN")>]
type Bool =
    inherit PushTypeBase

    [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

    new () = {inherit PushTypeBase ()}
    new (b : bool) = {inherit PushTypeBase(b)}

    static member Me = Bool()
    static member simpleOp f = simpleOp f Bool.Me.MyType
    static member monoOp f stack = monoOp f stack Bool.Me.MyType
    static member singleOp f = Bool.monoOp f Bool.Me.MyType

    // custom parsing
    static member parse s =
        let result = ref Unchecked.defaultof<bool>
        if not (System.Boolean.TryParse(s, result)) 
        then 
            Unchecked.defaultof<PushTypeBase> 
        else 
            Bool(!result) :> PushTypeBase

    override t.Parser 
        with get() = 
            ExtendedTypeParser(Bool.parse)

    override t.ToString() =
        base.ToString()

    [<PushOperation("AND", Description = "Logical AND of the top two booleans")>]
    static member And() = 
        Bool.simpleOp (&&)

    [<PushOperation("FROMFLOAT", Description = "Pushies FALSE if top of the FLOAT stack is 0.0. True otherwise")>]
    static member fromFloat() =
        Bool.monoOp (fun f -> f <> 0.) "FLOAT"

    [<PushOperation("FROMINTEGER", Description = "Pushies FALSE if top of the INTEGER stack is 0. True otherwise")>]
    static member fromInt() =
        Bool.monoOp (fun f -> f <> 0L) "INTEGER"

    [<PushOperation("NOT", Description = "Logical OR of the top of the stack")>]
    static member Not() = 
        Bool.singleOp (not)

    [<PushOperation("OR", Description = "Logical OR of the top two booleans")>]
    static member Or() = 
        Bool.simpleOp (||)

    [<PushOperation("RAND", Description = "Generates a random boolean")>]
    static member Rand() =
        push {
            return! result Bool.Me.MyType (Type.Random.Next(0, 2) > 0)
        }
