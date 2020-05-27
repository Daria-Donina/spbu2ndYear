module Task1Tests

open NUnit.Framework
open FsCheck
open Task1
open FsUnit

let testList = 
    [
        [], 0
        [0], 1
        [3], 0
        [4], 1
        [3; 6; 9], 1
        [-4], 1
        [-5], 0
        [-5; 8; 3; 0; 1; 118; -8], 4
    ] |> List.map (fun (list, result) -> TestCaseData(list, result))

[<TestCaseSource("testList")>]
[<Test>]
let mapTest list result = mapEvenNumbersCount list |> should equal result

[<TestCaseSource("testList")>]
[<Test>]
let filterTest list result = filterEvenNumbersCount list |> should equal result

[<TestCaseSource("testList")>]
[<Test>]
let foldTest list result = foldEvenNumbersCount list |> should equal result

let AreFunctionsEqual (list : list<int>) = 
    let filterResult = filterEvenNumbersCount list
    mapEvenNumbersCount list = filterResult && filterResult = foldEvenNumbersCount list

[<Test>]
let equivalenceTest () = 
    Check.QuickThrowOnFailure AreFunctionsEqual


