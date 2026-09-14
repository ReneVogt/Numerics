using Revo.Numerics.Solvers;

namespace Revo.Numerics.Interpolation;

public sealed class TrigonometricInterpolator : IInterpolator
{
    readonly double _periodFactor;
    readonly double[] _coefficients;

    public double[] Coefficients => [.._coefficients];

    TrigonometricInterpolator(double[] x, double[] y, double period)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));
        if (x.Length != y.Length) throw new ArgumentException("Arrays must have the same length.");
        if (x.Length == 0) throw new ArgumentException("At least one point is required for interpolation.");
        if (period <= 0 || double.IsNaN(period) || double.IsInfinity(period)) throw new ArgumentException("Period must be finite positive.");

        _periodFactor = 2 * Math.PI / period;
        var n = x.Length; 
        var even = n % 2 == 0;
        var m = n >> 1;
        if (even) m--;

        var les = Enumerable.Range(0, n).SelectMany(CreateRow).ToArray();
        var coeffs = LinearSolver.Solve(les, y);
        _coefficients = even ? [coeffs[0], 0, .. coeffs[1..^0], 0] : [coeffs[0], 0, .. coeffs[1..^0]];
        
        IEnumerable<double> CreateRow(int row)
        {
            var xi = x[row] * _periodFactor;
            yield return 1;
            for (var k = 1; k <= m; k++)
            {
                (var sin, var cos) = Math.SinCos(k * xi);
                yield return cos;
                yield return sin;
            }
            if (even) yield return Math.Cos((m+1) * xi);
        }
    }

    public double Evaluate(double x)
    {
        var sum = 0d;
        var sx = x * _periodFactor;
        for (var i = 0; i<_coefficients.Length >> 1; i++)
        {
            var ci = i << 1;
            (var sin, var cos) = Math.SinCos(i * sx);
            sum += _coefficients[ci] * cos + _coefficients[ci+1] * sin;
        }
            
        return sum;
    }

    public static IInterpolator Create(double[] x, double[] y, double period) => new TrigonometricInterpolator(x, y, period);

}
