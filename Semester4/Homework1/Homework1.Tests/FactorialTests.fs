module FactorialTests

open NUnit.Framework
open Factorial

[<TestCase(0, 1)>]
[<TestCase(1, 1)>]
[<TestCase(2, 2)>]
[<TestCase(3, 6)>]
[<TestCase(5, 120)>]
[<TestCase(10, 3628800)>]
[<Test>]
let factorialTest n result = 
    Assert.AreEqual(Some(result), factorial n)

[<TestCase(-1)>]
[<TestCase(-6)>]
[<Test>]
let factorialIncorrectTest n =
    Assert.AreEqual(None, factorial n)
