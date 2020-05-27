module Task2Tests

open NUnit.Framework
open Task2
open FsUnit

let testList = 
    [
        Task2.Empty, Task2.Empty
        Node(1, Task2.Empty, Task2.Empty), Node(2, Task2.Empty, Task2.Empty)
        Node(3, Node(0, Task2.Empty, Task2.Empty), Task2.Empty), Node(6, Node(0, Task2.Empty, Task2.Empty), Task2.Empty)
        Node(-2, Task2.Empty, Node(-5, Task2.Empty, Task2.Empty)), Node(-4, Task2.Empty, Node(-10, Task2.Empty, Task2.Empty))
        Node(0, Node(1, Task2.Empty, Task2.Empty), Node(0, Task2.Empty, Task2.Empty)), Node(0, Node(2, Task2.Empty, Task2.Empty), Node(0, Task2.Empty, Task2.Empty)) 
        Node(3, Node(1, Task2.Empty, Node(2, Task2.Empty, Task2.Empty)), Node(4, Task2.Empty, Task2.Empty)), Node(6, Node(2, Task2.Empty, Node(4, Task2.Empty, Task2.Empty)), Node(8, Task2.Empty, Task2.Empty))
    ] |> List.map (fun (tree, result) -> TestCaseData(tree, result))

[<TestCaseSource("testList")>]
[<Test>]
let treeMapTest tree result = treeMap (fun x -> x * 2) tree |> should equal result

