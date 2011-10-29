namespace push.types.stock

open push.parser
open push.types.stock
open System.Reflection
open System
open push.types

[<PushType("LITERAL", Description = "Double-quoted literals")>]
type Literal =
    inherit PushTypeBase

    new () = {inherit PushTypeBase ()} 
    new (l : string) = {inherit PushTypeBase(l)} 

    static member internal Me = new Literal()

    override t.ToString() =
        base.ToString()

    // custom parsing.
    // in this case custom parsing is disabled.
    // Push will parse these values
    override t.Parser 
        with get() = 
            Unchecked.defaultof<ExtendedTypeParser>

    override t.isQuotable with get() = false

    [<PushOperation("+", Description="Concatenate two top literals", ShouldPickAtRandom = false)>]
    static member Concat() =
        simpleOp (fun (a:string) b -> a + b) Literal.Me.MyType

    [<PushOperation("RAND", Description = "Pushes a random literal on top of the literal stack", ShouldPickAtRandom = false)>]
    static member Rand() =
        push {
            return! result Literal.Me.MyType (Name.GetRandomString 15)
        }
