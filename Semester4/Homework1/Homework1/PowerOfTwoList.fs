module PowerOfTwoList

let powerOfTwoList n m = 

    if n > m || n < 0 || m < 0 then
        None
    else
        let rec power x y =
            match y with
            | 0 -> 1
            | _ when y % 2 = 1 -> (power x (y - 1)) * x
            | _ ->
                let b = power x (y / 2)
                b * b

        let currentElementToAdd = power 2 (n + m)

        let rec recPowerOfTwoList i list current=
            if i = -1 then
                list
            else
                recPowerOfTwoList (i - 1) (current :: list) (current / 2)

        Some(recPowerOfTwoList m [] currentElementToAdd)

    

