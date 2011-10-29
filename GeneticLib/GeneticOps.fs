namespace push.genetics

open push.types
open push.parser
open push.types.stock
open System

type Genetics (config : GenConfig, population : Push list) =
    let populSize = config.populSize
    let numGenerations = config.numGenerations
    let getArgument = config.getArgument 
    let getResult = config.getResult
    let fitnessCriteria = config.fitnessValues
    let mutable population = population
    let mutable evolvedProgramIndex = -1

    member t.EvolvedProgram 
        with get () =
            if evolvedProgramIndex > 0 then Some population.[evolvedProgramIndex] else None

    member private t.evalMember (popMember : Push) =
        pushToExec popMember
        eval Exec.Me.MyType

    member private t.runMemberAndEvalFitness (populMember : Push) (fitnessCriterion : CodeFitnessCriterion)  =
        stockTypes.cleanAllStacks ()
        pushToExec fitnessCriterion.argument
        evalCode getArgument Exec.Me.MyType
        t.evalMember populMember
        let result = fitnessCriterion.evalFunc getResult
        Math.Abs(result - fitnessCriterion.value)


    member private t.pickNextPopulation (i : int) =
        let fitnessValues : float list = 
            population
            |> List.map(fun e -> fitnessCriteria |> List.map (fun c -> t.runMemberAndEvalFitness e c) |> List.sum  )

        let totalFitness = List.sum fitnessValues
        let minFitness = List.min fitnessValues
        let minFitnessIndex = List.findIndex (fun e -> e = minFitness) fitnessValues
        t.Describe(i, totalFitness, minFitness)
                        
        let selectionProbs = fitnessValues |> List.map(fun e -> 1.0 - e / totalFitness)

        // Cumulative sum of probabilities, reverse the result and chop off the first 0.
        let cumulativeProbs = fitnessValues |> List.fold(fun cum e -> (e + cum.Head)::cum) [0.] |> List.rev |> List.tail
            
        let spin = Type.Random.NextDouble()

        // spin the roulette as many times as we have members in our population.
        population
        |> List.map 
            (fun p ->
                let nextSelected = cumulativeProbs |> List.findIndex(fun e -> e < spin)
                population.[nextSelected]
            ), minFitness, minFitnessIndex
        
    member t.Describe (i, totalFitness, minFitness) =
        printfn "Generation: %d" i
        printfn "Total fitness: %f" totalFitness
        printfn "Minimal fitness: %f" minFitness
        printfn "------------------------"
        printfn ""

    member t.Run () =
        let mutable stop = false
        let mutable i = 0
        while (not stop) do
            if i = populSize then stop <- true
            else
                let pop, minValue, minIndex = t.pickNextPopulation i
                population <- pop
                i <- i + 1
                if minValue = 0. 
                then 
                    stop <- true
                    evolvedProgramIndex <- minIndex
                else
                    t.mutate ()

    member private t.replacePopulation (newPopulation : (int * Push) list) =
        for (i, e) in newPopulation do 
            population <- population |> List.replace i  e

    member private t.crossOver () =
        // pick a cross-over population, memorizing their indices
        let pickedForXover = 
            population
            |> List.filteredList(fun p -> shouldPickForCrossover ())
        
        // if an odd number was selected we throw one of them away since we cannot pair them all up this way.
        let pickedForXoverLength = if pickedForXover.Length &&& 1 > 0 then pickedForXover.Length - 1 else pickedForXover.Length
        let pickedForXover = if pickedForXover.Length <> pickedForXoverLength then pickedForXover.Tail else pickedForXover
        let partitionBoundary = pickedForXoverLength / 2

        // cross them over, creating a list of ((child1, child2), index1, index2)
        let crossedOver = 
            [
                for i in 0.. (partitionBoundary - 1) -> 
                    xoverSubtree (snd pickedForXover.[i]) (snd pickedForXover.[partitionBoundary + i]),
                    fst pickedForXover.[i], fst pickedForXover.[partitionBoundary + i]
            ]

        // split the list into two lists of the crossed-over children and the original indicies
        let crossedOverAll = crossedOver |> List.collect(fun ((p1, p2), i1, i2) -> [(i1, p1); (i2, p2)]) 

        // return the children back into the population
        t.replacePopulation crossedOverAll 

    member private t.mutatePopulation() =
        let pickedForMutation = 
            population
            |> List.filteredList(fun p -> shouldPickForMutation ())
            |> List.map(fun (i, e) -> i, if Rand.NextDouble() < 0.5 then removeRandomPiece e else insertRandomPiece e)
            
        t.replacePopulation pickedForMutation 
                    
    member t.mutate () =
        t.crossOver()
        t.mutatePopulation ()
        