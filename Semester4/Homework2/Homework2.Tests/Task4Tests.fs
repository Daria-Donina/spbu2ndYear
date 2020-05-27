module Task4Tests

open NUnit.Framework
open Task4
open FsUnit

let testList = 
    [
        0, 2
        1, 3
        5, 13
        13, 43
    ] |> List.map (fun (i, result) -> TestCaseData(i, result))

[<TestCaseSource("testList")>]
[<Test>]
let generateSequenceTest i result = 
    let enum = generateSequence().GetEnumerator()

    let rec iElement j =
        if j = i then
            enum.Current |> should equal result
        else 
            enum.MoveNext() |> should be True
            iElement (j + 1)

    enum.MoveNext() |> should be True
    iElement 0



