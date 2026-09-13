namespace Revo.Numerics.Interpolation;

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

    double[] IPolynomInterpolator.Interpolate(double[] x, double[] y)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));        
        if (x.Length != y.Length)
            throw new ArgumentException("The arrays must have the same length.");

        if (x.Length == 0) return [];

        return ConvertNewtonToMonomial(x, CalculateNewtonCoefficients(x, y));
    }

    public static double[] Interpolate(double[] x, double[] y) => ((IPolynomInterpolator)new NewtonPolynomInterpolator()).Interpolate(x, y);
}