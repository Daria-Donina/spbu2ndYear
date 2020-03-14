module FibonacciTests

open NUnit.Framework
open Fibonacci

[<TestCase(0, 0)>]
[<TestCase(1, 1)>]
[<TestCase(2, 1)>]
[<TestCase(3, 2)>]
[<TestCase(5, 5)>]
[<TestCase(7, 13)>]
[<TestCase(10, 55)>]
[<Test>]
let fibonacciTest n result = 
    Assert.AreEqual(Some(result), fibonacci n)

[<TestCase(-1)>]
[<TestCase(-6)>]
[<Test>]
let fibonacciIncorrectTest n =
    Assert.AreEqual(None, fibonacci n)

