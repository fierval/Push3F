namespace Push

open System
open System.Dynamic
open System.Linq.Expressions
open System.Reflection
open System.Runtime.CompilerServices
open System.Collections.Generic
open Microsoft.CSharp.RuntimeBinder
open Microsoft.FSharp.Reflection
open Arguments
open push.core
open push.parser
open push.genetics
open push.config
open push.types.stock

[<AutoOpen>]
module internal Genetic =
   
    let (?)  (targetObject : obj) (targetMember:string)  : 'TargetResult  = 
        let targetResultType = typeof<'TargetResult>
        if not (FSharpType.IsFunction targetResultType)
        then 
            let cs = CallSite<Func<CallSite, obj, obj>>.Create(Binder.GetMember(CSharpBinderFlags.None, targetMember, null, [| CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) |]))
            unbox (cs.Target.Invoke(cs, targetObject))
        else
            if not (FSharpType.IsFunction targetResultType) then failwithf "%A is not a function type" targetResultType 
            let domainType,_ = FSharpType.GetFunctionElements targetResultType
            let domainTypes = 
                if FSharpType.IsTuple domainType then FSharpType.GetTupleElements domainType 
                elif domainType = typeof<unit> then [| |]
                else [|domainType|]
            let objToObjFunction = 
               (fun argObj ->
                 let realArgs = 
                   match domainTypes with 
                   | [|  |] -> [| |]
                   | [| argTy |] -> [| argObj |]
                   | argTys -> FSharpValue.GetTupleFields(argObj)
             
                 let funcType = Expression.GetFuncType [| yield typeof<CallSite>; yield typeof<obj>; yield! domainTypes; yield typeof<obj> |]
                 let cty = typedefof<CallSite<_>>.MakeGenericType [| funcType |]
                 let cs = cty.InvokeMember("Create", BindingFlags.Public ||| BindingFlags.Static ||| BindingFlags.InvokeMethod, null, null, [|(box(Binder.InvokeMember(CSharpBinderFlags.None, targetMember, null, null, Array.create (realArgs.Length + 1) (CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, targetMember)))))|])
                            |> unbox<CallSite>
                 let target = cs.GetType().GetField("Target").GetValue(cs)
                 target.GetType().InvokeMember("Invoke", BindingFlags.Public ||| BindingFlags.Instance ||| BindingFlags.InvokeMethod, null, target, [| yield box cs; yield box targetObject; yield! realArgs |])
                 )
            let atyFunction = FSharpValue.MakeFunction(targetResultType,objToObjFunction)
            unbox<'TargetResult> atyFunction

    let (?<-) (targetObject : obj) (targetMember : string) (args : 'Args) : unit =
      let argumentInfos = [| CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null); 
                             CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant ||| CSharpArgumentInfoFlags.UseCompileTimeType, null) |]
      let binder = Binder.SetMember(CSharpBinderFlags.None, 
                                    targetMember, 
                                    targetObject.GetType(),
                                    argumentInfos)
      let setterSite = CallSite<Func<CallSite, obj, 'Args, obj>>.Create(binder)
      setterSite.Target.Invoke(setterSite, targetObject, args) |> ignore

    
    let readConfig (file : string) =
        let reader = ConfigReader(file)
        let conf = reader.Read()
        {
            populSize = conf?popSize
            maxCodePoints = conf?maxCodePoints
            numGenerations = conf?numGenerations
            getArgument = parseGetCode (conf?getArgument)
            getResult = parseGetCode (conf?getResult)
            probCrossover = conf?probCrossOver
            probMutation = conf?probMutation
            fitnessValues = 
                [for i = 0 to reader.CountSamples - 1 do 
                    yield
                        {argument = parseGetCode (reader.GetSampleValue(i, "In").ToString()); 
                        value = parseGetCode (reader.GetSampleValue(i, "Out").ToString())}]
        }

    let launchGeneticMode configFileName =
        try
            let config = readConfig configFileName
            let population = List.init (config.populSize - 1) (fun i -> Code.rand config.maxCodePoints)
            Genetics(config, population).Run()
        with
        | e -> Console.WriteLine(e.ToString())