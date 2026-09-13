namespace Revo.Numerics.Interpolation;

/// <summary>
/// Interpolates points using Newton divided differences and converts the result to monomial coefficients.
/// </summary>
/// <remarks>
/// Use the static <see cref="Interpolate"/> method to interpolate a set of points.
/// For n points, the calculation takes O(n²) time and O(n) additional storage.
/// </remarks>
public sealed class NewtonPolynomInterpolator : IPolynomInterpolator
{
    NewtonPolynomInterpolator() { }

    static double[] CalculateNewtonCoefficients(double[] x, double[] y)
    {
        var length = y.Length;
        var c = y.ToArray();
        for (var order = 1; order < length; order++)
        {
            for (var i = length - 1; i >= order; i--)
            {
                double denominator = x[i] - x[i - order];
                if (denominator == 0.0) throw new ArgumentException( "X coordinates must be distinct.");
                c[i] = (c[i] - c[i - 1]) / denominator;
            }
        }
        return c;
    }
    static double[] ConvertNewtonToMonomial(double[] x, double[] c)
    {
        var length = c.Length;
        var a = new double[length];

        a[0] = c[length- 1];

        var degree = 0;
        for (var k = length - 2; k >= 0; k--)
        {
            var root = x[k];

            for (var j = degree + 1; j >= 1; j--)            
                a[j] = a[j - 1] - root * a[j];            

            a[0] *= -root;
            a[0] += c[k];

            degree++;
        }

        return a;
    }

    /// <inheritdoc cref="IPolynomInterpolator.Interpolate"/>
    double[] IPolynomInterpolator.Interpolate(double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));        
        if (x.Length != y.Length)
            throw new ArgumentException("The arrays must have the same length.");

        if (x.Length == 0) return [];

        return ConvertNewtonToMonomial(x, CalculateNewtonCoefficients(x, y));
    }

    /// <summary>
    /// Calculates monomial coefficients of the interpolating polynomial using Newton divided differences.
    /// </summary>
    /// <param name="x">The distinct X coordinates of the points, in any order.</param>
    /// <param name="y">The corresponding Y coordinates, with the same length as <paramref name="x"/>.</param>
    /// <returns>
    /// An array a in ascending order of power, representing a[0] + a[1] * x + a[2] * x² + … .
    /// The array has the same length as the inputs; trailing coefficients are retained even for lower-degree polynomials.
    /// Empty inputs return an empty array; a single point returns its Y coordinate as the constant coefficient.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="x"/> or <paramref name="y"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The arrays have different lengths, or X coordinates are repeated.</exception>
    /// <remarks>
    /// Input arrays are not modified, and a nonempty result is newly allocated.
    /// Repeated X coordinates are detected using an exact zero comparison of their difference.
    /// Use finite coordinates; NaN and infinity are not explicitly rejected.
    /// Floating-point roundoff and ill-conditioned interpolation data can reduce accuracy, particularly
    /// when converting to the monomial basis.
    /// </remarks>
    public static double[] Interpolate(double[] x, double[] y) => ((IPolynomInterpolator)new NewtonPolynomInterpolator()).Interpolate(x, y);
}
