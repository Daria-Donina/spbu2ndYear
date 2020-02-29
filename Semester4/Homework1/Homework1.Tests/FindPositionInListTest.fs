module FindPositionInListTest

open NUnit.Framework
open FindPositionInList

[<TestCaseSource("testList")>]
[<Test>]
let findPositionTest x list result = 
    Assert.AreEqual(result, findPosition x list)

let testList = 
    [
        3, [1; 3; 4; 5], Some(1)
        -5, [-6; -4; 0; -2; -5], Some(4)
        -6, [-6; -4; 0; -2; 3], Some(0)
    ] |> List.map (fun (x, list, result) -> TestCaseData(x, list, result))

[<TestCaseSource("testListIncorrect")>]
[<Test>]
let findPositionIncorrectTest x list =
    Assert.AreEqual(None, findPosition x list)

let testListIncorrect = 
    [
        0, []
        2, [1; 3; 5]
    ] |> List.map (fun (x, list) -> TestCaseData(x, list))

