namespace push.parser

[<AutoOpen>]
module Ast = 
    open System
    open System.Reflection
    open push.types
    open System.Diagnostics
    open System.Collections.Generic
    open Microsoft.FSharp.Collections.Tagged

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<CustomEquality;CustomComparison>]
    type Push = 
        | Value of PushTypeBase
        | PushList of Push list
        | Operation of string * MethodInfo
        with
            interface IComparable with
                member x.CompareTo y =
                    let rec compare p1 p2 =
                        match p1, p2 with
                        | (Value v1, Value v2) -> Push.compareValues v1 v2
                        | (Operation (o1, m1), Operation (o2, m2)) -> 
                            let o1compo2 = o1.CompareTo(o2)
                            if o1compo2 = 0 then m1.Name.CompareTo(m2.Name)
                            else
                                o1compo2
                        | (PushList l1, PushList l2) -> 
                            if l1.Length <> l2.Length then l1.Length - l2.Length 
                                else 
                                    match List.map2 (fun e1 e2 -> compare e1 e2) l1 l2 
                                        |> List.tryFind(fun e -> e <> 0) with
                                    | Some x -> x
                                    | None -> 0
                                    
                        | (_,_) -> -1

                    match y with
                    | :? Push as p ->
                        if p = Unchecked.defaultof<Push> then -1
                        else
                            compare x p
                    | _ -> -1
            
            // the following two interfaces are used by the
            // powerpack two collections Set and Map
            interface IComparer<Push> with
                member x.Compare (p1, p2) =
                    ((p1) :> IComparable).CompareTo p2

            interface IEqualityComparer<Push> with
                member x.Equals (p1, p2) =
                    ((p1) :> IComparable).CompareTo p2 = 0

                member x.GetHashCode(objct) =
                    objct.GetHashCode()

            static member private compareValues (v1 : PushTypeBase) (v2 : PushTypeBase) =
                
                match v1.Value, v2.Value with
                | (:? int64 as i1), (:? float as f2) -> int ((float i1) - f2)
                | (:? float as f1), (:? int64 as i2) -> int (f1 - (float i2))
                | _ -> if v1.Value.GetType() <> v2.Value.GetType() 
                        then
                            -1
                        else
                            String.Compare(v1.Value.ToString(), v2.Value.ToString(), true)

                 
            member private t.StructuredFormatDisplay = 
                box(t.ToString(fun i -> " (")) 

            // we need a bit more fancy formatting for the actual ToString()
            // lists should be tabbed & sublists should start on the next line
            override t.ToString() = 
                t.ToString(fun a -> (String.Format("\n{0}(", System.String('\t', a))))

            member private t.ToString(seed : int -> string) =
                let rec toString o (seed : int -> string) acc =
                    match o with
                    | Value v -> v.ToString()
                    | Operation (tp, mi) -> tp + "." + ((mi.GetCustomAttributes(typeof<PushOperationAttribute>, false)).[0] :?> PushOperationAttribute).Name
                    | PushList l -> 
                        match l with 
                        | [] -> "()"
                        | _ ->
                            let s = l |> List.fold(fun str e -> str + (toString e seed (acc + 1)) + " ") (seed acc)
                            s.Substring(1, s.Length - 2) + ")"
                toString t seed 0

            override t.Equals(o) =
                match o with
                | :? Push as push -> (push :> IComparable).CompareTo t = 0
                | _ -> false

            override t.GetHashCode() =
                // does not really convert "to string"
                // but in the case of lists trying to minimize recursion
                // by making the function tail-recursive
                let rec toString p = 
                    match p with
                    | Value v -> v.ToString()
                    | Operation (name, m) -> (name + "." + m.Name)
                    | PushList l -> 
                        match l with
                        | [] -> System.String.Empty
                        | _ -> toString l.Head + toString (PushList(l.Tail))

                (toString t).GetHashCode()

            static member op_Equality (left : Push, right : Push) =
                if left = Unchecked.defaultof<Push> then right = Unchecked.defaultof<Push>
                elif right = Unchecked.defaultof<Push> then left = Unchecked.defaultof<Push>
                else left.Equals(right)

            member t.toList = 
                match t with
                | Value v -> PushList [t]
                | Operation (n, m) -> PushList [t]
                | PushList l -> t

            member t.asPushList = 
                match t.toList with 
                | PushList l -> l
                | _ -> []

            member t.isList = 
                match t with
                | PushList l -> true
                | _ -> false

            member private t.foldIntoMapOfUniqueItems : Map<Push, int, Push> =
                match t with
                | PushList l -> 
                     l |>
                        List.fold 
                            (fun map e -> 
                                if not (map.ContainsKey(e)) then map.Add(e, 1) else map.Remove(e).Add(e, map.[e] + 1)) (Map<Push, int, Push>.Empty(t))
                | _ -> Map<Push, int, Push>.Empty(t)

            static member discrepancy t p =
                let diffElementOccurrence (map1 : Map<Push, int, Push>) (map2 : Map<Push, int, Push>) elem =
                    let inMap1 = if map1.ContainsKey elem then map1.[elem] else 0
                    let inMap2 = if map2.ContainsKey elem then map2.[elem] else 0
                    Math.Abs(inMap1 - inMap2)

                let union left right =
                    let rightSet = PushSet.Create(PushList(right), right)
                    let  leftSet = PushSet.Create(PushList(left), left)
                    PushSet.Union(leftSet, rightSet)  |> Seq.toList

                let getKeys (map : Map<Push, int, Push>) = map.ToList() |> List.map(fun (k, v) -> k)

                match t, p with
                | PushList l1, PushList l2 ->
                    let mapT, mapP = t.foldIntoMapOfUniqueItems, p.foldIntoMapOfUniqueItems
                    let lstDistinctT, lstDistinctP = mapT |> getKeys, mapP |> getKeys
                    let allElems = union lstDistinctT lstDistinctP
                    allElems |> List.sumBy 
                                    (fun e -> diffElementOccurrence mapT mapP e)
                | _ -> Math.Abs((t :> IComparable).CompareTo p)
            
    and
        PushSet = Set<Push, Push>
                