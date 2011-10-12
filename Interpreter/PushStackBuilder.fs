namespace push.types

open push.stack

[<AutoOpen>]
module PushMonad =

    type PushMonad<'a when 'a :> list> = int * string * List<'a> * Stack<'a>

    type PushBuilder =
        member t.Bind (p : PushMonad<'a>, f : 'a -> PushMondad<'b>) =
            let n, key, stack = 
                match p with 
                | (n, key, _, stack) -> n, key, stack

            let result, leftOver = popMany n stack

            if result.Length > 0 then
                // stockTypes.Stacks <- stockTypes.Stacks.Replace(key, leftOver)
            
    

