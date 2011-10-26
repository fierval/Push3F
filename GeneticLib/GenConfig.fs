namespace push.genetics

[<AutoOpen>]
module GenConfig =

    open System
    open push.parser
    open push.types
    open push.types.stock
    open push.core

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

    let config = {
         populSize = 100 
         maxCodePoints = 200 
         numGenerations = 100
         getArgument = parseGetCode "CODE.NOOP"
         getResult = parseGetCode "CODE.NOOP"
         fitnessValues = [{argument = parseGetCode "0"; evalFunc = (fun code -> 0.); value = 0.}]
         probCrossover = 0.7
         probMutation = 0.3
     }

