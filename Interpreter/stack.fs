﻿namespace push.stack

module Stack =
    open push.types.Type
    open push.exceptions.PushException

    type Stack<'a> =
        | EmptyStack
        | StackNode of 'a * 'a Stack
 
    let hd = function
        | EmptyStack -> raise (PushException( "Empty stack"))
        | StackNode(hd, tl) -> hd
 
    let tl = function
        | EmptyStack -> raise (PushException( "Empty stack"))
        | StackNode(hd, tl) -> tl
 
    let push hd tl = StackNode(hd, tl)
 
    let empty = EmptyStack

    let pop = function
        | EmptyStack -> raise (PushException("Empty stack"))
        | StackNode(hd, tl) -> hd, tl
    
    
    let rec internal reverseTail stack acc =
            match stack with
            | EmptyStack -> acc
            | StackNode(h, t) -> reverseTail t (push hd acc)
 
    // reverses the stack
    let reverse = function
        | EmptyStack -> EmptyStack
        | StackNode (hd, tl) -> reverseTail (StackNode(hd, tl)) empty

    // append expresses itself neatly in terms of reverse
    // it is an expensive operation, though.
    let append st1 st2 = 
        match st1 with
        | EmptyStack -> st2
        | StackNode(hd, tl) -> reverseTail (reverse st1) st2

    let internal yankOrDup n shouldDup = function
        | EmptyStack -> raise (PushException("Empty stack"))       
        | StackNode(hd, tl) -> 
            if n < 0 
            then 
                raise (PushException ("Empty stack"))
            let rec yankTail stack index headAcc tailAcc =
                match index with
                | 0 -> 
                    let mutable hd, tl = pop tailAcc
                    let head = push hd (reverse headAcc)
                    if shouldDup then tl <- push hd tl
                    append head tl
                | n -> 
                    match stack with
                    | EmptyStack -> raise (PushException("Empty Stack"))
                    | StackNode(h, t) -> yankTail t (index - 1) (push h headAcc) t
            yankTail (StackNode(hd, tl)) n empty (StackNode(hd, tl))

    let dup n stack = yankOrDup n true stack
    let yank n stack = yankOrDup n false stack