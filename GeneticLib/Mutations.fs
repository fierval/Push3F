namespace push.genetics

[<AutoOpen>]
module internal Mutations =
    open push.types
    open push.types.stock
    open System

    let Rand = Type.Random
    let length (code : Push) = code.toList.Length
    let randWithinRange (code : Push) = Rand.Next(1, length code)
    let shouldPickForOp prob =
        Rand.NextDouble() < prob
    
    // pick a random element from the collection, but not the one specified
    let randomPick collectionSize indexNotToPick =
        if indexNotToPick = 0 then
            Rand.Next(1, collectionSize)
        elif indexNotToPick = collectionSize - 1 then
            Rand.Next(0, collectionSize - 1)
        else
            let rndMore = Rand.Next(indexNotToPick + 1, collectionSize)
            let rndLess = Rand.Next(0, indexNotToPick)
            if Rand.NextDouble() < 0.5 then rndLess else rndMore

    let removeRandomPiece (code : Push) =
        let r = randWithinRange code
        PushList(code.toList |> List.remove r)
        
    let trimExtraCodePoints maxCodePoints (code : Push)  =
        let rec trimExtraCodePointsTailRec (code : Push) =
            if code.toList.Length <= maxCodePoints then code
            else
                trimExtraCodePointsTailRec (removeRandomPiece code)

        trimExtraCodePointsTailRec code

    let insertRandomPiece (code : Push) maxCodePoints =
        let r = randWithinRange code
        let newCode = PushList(Code.insert code r (Code.rand (maxCodePoints / 2)))
        trimExtraCodePoints maxCodePoints newCode
         
    let xoverSubtree  (mom : Push) (dad : Push) maxCodePoints =
        let r1 = randWithinRange mom
        let r2 = randWithinRange dad

        let child1 = PushList(Code.insert mom r1 (Code.extract dad r2)) |> trimExtraCodePoints maxCodePoints
        
        let child2 = PushList(Code.insert dad r2 (Code.extract mom r1)) |> trimExtraCodePoints maxCodePoints
            
        child1, child2

