module Task1Tests

open NUnit.Framework
open FsUnit
open Task1

[<TestCaseSource("testList")>]
[<Test>]
let minItemTest list result = 
    minItem list |> should equal result

let testList () = 
    [
        [1], 1
        [2; 5; 3; 1; 15], 1
        [-4; 0; 3; 1; -6], -6
        [0; 5; 3; 1; 32], 0
    ] |> List.map (fun (list, result) -> TestCaseData(list, result))
