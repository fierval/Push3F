namespace push.genetics

[<AutoOpen>]
module GenConfig =
    open System.Runtime.CompilerServices
    open System
    open push.parser
    open push.types
    open push.types.stock
    open push.core
    open push.config

    type GenConfig = 
        {
            populSize : int
            maxCodePoints : int
            numGenerations : int
            getArgument : Push
            getResult : Push
            probCrossover : float
            probMutation : float
            fitnessValues : CodeFitnessCriterion list
        }

