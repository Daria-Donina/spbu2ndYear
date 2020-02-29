module ReverseListTests

open NUnit.Framework
open ReverseList

[<TestCaseSource("testList")>]
[<Test>]
let reverseListTest list result = 
    Assert.AreEqual(result, reverseList list)

let testList = 
    [
        [], []
        [1; 2], [2; 1]
        [3; 7; 8], [8; 7; 3]
        [-5; 8; 3; 0; 1; 118; -8], [-8; 118; 1; 0; 3; 8; -5]
    ] |> List.map (fun (list, result) -> TestCaseData(list, result))
