using Revo.Numerics.Solvers;

namespace Revo.Numerics.Interpolation;

/// <summary>
/// Interpolates points with a trigonometric polynomial of a specified period using double-precision arithmetic.
/// </summary>
/// <remarks>
/// Use <see cref="Create"/> or <see cref="Interpolators.InterpolateTrigonometric"/> to create an instance.
/// The coefficients are obtained by solving a linear system in a sine and cosine basis.
/// </remarks>
public sealed class TrigonometricInterpolator : IInterpolator
{
    readonly double _periodFactor;
    readonly double[] _coefficients;

    /// <summary>
    /// Gets a copy of the trigonometric coefficients as consecutive cosine/sine pairs.
    /// </summary>
    /// <value>
    /// The array [a0, b0, a1, b1, ..., am, bm], where a0 is the constant term and b0 is zero.
    /// For angular frequency omega = 2*pi/period, ak multiplies cos(k*omega*x) and
    /// bk multiplies sin(k*omega*x). The constant term is not divided by two.
    /// </value>
    /// <remarks>
    /// For n points, m = floor(n/2). Odd n produces n+1 coefficients; even n produces n+2
    /// coefficients with bm set to zero. A single point produces [y[0], 0].
    /// Each access returns a new array. Modifying it does not affect the interpolator.
    /// </remarks>
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

    /// <summary>
    /// Evaluates the trigonometric polynomial at the specified coordinate.
    /// </summary>
    /// <param name="x">The X coordinate, in the same units as the input coordinates and period.</param>
    /// <returns>The sum of ak*cos(k*omega*x) + bk*sin(k*omega*x), where omega = 2*pi/period.</returns>
    /// <remarks>
    /// The function repeats with the specified period in exact arithmetic; x need not lie within
    /// the range of the supplied points. Use finite coordinates. Evaluation takes O(n) time
    /// and O(1) additional storage for n input points.
    /// </remarks>
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

    /// <summary>
    /// Creates a trigonometric interpolator through the supplied points with the specified period.
    /// </summary>
    /// <param name="x">The X coordinates of at least one point, in any order.</param>
    /// <param name="y">The corresponding Y coordinates, with the same length as <paramref name="x"/>.</param>
    /// <param name="period">A finite, strictly positive period, in the same units as <paramref name="x"/>.</param>
    /// <returns>
    /// An interpolator exposing complete cosine/sine coefficient pairs through <see cref="Coefficients"/>.
    /// A single point produces a constant function with value y[0].
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="x"/> or <paramref name="y"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The arrays are empty or have different lengths, or <paramref name="period"/> is not finite and strictly positive.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The interpolation system is singular according to the solver's tolerance and has no unique solution.
    /// </exception>
    /// <remarks>
    /// The points need not be equally spaced, but must define a uniquely solvable system in the chosen basis.
    /// For n = 2m+1 points, all sine and cosine terms through harmonic m are included.
    /// For n = 2m points, the highest harmonic includes only the cosine term.
    /// The input arrays are neither modified nor retained. Use finite coordinates; NaN and infinity
    /// in the arrays are not explicitly rejected. Duplicate coordinates and equal phases are not
    /// explicitly validated; singularity detection is performed by the linear solver.
    /// Construction takes O(n³) time and O(n²) additional storage. Roundoff and ill-conditioned
    /// interpolation systems can reduce accuracy.
    /// </remarks>
    public static IInterpolator Create(double[] x, double[] y, double period) => new TrigonometricInterpolator(x, y, period);

}
