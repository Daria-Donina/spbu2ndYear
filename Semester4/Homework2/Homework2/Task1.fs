module Task1

// Counts even elements of a list using List.map.
let mapEvenNumbersCount (list : List<int>) = 
    list |> List.map (fun x -> abs(x + 1) % 2) |> List.sum

// Counts even elements of a list using List.filter.
let filterEvenNumbersCount (list : List<int>) =
    list |> List.filter (fun x -> x % 2 = 0) |> List.length

// Counts even elements of a list using List.fold.
let foldEvenNumbersCount (list : List<int>) =
    list |> List.fold (fun acc x -> acc + abs(x + 1) % 2) 0