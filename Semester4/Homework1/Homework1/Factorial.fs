module Factorial

let factorial n =

    let rec recFactorial x acc =
        if x < 0 then
            None
        else if x = 0 || x = 1 then
            Some(acc)
        else
            recFactorial (x - 1) (x * acc) 

    recFactorial n 1
