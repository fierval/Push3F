namespace push.stack

module Stack =
    open push.types.Type
    open push.exceptions.PushExceptions
    open System.Diagnostics;

    [<StructuredFormatDisplay("{StructuredFormatDisplay}")>]
    [<DebuggerDisplay("{StructuredFormatDisplay}")>]
    type Stack<'a> = 
        | StackNode of 'a list
        with
            member private t.StructuredFormatDisplay = 
                box(t.asList)
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

    let pop = function
        | StackNode([]) -> Unchecked.defaultof<'a>, StackNode([])
        | StackNode(hd::tl) -> hd, StackNode(tl)
    
    // reverses the stack
    let reverse = function
        | StackNode([]) -> StackNode([])
        | StackNode (x) -> StackNode(List.rev x)

    // append expresses itself neatly in terms of reverse
    // it is an expensive operation, though.
    let append (st1:Stack<'a>) (st2:Stack<'a>) = st1.asList @ st2.asList

    let dup n (stack : 'a Stack) = if n >= stack.length then stack else StackNode(stack.asList.[n]::stack.asList)
    let yank n (stack : 'a Stack) = 
        if n >= stack.length then stack
        else
            let newHd = stack.asList.[n]
            let rec splitList lst1 index acc1 acc2 = 
                match index with
                | 0 -> acc1, acc2
                | x -> 
                    match lst1 with 
                    | [] -> acc1, acc2
                    | hd::tl -> splitList tl (index - 1) (acc1 @ [hd]) tl
            let listHead, listTail = splitList stack.asList n List.empty List.empty
            if listTail.IsEmpty then StackNode(listHead) else
            let head, tail = pop (StackNode(listTail))
            let totalList = (head::listHead) @ tail.asList
            StackNode(totalList)