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

        let rec recPowerOfTwoList n i list =
            if i = -1 then
                list
            else
                recPowerOfTwoList n (i - 1) (power 2 (n + i) :: list)

        Some(recPowerOfTwoList n m [])

    

