namespace push.stack

module Stack =
    open push.exceptions.PushExceptions
    open System.Diagnostics;

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<DebuggerDisplay("{StructuredFormatDisplay}")>]
    type Stack<'a> = 
        | StackNode of 'a list
        with
            member private t.StructuredFormatDisplay = 
                let str = t.asList |> List.fold (fun st e -> st + e.ToString() + "; ") "("
                str.Substring(0, str.Length - 2) + ")" 
            member t.length =
                match t with
                | StackNode(x) -> x.Length
            member t.asList = 
                match t with StackNode(x) -> x

    let peek = function
        | StackNode([]) -> Unchecked.defaultof<'a>
        | StackNode(hd::tl) -> hd
 
    let tl = function
        | StackNode([]) -> []
        | StackNode(hd::tl) -> tl
 
    let push hd tl = 
        match tl with
        |StackNode(x) -> StackNode(hd::x)
 
    let empty = StackNode([])

    let popMany n (stack : Stack<'a>) =
        let noopReturn = [], stack
        if stack.length = 0 then noopReturn
        else
            match n with
            | x when x <= 0 || stack.length < n -> noopReturn
            | x -> 
                let rec popManyTail n st acc =
                    match n with
                    | 0 -> acc
                    | _ -> 
                        let hd, tl = List.head st, List.tail st
                        popManyTail (n - 1) tl (hd::fst acc, StackNode(tl))
                popManyTail n stack.asList ([],empty)

    let splitIntoLists n (stack : Stack<'a>) = 
        let res = popMany n stack
        fst res, (snd res).asList

    let pop = function
        | StackNode([]) -> Unchecked.defaultof<'a>, StackNode([])
        | StackNode(hd::tl) -> hd, StackNode(tl)
    
    // reverses the stack
    let reverse = function
        | StackNode([]) -> StackNode([])
        | StackNode (x) -> StackNode(List.rev x)

    let popManyReverse n stack = 
        match popMany n stack with
        | [], _ -> [], stack
        | lst, _ -> List.rev lst, stack

    // append expresses itself neatly in terms of reverse
    // it is an expensive operation, though.
    let append (st1:Stack<'a>) (st2:Stack<'a>) = st1.asList @ st2.asList

    let dup n (stack : 'a Stack) = if n >= stack.length then stack else StackNode(stack.asList.[n]::stack.asList)
    
    let yank n (stack : 'a Stack) = 
        if n >= stack.length then stack
        else
            let listHead, listTail = splitIntoLists n stack
            if listTail.IsEmpty then StackNode(listHead) else
            let head, tail = pop (StackNode(listTail))
            let totalList = (head::listHead) @ tail.asList
            StackNode(totalList)

    let stacksInit (map : Map<string, 'a>) = Map.map (fun t a -> empty) map