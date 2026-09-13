# NewtonPolynomInterpolator

[Back to the project overview](../README.md)

`NewtonPolynomInterpolator`, in `Revo.Numerics.Interpolation`, calculates a polynomial through a set of points using `double` arithmetic. It first computes Newton divided differences, then converts them to **monomial coefficients in ascending order of power**:

```text
p(x) = a[0] + a[1] * x + a[2] * x² + … + a[n - 1] * x^(n - 1)
```

The returned array contains these monomial coefficients, rather than the intermediate Newton coefficients.

## Interpolating points

For the points `(1, 0)`, `(2, 1)`, and `(3, 4)`, the interpolating polynomial is `p(x) = 1 - 2x + x²`:

```csharp
using System;
using Revo.Numerics.Interpolation;

double[] x = [1, 2, 3];
double[] y = [0, 1, 4];
double[] coefficients = NewtonPolynomInterpolator.Interpolate(x, y);

Console.WriteLine(string.Join(", ", coefficients)); // 1, -2, 1

// Evaluate the returned polynomial using Horner's method.
static double Evaluate(double[] coefficients, double x)
{
    double result = 0;
    for (int i = coefficients.Length - 1; i >= 0; i--)
        result = result * x + coefficients[i];
    return result;
}

Console.WriteLine(Evaluate(coefficients, 2.5)); // 2.25
```

The arrays contain paired coordinates: `x[i]` and `y[i]` belong to the same point. X coordinates must be distinct, but they need not be sorted or equally spaced. Reorder both arrays together to preserve the pairs.

For `n` points, the result contains exactly `n` coefficients and represents a polynomial of degree at most `n - 1`. Coefficients for higher powers are retained even when the polynomial has a lower degree; they are zero in exact arithmetic but may contain small roundoff errors.

Neither input array is modified. A nonempty result is a new array, independent of both inputs. The calculation takes `O(n²)` time and `O(n)` additional storage.

## API summary and validation

| Member | Purpose |
| --- | --- |
| `NewtonPolynomInterpolator.Interpolate(x, y)` | Compute the monomial coefficients using Newton divided differences. |
| `IPolynomInterpolator.Interpolate(x, y)` | Define the instance interpolation contract for implementations. |

`NewtonPolynomInterpolator` implements `IPolynomInterpolator` explicitly. Its current public entry point is the static method; the class has no public constructor or factory returning an interface instance.

- A null array throws `ArgumentNullException`, identifying `x` or `y`.
- Arrays of different lengths throw `ArgumentException`.
- Repeated X coordinates throw `ArgumentException`, even if their Y coordinates agree or the repeated entries are not adjacent. Positive and negative zero count as the same X coordinate.
- Two empty arrays return an empty coefficient array.
- A single point returns `[y[0]]`, representing a constant polynomial.

## Numerical behavior

Use finite coordinates. The implementation does not explicitly reject `NaN` or infinity in the arrays, and these can propagate into the result.

Duplicate X coordinates are detected by comparing their difference exactly to zero, without a tolerance. Very close but distinct coordinates are accepted; they can nevertheless amplify floating-point errors. High polynomial degrees, large coordinate magnitudes, and conversion to the monomial basis can also reduce accuracy or cause overflow. Changing point order can change rounding errors, although the mathematical interpolating polynomial is unchanged.

When checking results, compare both coefficients and evaluated values with absolute/relative tolerances suitable for the scale of your data. Interpolation passes through the supplied points in exact arithmetic; it does not perform least-squares fitting of noisy measurements. Evaluation outside the range of the points is extrapolation and may be inaccurate as an approximation of the underlying function.

Source: [NewtonPolynomInterpolator](../src/Numerics/Numerics/Interpolation/NewtonPolynomInterpolator.cs) and [IPolynomInterpolator](../src/Numerics/Numerics/Interpolation/IPolynomInterpolator.cs).
