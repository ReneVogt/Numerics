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

Construction takes `O(n²)` time and `O(n)` additional storage. Monomial coefficients are computed lazily on first access in `O(n²)` time and cached; accessing `Coefficients` copies `n` elements. Evaluation uses nested Newton evaluation in `O(n)` time, independently of monomial conversion.

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
- Nonfinite coordinates in either input array throw `ArgumentException`, identifying `x` or `y`.
- A required Newton coefficient that overflows, or a nonzero coefficient that underflows to zero, causes construction to throw `ArithmeticException`. Nonzero subnormal coefficients are supported.
- Monomial conversion applies the same range checks when `Coefficients` is first accessed. Failure of this conversion does not prevent subsequent Newton evaluation.
- `Evaluate` rejects nonfinite coordinates with `ArgumentOutOfRangeException` and throws `ArithmeticException` if a required Newton evaluation step overflows. Evaluation results may underflow to zero.

## Numerical behavior

Use finite coordinates. Divided differences use separately scaled numerator and denominator differences, with powers of two and an extended intermediate exponent range. This avoids an overflowing difference destroying a representable quotient. For example, both the line through `(-1e308, -1), (1e308, 1)` and the line through `(-1, -1e308), (1, 1e308)` can be constructed and evaluated.

Nested Newton evaluation and monomial conversion use fused multiply-add operations, with scaled intermediate arithmetic where needed. This also avoids a product overflowing before an addition brings the result back into range. Stored Newton and monomial coefficients are still `double` values: this is not arbitrary-precision arithmetic, and intermediate coefficients or evaluation steps outside the supported range can still cause explicit numerical failure even when some final results are representable.

Duplicate X coordinates are detected by exact equality, without a tolerance. Very close but distinct coordinates are accepted; they can nevertheless amplify floating-point errors. High polynomial degrees, large coordinate magnitudes, and conversion to the monomial basis can also reduce accuracy or exceed the supported numerical range. Scaling avoids unnecessary range failures but does not fix ill-conditioning or eliminate rounding errors. Changing point order can change rounding errors, although the mathematical interpolating polynomial is unchanged.

When checking results, compare both coefficients and evaluated values with absolute/relative tolerances suitable for the scale of your data. Interpolation passes through the supplied points in exact arithmetic; it does not perform least-squares fitting of noisy measurements. Evaluation outside the range of the points is extrapolation and may be inaccurate as an approximation of the underlying function.

Source: [Interpolators](../src/Numerics/Numerics/Interpolation/Interpolators.cs), [IInterpolator](../src/Numerics/Numerics/Interpolation/IInterpolator.cs), and [NewtonPolynomInterpolator](../src/Numerics/Numerics/Interpolation/NewtonPolynomInterpolator.cs).
