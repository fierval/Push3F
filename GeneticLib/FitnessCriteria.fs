namespace push.genetics

[<AutoOpen>]
module FitnessCriteria =

    open System
    open push.types

    type FitnessCriterion<'a> =
        { argument : 'a; value : 'a; }

    type CodeFitnessCriterion = FitnessCriterion<Push>

