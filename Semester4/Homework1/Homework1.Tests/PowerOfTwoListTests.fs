module PowerOfTwoListTest

open NUnit.Framework
open PowerOfTwoList

[<TestCaseSource("testList")>]
[<Test>]
let powerOfTwoListTest n m result = 
    Assert.AreEqual(result, powerOfTwoList n m)

let testList = 
    [
        0, 0, Some([1])
        0, 2, Some([1; 2; 4])
        1, 3, Some([2; 4; 8; 16])
        2, 3, Some([4; 8; 16; 32])
        1, 6, Some([2; 4; 8; 16; 32; 64; 128])
    ] |> List.map (fun (n, m, result) -> TestCaseData(n, m, result))

[<TestCase(-1, 0)>]
[<TestCase(-6, -4)>]
[<TestCase(6, 4)>]
[<TestCase(5, 0)>]
[<Test>]
let powerOfTwoListIncorrectTest n m =
    Assert.AreEqual(None, powerOfTwoList n m)

