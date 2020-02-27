module Fibonacci

let fibonacci n =

    let rec recFibonacci x acc1 acc2 i =
        if x < 0 then
            None
        else if i = x then
            Some(acc2)
        else 
            recFibonacci x acc2 (acc1 + acc2) (i + 1)

    recFibonacci n 1 0 0



