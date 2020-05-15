﻿module Task1

// Gets the minimum item of the int list.
let minItem (list : List<int>) =
    List.fold (fun x acc ->
        let newAcc = 
            match List.filter (fun i -> i < acc) [x] with
            |[x] -> x
            |_ -> acc
        newAcc) (List.head list) list