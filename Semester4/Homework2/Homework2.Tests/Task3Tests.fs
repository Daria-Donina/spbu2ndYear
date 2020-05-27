module Task3Tests

open NUnit.Framework
open Task3
open FsUnit

let testList = 
    [
        Number(0), 0
        Addition(Number(5), Number(6)), 11
        Subtraction(Number(-2), Number(5)), -7
        Addition(Multiplication(Number(6), Number(0)), Subtraction(Division(Number(5), Number(3)), Number(19))), -18
    ] |> List.map (fun (expression, result) -> TestCaseData(expression, result))

[<TestCaseSource("testList")>]
[<Test>]
let calculateTest expression result = calculate expression |> should equal result

