namespace push.genetics

open push.types
open push.types.stock
open push.parser
open System

type Genetics (config : GenConfig, population : Push list) =
    let populSize = config.populSize
    let numGenerations = config.numGenerations
    let getArgument = config.getArgument 
    let getResult = config.getResult
    let fitnessCriteria = config.fitnessValues
    let mutable population = population
    let mutable evolvedProgramIndex = -1

    [<DefaultValue>]
    static val mutable private rnd : Random

    static member Random 
        with get () = 
            if Genetics.rnd = Unchecked.defaultof<Random> 
            then 
                Genetics.rnd <- Random(int DateTime.UtcNow.Ticks)
            Genetics.rnd

    member t.EvolvedProgram 
        with get () =
            if evolvedProgramIndex > 0 then Some population.[evolvedProgramIndex] else None

    member t.evalMember (popMember : Push) =
        pushToExec popMember
        eval Exec.Me.MyType

    member t.runMemberAndEvalFitness (populMember : Push) (fitnessCriterion : CodeFitnessCriterion)  =
        stockTypes.cleanAllStacks ()
        pushToExec fitnessCriterion.argument
        evalCode getArgument Exec.Me.MyType
        t.evalMember populMember
        let result = fitnessCriterion.evalFunc getResult
        Math.Abs(result - fitnessCriterion.value)


    member t.pickNextPopulation (i : int) =
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
            
        let spin = Genetics.Random.NextDouble()

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
                if minValue = 0. 
                then 
                    stop <- true
                    evolvedProgramIndex <- minIndex
                else
                    population <- t.Mutate ()

    // stubbed out for the moment
    member t.Mutate () =
        population