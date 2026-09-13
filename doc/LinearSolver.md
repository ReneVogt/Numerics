# LinearSolver

[Back to the project overview](../README.md)

`LinearSolver`, in `Revo.Numerics.Solvers`, solves square linear equation systems of the form `Ax = b` using `double` arithmetic. It uses LU decomposition with complete pivoting: both rows and columns can be reordered when selecting pivots. The returned solution is restored to the original variable order.

Use the static `Solve` method for a single system, or `Create` to factorize a matrix once and solve it for several right-hand sides.

## Solving a system

Consider:

```text
2x + y = 5
 x - y = 1
```

The solution is `x = 2`, `y = 1`:

```csharp
using System;
using Revo.Numerics.Solvers;

double[] coefficients =
[
    2,  1,
    1, -1
];
double[] rightHandSide = [5, 1];

double[] solution = LinearSolver.Solve(coefficients, rightHandSide);
Console.WriteLine(solution[0]); // 2
Console.WriteLine(solution[1]); // 1
```

Coefficients are supplied in **row-major order**: all coefficients of the first equation, then those of the second equation, and so on. For an `n × n` matrix, the entry at `(row, column)` is at `coefficients[row * n + column]`, with zero-based indices.

The length of `rightHandSide` determines `n` for the static method, and `coefficients` must contain exactly `n * n` entries. The result contains `n` values, one for each variable in its original order.

## Reusing a factorization

When the coefficient matrix stays the same, create an `ILinearSolver` and reuse it:

```csharp
using System;
using Revo.Numerics.Solvers;

double[] coefficients = [2, 1, 1, -1];
ILinearSolver solver = LinearSolver.Create(2, coefficients);

double[] first = solver.Solve([5, 1]);
double[] second = solver.Solve([8, 1]);

Console.WriteLine(string.Join(", ", first));  // 2, 1
Console.WriteLine(string.Join(", ", second)); // 3, 2
Console.WriteLine(solver.IsSingularMatrix);   // False
```

`Create` performs the decomposition and returns `ILinearSolver`. There is no public constructor. Each instance `Solve` reuses the decomposition and allocates a new solution array.

The coefficients are copied during creation; subsequent changes to the original array do not affect the solver. Neither the coefficient array nor the right-hand side is modified by the public operations.

For a dense `n × n` system, factorization takes roughly `O(n³)` time and `O(n²)` storage; each subsequent solve takes `O(n²)` time.

## Tolerance and numerical behavior

Both `Create` and the static `Solve` accept an optional `epsilon`. The default is `LinearSolver.DefaultEpsilon`, or **`1e-8`**. A value is considered zero when:

```text
Math.Abs(value) <= epsilon
```

This is an **absolute** tolerance, not a relative tolerance or a bound on the error in the solution. It influences pivot handling, singularity detection, and the classification of singular systems. Choose it with the scale of your data in mind; rescaling a system can change its classification at the same absolute tolerance.

```csharp
using System;
using Revo.Numerics.Solvers;

double[] coefficients = [1e-10];
Console.WriteLine(LinearSolver.Create(1, coefficients).IsSingularMatrix); // True

var solver = LinearSolver.Create(1, coefficients, epsilon: 1e-12);
Console.WriteLine(solver.IsSingularMatrix); // False
Console.WriteLine(solver.Solve([2e-10])[0]); // 2
```

`epsilon` must be finite and non-negative. Zero is allowed and uses exact zero comparisons, but floating-point roundoff still applies. Complete pivoting does not guarantee an accurate result for an ill-conditioned system. For numerical validation, compare the residual `Ax - b` against a tolerance appropriate to your application.

Use finite coefficients and right-hand-side values. The current implementation validates `epsilon` but does not reject NaN or infinity in the arrays.

## Singular systems

`Create` can return a solver for a singular matrix. `IsSingularMatrix` depends only on the coefficient matrix and the chosen tolerance; it cannot distinguish an inconsistent system from one with infinitely many solutions without a right-hand side.

Calling `Solve` on such a system throws `SingularMatrixException`. Inspect its `State` property:

| State | Meaning |
| --- | --- |
| `LinearEquationSystemState.NoSolution` | The equations are inconsistent. |
| `LinearEquationSystemState.InfiniteSolutions` | The equations are consistent but do not determine a unique solution. |

```csharp
using System;
using Revo.Numerics.Solvers;

// x + y = b[0], 2x + 2y = b[1]
var solver = LinearSolver.Create(2, [1, 1, 2, 2]);
Console.WriteLine(solver.IsSingularMatrix); // True

try
{
    solver.Solve([3, 6]);
}
catch (SingularMatrixException exception)
{
    Console.WriteLine(exception.State); // InfiniteSolutions
}

try
{
    solver.Solve([3, 7]);
}
catch (SingularMatrixException exception)
{
    Console.WriteLine(exception.State); // NoSolution
}
```

The solver returns only unique solutions; it does not return a parameterization of infinitely many solutions or a least-squares result for inconsistent systems.

## API summary and validation

| Member | Purpose |
| --- | --- |
| `LinearSolver.Solve(coefficients, rightHandSide, epsilon)` | Factorize and solve in one call. |
| `LinearSolver.Create(numberOfVariables, coefficients, epsilon)` | Return a reusable `ILinearSolver`. |
| `solver.Solve(rightHandSide)` | Solve using the stored factorization. |
| `solver.IsSingularMatrix` | Report singularity according to the configured tolerance. |
| `LinearSolver.DefaultEpsilon` | Default absolute zero tolerance. |

- Null arrays throw `ArgumentNullException`.
- Incorrect coefficient or right-hand-side lengths throw `ArgumentException`.
- A negative variable count, or a negative or non-finite `epsilon`, throws `ArgumentOutOfRangeException`.
- Singular systems throw `SingularMatrixException` when solving.
- An empty system is supported: `Create(0, [])` is not singular and `Solve([])` returns an empty array.
- Matrices must be square. Rectangular, sparse-specific, and `BigDecimal` solvers are not provided by this API.

Source: [LinearSolver](../src/Numerics/Numerics/Solvers/LinearSolver.cs), [ILinearSolver](../src/Numerics/Numerics/Solvers/ILinearSolver.cs), and [SingularMatrixException](../src/Numerics/Numerics/Solvers/SingularMatrixException.cs).
