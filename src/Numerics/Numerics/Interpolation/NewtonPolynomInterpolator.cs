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
        if (x.Any(value => !double.IsFinite(value)))
            throw new ArgumentException("X coordinates must be finite.", nameof(x));
        if (y.Any(value => !double.IsFinite(value)))
            throw new ArgumentException("Y coordinates must be finite.", nameof(y));

        _nodes = [.. x];
        _newtonCoefficients = CalculateNewtonCoefficients(_nodes, y);
        _coefficients = new Lazy<double[]>(
            () => ConvertNewtonToMonomial(_nodes, _newtonCoefficients));
    }

    /// <inheritdoc/>
    public double Evaluate(double x) 
    {
        if (!double.IsFinite(x))
            throw new ArgumentOutOfRangeException(nameof(x), "The evaluation coordinate must be finite.");

        var value = _newtonCoefficients[^1];
        for (var i = _newtonCoefficients.Length - 2; i >= 0; i--)
        {
            var difference = x - _nodes[i];
            var next = Math.FusedMultiplyAdd(difference, value, _newtonCoefficients[i]);
            if (double.IsFinite(difference) && double.IsFinite(next))
                value = next;
            else
                value = ScaledNumber.MultiplyAdd(
                    ScaledNumber.Difference(x, _nodes[i]),
                    ScaledNumber.FromDouble(value),
                    ScaledNumber.FromDouble(_newtonCoefficients[i])).ToDouble(rejectUnderflow: false);
        }
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
                if (x[i] == x[i - order]) throw new ArgumentException("X coordinates must be distinct.");
                var numerator = ScaledNumber.Difference(c[i], c[i - 1]);
                var denominator = ScaledNumber.Difference(x[i], x[i - order]);
                c[i] = ScaledNumber.Divide(numerator, denominator).ToDouble();
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
                a[j] = MultiplyAddCoefficient(-root, a[j], a[j - 1]);

            a[0] = MultiplyAddCoefficient(-root, a[0], c[k]);

            degree++;
        }

        return a;
    }

    static double MultiplyAddCoefficient(double factor, double value, double addend)
    {
        var result = Math.FusedMultiplyAdd(factor, value, addend);
        if (double.IsFinite(result) && result != 0.0)
            return result;

        // A zero may be exact cancellation or underflow; distinguish them before storing it.
        return ScaledNumber.MultiplyAdd(
            ScaledNumber.FromDouble(factor),
            ScaledNumber.FromDouble(value),
            ScaledNumber.FromDouble(addend)).ToDouble();
    }

    // Only intermediate operations use an extended exponent range. Stored coefficients
    // remain doubles, so overflow and nonzero coefficients rounded to zero are errors.
    readonly struct ScaledNumber
    {
        readonly double _mantissa;
        readonly int _exponent;

        ScaledNumber(double mantissa, int exponent)
        {
            if (mantissa == 0.0)
            {
                _mantissa = 0.0;
                _exponent = 0;
                return;
            }

            var shift = Math.ILogB(Math.Abs(mantissa));
            _mantissa = Math.ScaleB(mantissa, -shift);
            _exponent = exponent + shift;
        }

        public static ScaledNumber FromDouble(double value) => new(value, 0);

        public static ScaledNumber Difference(double left, double right)
        {
            var a = FromDouble(left);
            var b = FromDouble(right);
            if (a._mantissa == 0.0) return new(-b._mantissa, b._exponent);
            if (b._mantissa == 0.0) return a;

            var exponent = Math.Max(a._exponent, b._exponent);
            return new(
                Math.ScaleB(a._mantissa, a._exponent - exponent) -
                Math.ScaleB(b._mantissa, b._exponent - exponent), exponent);
        }

        public static ScaledNumber Divide(ScaledNumber numerator, ScaledNumber denominator) =>
            new(numerator._mantissa / denominator._mantissa, numerator._exponent - denominator._exponent);

        public static ScaledNumber MultiplyAdd(ScaledNumber left, ScaledNumber right, ScaledNumber addend)
        {
            if (left._mantissa == 0.0 || right._mantissa == 0.0) return addend;

            var productExponent = left._exponent + right._exponent;
            if (addend._mantissa == 0.0)
                return new(left._mantissa * right._mantissa, productExponent);

            var exponent = Math.Max(productExponent, addend._exponent);
            return new(Math.FusedMultiplyAdd(
                Math.ScaleB(left._mantissa, productExponent - exponent),
                right._mantissa,
                Math.ScaleB(addend._mantissa, addend._exponent - exponent)), exponent);
        }

        public double ToDouble(bool rejectUnderflow = true)
        {
            var result = Math.ScaleB(_mantissa, _exponent);
            if (!double.IsFinite(result) || (rejectUnderflow && result == 0.0 && _mantissa != 0.0))
                throw new ArithmeticException("A required interpolation value is outside the representable double range.");
            return result;
        }
    }

    /// <inheritdoc cref="Interpolators.InterpolateNewtonPolynom"/>
    public static IInterpolator Create(double[] x, double[] y) => new NewtonPolynomInterpolator(x, y);
}
