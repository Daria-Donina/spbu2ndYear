module ReverseList

let reverseList list = 

    let rec recReverseList list newList =
        match list with
        | [] -> newList
        | head :: tail -> recReverseList tail (head :: newList)

    recReverseList list []

