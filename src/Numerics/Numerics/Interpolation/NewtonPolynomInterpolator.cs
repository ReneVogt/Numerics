namespace Revo.Numerics.Interpolation;

/// <summary>
/// Interpolates points using Newton divided differences and converts the result to monomial coefficients.
/// </summary>
/// <remarks>Instances are exposed through <see cref="Interpolators.InterpolateNewtonPolynom"/>.</remarks>
sealed class NewtonPolynomInterpolator : IInterpolator
{
    readonly double[] _coefficients;

    /// <inheritdoc/>
    public double[] Coefficients => [.. _coefficients];

    NewtonPolynomInterpolator(double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));
        if (x.Length != y.Length)
            throw new ArgumentException("The arrays must have the same length.");
        if (x.Length == 0)
            throw new ArgumentException("At least one point is required for interpolation.");

        _coefficients = ConvertNewtonToMonomial(x, CalculateNewtonCoefficients(x, y));
    }

    /// <inheritdoc/>
    public double Evaluate(double x) 
    {
        var sum = 0d;
        for(var i=0; i < _coefficients.Length; i++)
            sum += _coefficients[i] * Math.Pow(x, i);
        return sum;
    }

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

    /// <inheritdoc cref="Interpolators.InterpolateNewtonPolynom"/>
    public static IInterpolator Create(double[] x, double[] y) => new NewtonPolynomInterpolator(x, y);
}
