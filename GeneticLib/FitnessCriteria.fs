namespace push.genetics

[<AutoOpen>]
module FitnessCriteria =

    open System
    open push.parser

    type FitnessCriterion<'a> =
        { argument : 'a; value : float; evalFunc : 'a -> float}

    type CodeFitnessCriterion = FitnessCriterion<Push>

