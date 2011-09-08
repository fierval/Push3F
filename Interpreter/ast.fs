namespace push.parser

module Ast = 
    open System.Reflection
    open push.types.Type

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<CustomEquality;NoComparison>]
    type Push = 
        | Value of PushTypeBase
        | PushList of Push list
        | Operation of string * MethodInfo
        with 
            member private t.StructuredFormatDisplay = 
                match t with
                | Value i -> i.StructuredFormatDisplay
                | PushList l -> l :> obj
                | Operation (tp, mi) -> box ("\"" + mi.DeclaringType.Name + "." + tp + "\"")

            override t.Equals(o) =
                let rec areEq a1 a2 =
                    match a1, a2 with
                    | (Value v1, Value v2) -> v1.ToString() = v2.ToString()
                    | (Operation (o1, m1), Operation (o2, m2)) -> o1 = o2 && m2.Name = m2.Name
                    | (PushList l1, PushList l2) -> List.forall2 (fun e1 e2 -> areEq e1 e2) l1 l2
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
                        | _ -> toString l.Head

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
                let lst = 
                    match t with
                    | Value v -> PushList [t]
                    | Operation (n, m) -> PushList [t]
                    | PushList l -> t
                match lst with 
                | PushList l -> l
                | _ -> []

            member t.isList = 
                match t with
                | PushList l -> true
                | _ -> false
