module Task4

type Expression =
    | Addition of Expression * Expression
    | Subtraction of Expression * Expression
    | Multiplication of Expression * Expression
    | Division of Expression * Expression
    | Number of int

// Generates endless sequence of prime numbers.
let generateSequence () =

    let isPrime x =
        let rec find i =
            if i >= x then true
            elif (x % i = 0) then false
            else find (i + 1)
        find 2

    let initializer i =
        let rec findPrime j acc =
            if acc = i + 1 then j - 1
            elif isPrime j then findPrime (j + 1) (acc + 1)
            else findPrime (j + 1) acc
        findPrime 2 0

    Seq.initInfinite initializer

