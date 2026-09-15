namespace Revo.Numerics.Interpolation;

/// <summary>
/// Interpolates and evaluates points in Newton form, converting to monomial coefficients on demand.
/// </summary>
/// <remarks>Instances are exposed through <see cref="Interpolators.InterpolateNewtonPolynom"/>.</remarks>
sealed class NewtonPolynomInterpolator : IInterpolator
{
    readonly double[] _nodes;
    readonly double[] _newtonCoefficients;
    readonly Lazy<double[]> _coefficients;

    /// <inheritdoc/>
    public double[] Coefficients => [.. _coefficients.Value];

    NewtonPolynomInterpolator(double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));
        if (x.Length != y.Length)
            throw new ArgumentException("The arrays must have the same length.");
        if (x.Length == 0)
            throw new ArgumentException("At least one point is required for interpolation.");

        _nodes = [.. x];
        _newtonCoefficients = CalculateNewtonCoefficients(_nodes, y);
        _coefficients = new Lazy<double[]>(
            () => ConvertNewtonToMonomial(_nodes, _newtonCoefficients));
    }

    /// <inheritdoc/>
    public double Evaluate(double x) 
    {
        var value = _newtonCoefficients[^1];
        for (var i = _newtonCoefficients.Length - 2; i >= 0; i--)
            value = _newtonCoefficients[i] + (x - _nodes[i]) * value;
        return value;
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
