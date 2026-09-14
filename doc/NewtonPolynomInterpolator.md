# NewtonPolynomInterpolator

[Back to the project overview](../README.md)

`Interpolators.InterpolateNewtonPolynom`, in `Revo.Numerics.Interpolation`, creates an `IInterpolator` representing a polynomial through a set of points using `double` arithmetic. The internal `NewtonPolynomInterpolator` implementation first computes Newton divided differences, then converts them to **monomial coefficients in ascending order of power**:

```text
p(x) = a[0] + a[1] * x + a[2] * x² + … + a[n - 1] * x^(n - 1)
```

The interpolator's `Coefficients` property returns these monomial coefficients, rather than the intermediate Newton coefficients. Use `Evaluate(x)` to evaluate the polynomial directly.

## Interpolating points

For the points `(1, 0)`, `(2, 1)`, and `(3, 4)`, the interpolating polynomial is `p(x) = 1 - 2x + x²`:

```csharp
using System;
using Revo.Numerics.Interpolation;

double[] x = [1, 2, 3];
double[] y = [0, 1, 4];
IInterpolator interpolator = Interpolators.InterpolateNewtonPolynom(x, y);
double[] coefficients = interpolator.Coefficients;

Console.WriteLine(string.Join(", ", coefficients)); // 1, -2, 1

Console.WriteLine(interpolator.Evaluate(2.5)); // 2.25
```

The arrays contain paired coordinates: `x[i]` and `y[i]` belong to the same point. X coordinates must be distinct, but they need not be sorted or equally spaced. Reorder both arrays together to preserve the pairs.

For `n` points, `Coefficients` contains exactly `n` elements and the interpolator represents a polynomial of degree at most `n - 1`. Coefficients for higher powers are retained even when the polynomial has a lower degree; they are zero in exact arithmetic but may contain small roundoff errors.

Neither input array is modified or retained. Changing the input arrays after construction does not affect the interpolator. Each access to `Coefficients` returns a new, independent array; changing that array does not affect later coefficient access or `Evaluate`.

Construction takes `O(n²)` time and `O(n)` additional storage. Accessing `Coefficients` copies `n` elements. Evaluation sums the monomial terms using the stored coefficients.

## API summary and validation

| Member | Purpose |
| --- | --- |
| `Interpolators.InterpolateNewtonPolynom(x, y)` | Create an `IInterpolator` using Newton divided differences. |
| `IInterpolator.Coefficients` | For Newton interpolation, return a new array of monomial coefficients in ascending order of power. |
| `IInterpolator.Evaluate(x)` | Evaluate the interpolating function at the supplied coordinate. |

`NewtonPolynomInterpolator` is an internal implementation. Create instances through the public `Interpolators` factory and use the returned `IInterpolator`. The interface represents interpolating functions in general; the coefficient basis and order depend on the interpolation method.

- A null array throws `ArgumentNullException`, identifying `x` or `y`.
- Arrays of different lengths throw `ArgumentException`.
- Repeated X coordinates throw `ArgumentException`, even if their Y coordinates agree or the repeated entries are not adjacent. Positive and negative zero count as the same X coordinate.
- Two empty arrays throw `ArgumentException`; at least one point is required.
- A single point produces coefficients `[y[0]]` and a constant polynomial, including away from the supplied X coordinate.

## Numerical behavior

Use finite coordinates. The implementation does not explicitly reject `NaN` or infinity in the arrays, and these can propagate into the result.

Duplicate X coordinates are detected by comparing their difference exactly to zero, without a tolerance. Very close but distinct coordinates are accepted; they can nevertheless amplify floating-point errors. High polynomial degrees, large coordinate magnitudes, and conversion to the monomial basis can also reduce accuracy or cause overflow. Changing point order can change rounding errors, although the mathematical interpolating polynomial is unchanged.

When checking results, compare both coefficients and evaluated values with absolute/relative tolerances suitable for the scale of your data. Interpolation passes through the supplied points in exact arithmetic; it does not perform least-squares fitting of noisy measurements. Evaluation outside the range of the points is extrapolation and may be inaccurate as an approximation of the underlying function.

Source: [Interpolators](../src/Numerics/Numerics/Interpolation/Interpolators.cs), [IInterpolator](../src/Numerics/Numerics/Interpolation/IInterpolator.cs), and [NewtonPolynomInterpolator](../src/Numerics/Numerics/Interpolation/NewtonPolynomInterpolator.cs).
