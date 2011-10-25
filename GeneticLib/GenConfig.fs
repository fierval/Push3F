namespace push.genetics

[<AutoOpen>]
module GenConfig =

    open System
    open push.parser
    open push.types

    type GenConfig = 
        {
            populSize : int
            numGenerations : int
            getArgument : Push
            getResult : Push
            fitnessValues : CodeFitnessCriterion list
        }
