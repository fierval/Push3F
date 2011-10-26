namespace push.types

[<AutoOpen>]
module TypeExensions =

    // map extensions
    type Microsoft.FSharp.Collections.Map<'Key, 'Value when 'Key : comparison>  with
        member t.Replace (key, value) =
            t.Remove(key).Add(key, value)

        member t.Append (map) =
            ((t |> Map.toList) @ (map |> Map.toList)) |> Map.ofList    

        member t.KeyCollection = t |> Map.toList |> List.map (fun (key,value) -> key)

    // list extensions
    type Microsoft.FSharp.Collections.List<'T> with
        // like List.filter, only with an extra index argument
        static member filteri (filter : int -> 'T -> bool) (lst : 'T list) =
            lst |> List.mapi(fun i e -> (i, e)) |> List.filter(fun (i, e) -> filter i e) |> List.map(fun (i, e) -> e)

        // list resulting in removal of an element at index "index"
        static member remove index (lst : 'T list) =
            lst |> List.filteri(fun i e -> index <> i)

        // list of filtered elements together with their indices in the original list
        static member filteredList (filter : 'T -> bool) (lst : 'T list) =
            lst |> List.mapi(fun i e -> (i, e)) |> List.filter(fun (i, e) -> filter e) 

        // list of indices we get as the result of applying a filter
        static member filteredIndices (filter : 'T -> bool) (lst : 'T list) =
            lst |> List.filteredList filter |> List.map(fun (i, e) -> i)

        static member replace index (elem : 'T) (lst : 'T list) =
            lst |> List.mapi(fun i e -> (i, e)) |> List.partition (fun (i, e) -> i < index) 
            ||> (fun a b -> (a |> List.map(fun (i, e) -> e)) @ (elem :: (b.Tail |> List.map (fun (i, e) -> e))))