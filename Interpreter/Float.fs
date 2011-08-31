﻿namespace push.types.stock

module StockTypesFloat =
    open push.types.Type
    open push.types.TypeAttributes
    open push.types.TypeFactory

    open System.Reflection

    [<PushType("FLOAT")>]
    type Float =
        inherit PushTypeBase

        [<DefaultValue>]static val mutable private operations : Map<string, MethodInfo>

        override t.Operations 
            with get() = 
                if Unchecked.defaultof<Map<string, MethodInfo>> = Float.operations 
                    then 
                        Float.operations <- PushTypeBase.GetOperations(t)
                Float.operations

        new () = {inherit PushTypeBase ()}


        new (f : float) = {inherit PushTypeBase(f)}


        override t.ToString() =
            base.ToString()

        [<PushOperation("+")>]
        static member Add() =
            match processArgs2 typeof<Float> with
            | (Some a1, Some a2) -> pushResult(new Float(a1.Raw<float>() + a2.Raw<float>()))
            | _ -> ()
            
        [<PushOperation("*")>]
        static member Multiply() =
            match processArgs2 typeof<Float> with
            | (Some a1, Some a2) -> pushResult(new Float(a1.Raw<float>() * a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("-")>]
        static member Subtract() =
            match processArgs2 typeof<Float> with
            | (Some a1, Some a2) -> pushResult(new Float(a1.Raw<float>() - a2.Raw<float>()))
            | _ -> ()

        [<PushOperation("/")>]
        static member Divide() =
            match processArgs2 typeof<Float> with
            | (Some a1, Some a2) -> 
                if a1.Raw<float>() = 0. 
                then pushResult(new Float(System.Double.MinValue)) 
                else pushResult(new Float(a1.Raw<float>() - a2.Raw<float>()))
            | _ -> ()
