module Task3

type Expression =
    | Addition of Expression * Expression
    | Subtraction of Expression * Expression
    | Multiplication of Expression * Expression
    | Division of Expression * Expression
    | Number of int

// Calculates the value of an arithmetic expression parsing tree.
let calculate (expression: Expression) =
    let rec recCalculate localExp =
        match localExp with
        | Addition(exp1, exp2) -> recCalculate exp1 + recCalculate exp2 
        | Subtraction(exp1, exp2) -> recCalculate exp1 - recCalculate exp2
        | Multiplication(exp1, exp2) -> recCalculate exp1 * recCalculate exp2
        | Division(exp1, exp2) -> recCalculate exp1 / recCalculate exp2
        | Number(x) -> x
    recCalculate expression
