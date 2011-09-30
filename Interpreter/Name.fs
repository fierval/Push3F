﻿namespace push.types.stock

[<AutoOpen>]
module StockTypesName =
    open push.types
    open push.types.stock
    open push.parser

    open System.Reflection
    open System

    [<PushType("NAME")>]
    type Name =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (s : string) = {inherit PushTypeBase(s)}

        static member Me = Name()

        override t.isQuotable with get() = true

        override t.Eval = 
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
            let size = Name.Random.Next(1, maxSize) // limit it in size to 20 characters
            let chars = 
                [|
            
                    for i = 0 to size do yield (char(int(26. * Name.Random.NextDouble()) + 65))
                |]
            new System.String(chars)
           
        [<PushOperation("=")>]
        static member Eq() =
            match processArgs2 Name.Me.MyType with
            | [a1; a2] -> pushResult(Bool(a1.Raw<int64>() = a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("QUOTE", Description = "Next NAME is simply pushed onto the name stack")>]
        static member Quote () =
            setState Name.Me.MyType State.Quote

        [<PushOperation("RAND", Description = "Pushes a random generated NAME to the name stack")>]
        static member Rand () =
            pushResult (Name (Name.GetRandomString 15))

        [<PushOperation("RANDBOUNDNAME", Description = "Pushes a random bound NAME to the name stack")>]
        static member RandBoundName () =
            if stockTypes.Bindings.IsEmpty then ()
            let selectedName = fst ((stockTypes.Bindings |> Map.toList).[Name.Random.Next(stockTypes.Bindings.Count)])
            pushResult (Name (selectedName))
   