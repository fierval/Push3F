﻿namespace push.types.stock

open push.types
open push.types.stock
open push.parser

open System.Reflection
open System

[<PushType("NAME", ShouldPickAtRandom = false)>]
type Name =
    inherit PushTypeBase

    new () = {inherit PushTypeBase ()}
    new (s : string) = {inherit PushTypeBase(s)}

    static member Me = Name()

    override t.isQuotable with get() = true

    override t.Eval =
        if getState Name.Me.MyType = State.Quote then
            setState Name.Me.MyType State.Exec
            t :> PushTypeBase
        else
            match tryGetBinding (t.Raw<string>()) with
            | Some value -> value
            | None -> t :> PushTypeBase

    override t.ToString() =
        base.ToString()
        
    // custom parsing.
    // in this case custom parsing is disabled.
    // Push will parse these values
    override t.Parser 
        with get() = 
            Unchecked.defaultof<ExtendedTypeParser>

        // generates a random string of maximum size
    static member GetRandomString maxSize =
        let size = Type.Random.Next(1, maxSize) // limit it in size to 20 characters
        let chars = 
            [|
            
                for i = 0 to size do yield (char(int(26. * Type.Random.NextDouble()) + 65))
            |]
        new System.String(chars)
           
    [<PushOperation("QUOTE", Description = "Next NAME is simply pushed onto the name stack")>]
    static member Quote () =
        setState Name.Me.MyType State.Quote

    [<PushOperation("RAND", Description = "Pushes a random generated NAME to the name stack")>]
    static member Rand () =
        push {
            return! result Name.Me.MyType (Name.GetRandomString 15)
        }

    [<PushOperation("RANDBOUNDNAME", Description = "Pushes a random bound NAME to the name stack")>]
    static member RandBoundName () =
        if not stockTypes.Bindings.IsEmpty then
            let selectedName = fst ((stockTypes.Bindings |> Map.toList).[Type.Random.Next(0, stockTypes.Bindings.Count)])
            pushResult (Name (selectedName))
   
    [<PushOperation("DUMPBINDINGS", Description = "Dumps all current bindings to the console") >]
    static member DumpBindings() =
        if not stockTypes.Bindings.IsEmpty then 
            stockTypes.Bindings |> Map.iter(fun key value -> Console.WriteLine("{0}          {1}", key, value.StructuredFormatDisplay))