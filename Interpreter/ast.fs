namespace push.parser

[<AutoOpen>]
module Ast = 
    open System
    open System.Reflection
    open push.types
    open System.Diagnostics
    open System.Collections.Generic
    open System.Linq

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
                let rec areEq a1 a2 =
                    match a1, a2 with
                    | (Value v1, Value v2) -> Push.compareValues v1 v2 = 0
                    | (Operation (o1, m1), Operation (o2, m2)) -> o1 = o2 && m2.Name = m2.Name
                    | (PushList l1, PushList l2) -> 
                        match l1, l2 with
                        | [], [] -> true
                        | lst1, lst2 -> 
                            if lst1.Length <> lst2.Length then false else List.forall2 (fun e1 e2 -> areEq e1 e2) l1 l2
                    | _ -> false
            
                match o with
                | :? Push as push -> areEq t push
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

            member private t.foldIntoMapOfUniqueItems : Dictionary<Push, int> =
                match t with
                | PushList l -> 
                    l.ToList().Aggregate(new Dictionary<Push, int>((t :> IEqualityComparer<Push>)), 
                        (fun acc e -> 
                            if not (acc.ContainsKey(e)) 
                            then 
                                acc.Add(e, 1) 
                            else 
                                acc.[e] <- acc.[e] + 1
                            acc)
                            )
                | _ -> new Dictionary<Push, int>()

            static member discrepancy t p =

                let intersect left right =
                    let rightSet = HashSet(right, (PushList(right) :> IEqualityComparer<Push>))
                    let  leftSet = HashSet(left, (PushList(left) :> IEqualityComparer<Push>))
                    leftSet.Intersect(rightSet) |> Seq.toList

                let except left right =
                    let rightSet = HashSet(right, (PushList(right) :> IEqualityComparer<Push>))
                    let  leftSet = HashSet(left, (PushList(left) :> IEqualityComparer<Push>))
                    leftSet.Except(rightSet) |> Seq.toList

                match t, p with
                | PushList l1, PushList l2 ->
                    let mapT, mapP = t.foldIntoMapOfUniqueItems, p.foldIntoMapOfUniqueItems
                    let lstDistinctT, lstDistinctP = mapT.Keys, mapP.Keys
                    let distinctElems = 
                        (except (lstDistinctT |> Seq.toList) (lstDistinctP |> Seq.toList)).Length + (except (lstDistinctP  |> Seq.toList) (lstDistinctT |> Seq.toList)).Length
                    let commonElems = intersect (lstDistinctT |> Seq.toList) (lstDistinctP |> Seq.toList)
                    let distinction = commonElems |> List.sumBy (fun e -> Math.Abs(mapT.[e] - mapP.[e]))
                    distinction + distinctElems                                        
                | _ -> Math.Abs((t :> IComparable).CompareTo p)
