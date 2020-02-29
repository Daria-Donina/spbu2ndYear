module FindPositionInList

let findPosition x list =
    
    let rec recFindPosition x list i = 
        match list with
        | [] -> None
        | head :: tail ->
            if x = head then
                Some(i)
            else 
                recFindPosition x tail (i + 1)

    recFindPosition x list 0
