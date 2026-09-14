# TrigonometricInterpolator

[Back to the project overview](../README.md)

`TrigonometricInterpolator`, in `Revo.Numerics.Interpolation`, constructs a trigonometric polynomial through supplied points using `double` arithmetic and a specified period. Create an `IInterpolator` with `Interpolators.InterpolateTrigonometric(x, y, period)` or `TrigonometricInterpolator.Create(x, y, period)`.

## Interpolating periodic data

For period `4`, the function `f(x) = 2 + 3 cos(πx/2) - 4 sin(πx/2) + 1.5 cos(πx)` has the following values:

```csharp
using System;
using Revo.Numerics.Interpolation;

double[] x = [0, 1, 2, 3];
double[] y = [6.5, -3.5, 0.5, 4.5];
IInterpolator interpolator = Interpolators.InterpolateTrigonometric(x, y, period: 4);

double[] coefficients = interpolator.Coefficients;
// Approximately [2, 0, 3, -4, 1.5, 0].

Console.WriteLine(interpolator.Evaluate(0.5)); // Approximately 1.2928932188134525
Console.WriteLine(interpolator.Evaluate(4.5)); // Same value, up to roundoff
```

The arrays contain paired coordinates: `x[i]` and `y[i]` belong to the same point. Coordinates need not be sorted, equally spaced, or confined to one period. Reorder both arrays together to preserve the pairs. The supplied period uses the same units as the X coordinates; it is not inferred from the data and need not be the function's smallest period.

## Coefficient convention

With `ω = 2π / period`, the function is represented as:

```text
f(x) = Σ (aₖ cos(kωx) + bₖ sin(kωx)),  k = 0, …, m
Coefficients = [a₀, b₀, a₁, b₁, …, aₘ, bₘ]
```

Element `2*k` is `aₖ`, and element `2*k + 1` is `bₖ`. The constant term is **a₀**, not `a₀/2`. Since `sin(0) = 0`, `b₀` is explicitly stored as zero.

For `n` input points, `m = floor(n/2)`:

| Point count | Fitted terms | Returned array length |
| --- | --- | --- |
| Odd: `n = 2m + 1` | Constant plus all cosine/sine pairs through harmonic `m` | `n + 1`, including `b₀ = 0` |
| Even: `n = 2m` | Constant plus pairs through harmonic `m - 1`, then `aₘ cos(mωx)` | `n + 2`, including `b₀ = 0` and `bₘ = 0` |

A single point produces `[y[0], 0]` and a constant function. Higher-harmonic coefficients remain in the array even if the data require fewer terms; fitted zeros may have small roundoff errors. The explicitly inserted `b₀` and, for even `n`, `bₘ` are exactly zero.

For even `n`, the highest sine term is excluded from the interpolation basis. Matching a reference function away from the nodes therefore requires that it be representable in this basis. Distinct phases alone do not guarantee a uniquely solvable system for arbitrary even-sized node sets.

## API summary and validation

| Member | Purpose |
| --- | --- |
| `Interpolators.InterpolateTrigonometric(x, y, period)` | Create a trigonometric interpolator exposed as `IInterpolator`. |
| `TrigonometricInterpolator.Create(x, y, period)` | Equivalent factory on the public implementation class. |
| `Coefficients` | Return a new array of complete cosine/sine coefficient pairs. |
| `Evaluate(x)` | Evaluate the periodic function at the supplied coordinate. |

- A null input array throws `ArgumentNullException`, identifying `x` or `y`.
- Empty arrays or arrays of different lengths throw `ArgumentException`.
- A period that is zero, negative, `NaN`, or infinite throws `ArgumentException`.
- The interpolation system must have a unique solution. The current implementation uses `LinearSolver` and throws `SingularMatrixException` when it detects a singular system. Its `State` distinguishes no solution from infinitely many solutions; see the [LinearSolver guide](LinearSolver.md).

There is currently no separate validation for duplicate X coordinates or coordinates with equal phase modulo the period. Such points make the mathematical system singular. Singularity detection is left to the solver and its numerical tolerance, rather than a dedicated duplicate-coordinate check.

Neither input array is modified or retained. Later changes to the inputs do not affect the interpolator. Every access to `Coefficients` returns a fresh copy; modifying that copy does not affect later coefficient access or evaluation.

## Numerical behavior and cost

Use finite coordinates and values. Unlike the period, `NaN` and infinity in the input arrays or the evaluation coordinate are not explicitly rejected. They can produce nonfinite or otherwise unusable results.

The implementation builds a dense interpolation matrix and solves it using LU decomposition with complete pivoting. Construction takes `O(n³)` time and `O(n²)` additional storage. Evaluation takes `O(n)` time and `O(1)` additional storage. Reading `Coefficients` takes `O(n)` time and allocates a new array.

Closely spaced phases, nearly singular node configurations, and large arguments can reduce numerical accuracy. Extremely small periods can also overflow the angular-frequency calculation. The solver uses an absolute tolerance for singularity detection, so mathematically distinct phases do not guarantee a numerically usable system.

Compare coefficients and evaluated values using suitable absolute/relative tolerances. The polynomial passes through the supplied points and repeats with the specified period in exact arithmetic. Evaluation outside the range of the supplied points follows this periodic continuation. Interpolation does not perform least-squares fitting or establish that the underlying data are actually periodic.

Source: [TrigonometricInterpolator](../src/Numerics/Numerics/Interpolation/TrigonometricInterpolator.cs), [Interpolators](../src/Numerics/Numerics/Interpolation/Interpolators.cs), and [IInterpolator](../src/Numerics/Numerics/Interpolation/IInterpolator.cs).
