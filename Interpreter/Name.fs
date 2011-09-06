namespace push.types.stock

module StockTypesName =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory
    open push.types.stock.StockTypesBool

    open System.Reflection
    open System

    [<PushType("NAME")>]
    type Name =
        inherit PushTypeBase

        new () = {inherit PushTypeBase ()}
        new (s : string) = {inherit PushTypeBase(s)}

        static member private Me = new Name()

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
            let rnd = new Random(int (DateTime.UtcNow.Ticks))
            let size = rnd.Next(1, maxSize) // limit it in size to 20 characters
            let chars = 
                [|
            
                for i = 0 to size do
                    yield (char(int(26. * rnd.NextDouble()) + 65))
                |]
            new System.String(chars)
           
        [<PushOperation("=")>]
        static member Eq() =
            match processArgs2 Name.Me.MyType with
            | [a1; a2] -> pushResult(new Bool(a1.Raw<int64>() = a2.Raw<int64>()))
            | _ -> ()

        [<PushOperation("RAND", Description = "Pushes a random generated NAME to the name stack")>]
        static member Rand () =
            pushResult (new Name (Name.GetRandomString 15))

        [<PushOperation("RANDBOUNDNAME", Description = "Pushes a random bound NAME to the name stack")>]
        static member RandBoundName () =
            if stockTypes.Bindings.IsEmpty then ()
            let rnd = new Random(int (DateTime.UtcNow.Ticks))
            let selectedName = fst ((stockTypes.Bindings |> Map.toList).[rnd.Next(stockTypes.Bindings.Count)])
            pushResult (new Name (selectedName))
        