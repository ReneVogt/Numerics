using Revo.Numerics.Properties;
using System.Diagnostics;
using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    /// <summary>
    /// Calculates the square root of the current
    /// <see cref="BigDecimal"/> using up to <see cref="BigDecimalContext.Precision"/>
    /// decimal digits.
    /// </summary>
    /// <returns>The square root of the current <see cref="BigDecimal"/> truncated at
    /// <see cref="BigDecimalContext.Precision"/> decimal digits.</returns>.
    public BigDecimal Sqrt() => Sqrt(BigDecimalContext.Precision);
    /// <summary>
    /// Calculates the square root of the current
    /// <see cref="BigDecimal"/> using up to <see cref="BigDecimalContext.Precision"/>
    /// decimal digits.
    /// </summary>
    /// <returns>The square root of the current <see cref="BigDecimal"/> truncated at
    /// <see cref="BigDecimalContext.Precision"/> decimal digits.</returns>.
    public BigDecimal Sqrt(int precision)
    {
        if (Mantissa < 0) throw new ArithmeticException(string.Format(Resources.BigDecimal_Sqrt_ArgumentOutOfRange, this));
        return CheckedSqrt(precision);
    }
    BigDecimal CheckedSqrt(int precision)
    {
        var mantissa = Mantissa;
        var exponent = Exponent;
        if ((exponent & 1) == 1)
        {
            mantissa *= 10;
            exponent -= 1;
        }
        exponent >>= 1;

        var precisionBuffer = Math.Abs(exponent) + 1;
        mantissa = Shift(mantissa, 2 * (precision + precisionBuffer));
        var sqrt = IntegerSquareRoot(mantissa);
        sqrt = Shift(sqrt, exponent - precisionBuffer);
        return new(sqrt, -precision);
    }
    static BigInteger IntegerSquareRoot(BigInteger i)
    {
        Debug.Assert(i.Sign >= 0);
        if (i.IsZero) return BigInteger.Zero;

        var x0 = i >> (int)(i.GetBitLength() >> 1); // initial guess
        var x1 = (x0 + i / x0) >> 1;
        while (x1 != x0)
        {
            x0 = x1;
            x1 = (x0 + i / x0) >> 1;
        }

        return x0;
    }

    /// <summary>
    /// Calculates the square root of the given
    /// <see cref="BigDecimal"/> using up to <paramref name="precision"/>
    /// decimal digits.
    /// </summary>
    /// <param name="value">The <see cref="BigDecimal"/> value to calculate the square root of.</param>
    /// <param name="precision">The number of decimal digits to generate.</param>
    /// <returns>The square root of the given <paramref name="value"/> truncated at
    /// <paramref name="precision"/> decimal digits.</returns>.
    public static BigDecimal Sqrt(BigDecimal value) => Sqrt(value, BigDecimalContext.Precision);
    /// <summary>
    /// Calculates the square root of the given
    /// <see cref="BigDecimal"/> using up to <paramref name="precision"/>#
    /// decimal digits.
    /// </summary>
    /// <param name="value">The <see cref="BigDecimal"/> value to calculate the square root of.</param>
    /// <param name="precision">The number of decimal digits to generate.</param>
    /// <returns>The square root of the given <paramref name="value"/> truncated at
    /// <paramref name="precision"/> decimal digits.</returns>.
    public static BigDecimal Sqrt(BigDecimal value, int precision)
    {
        if (value.Mantissa < 0)
            throw new ArgumentOutOfRangeException(message: string.Format(Resources.BigDecimal_Sqrt_ArgumentOutOfRange, value), paramName: nameof(value));

        return value.CheckedSqrt(precision);
    }
}
