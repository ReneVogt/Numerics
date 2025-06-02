using Revo.Numerics.Properties;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    /// <inheritdoc/>
    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        BigDecimal other => CompareTo(other),
        _ => throw new ArgumentException(message: string.Format(Resources.BigDecimal_InvalidCompareArgument, nameof(BigDecimal), obj.GetType().Name))
    };
    /// <inheritdoc/>
    public int CompareTo(BigDecimal other)
    {
        if (Mantissa.IsZero) return other.Mantissa.IsZero ? 0 : -other.Sign;
        if (other.Mantissa.IsZero) return Sign;

        var comparedSigns = Sign.CompareTo(other.Sign);
        if (comparedSigns != 0) return comparedSigns;

        var comparedExponents = Exponent.CompareTo(other.Exponent);        
        if (comparedExponents == 0) return Mantissa.CompareTo(other.Mantissa);

        if (comparedExponents < 0)
        {
            var diff = other.Exponent - Exponent;
            var m = other.Mantissa;
            while (diff-- > 0) m *= 10;
            return Mantissa.CompareTo(m);
        }
        else
        {
            var diff = Exponent - other.Exponent;
            var m = Mantissa;
            while (diff-- > 0) m *= 10;
            return m.CompareTo(other.Mantissa);
        }
    }

    /// <inheritdoc/>
    public override bool Equals([NotNullWhen(true)] object? obj) => obj is BigDecimal b && Equals(b);
    /// <inheritdoc/>
    public bool Equals(BigDecimal other) => Exponent.Equals(other.Exponent) && Mantissa.Equals(other.Mantissa);

    public static BigDecimal operator +(BigDecimal left, BigDecimal right) => left.Add(right);
    public BigDecimal Add(BigDecimal summand)
    {
        var (m1, m2, e) = Align(summand);
        return new(m1 + m2, e);
    }
    public static BigDecimal operator -(BigDecimal left, BigDecimal right) => left.Subtract(right);
    public BigDecimal Subtract(BigDecimal subtrahend)
    {
        var (m1, m2, e) = Align(subtrahend);
        return new(m1 - m2, e);
    }
    public static BigDecimal operator *(BigDecimal left, BigDecimal right) => left.MultiplyBy(right);
    public BigDecimal MultiplyBy(BigDecimal factor)
    {
        var (m1, m2, e) = Align(factor);
        return new(m1 * m2, 2 * e);
    }
    public static BigDecimal operator /(BigDecimal divident, BigDecimal divisor) => divident.DivideBy(divisor);
    public BigDecimal DivideBy(BigDecimal divisor) => DivideBy(divisor, BigDecimalContext.Precision);
    public BigDecimal DivideBy(BigDecimal divisor, int precision)
    {
        if (divisor.Mantissa.IsZero) throw new DivideByZeroException();
        var (m1, m2, _) = Align(divisor);
        m1 *= BigInteger.Pow(10, precision);
        return new BigDecimal(m1 / m2, -precision);
    }
    public static BigDecimal operator %(BigDecimal divident, BigDecimal divisor) => divident.ModulusBy(divisor);
    public BigDecimal ModulusBy(BigDecimal divisor)
    {
        if (divisor.Mantissa.IsZero) throw new DivideByZeroException();
        var (m1, m2, e) = Align(divisor);
        return new BigDecimal(m1 % m2, e);
    }
    public static (BigDecimal Quotient, BigDecimal Remainder) DivRem(BigDecimal divident, BigDecimal divisor) => DivRem(divident, divisor, BigDecimalContext.Precision);
    public static (BigDecimal Quotient, BigDecimal Remainder) DivRem(BigDecimal divident, BigDecimal divisor, int precision)
    {
        if (divisor.Mantissa.IsZero) throw new DivideByZeroException();
        var (m1, m2, e) = Align(divident, divisor);
        var remainder = new BigDecimal(m1 % m2, e);
        m1 *= BigInteger.Pow(10, precision);
        return (new BigDecimal(m1 / m2, -precision), remainder);
    }

    public BigDecimal Shift(int shift) => new (Mantissa, checked(Exponent + shift));

    public static BigDecimal operator +(BigDecimal value) => value;
    public static BigDecimal operator ++(BigDecimal value) => value.Add(1);
    public static BigDecimal operator -(BigDecimal value) => new (-value.Mantissa, value.Exponent);
    public static BigDecimal operator --(BigDecimal value) => value.Subtract(1);

    public static bool operator ==(BigDecimal left, BigDecimal right) => left.Equals(right);
    public static bool operator !=(BigDecimal left, BigDecimal right) => !left.Equals(right);
    public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;
    public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;
    public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;
    public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;

    public static BigDecimal operator <<(BigDecimal value, int shift) => value.Shift(shift);
    public static BigDecimal operator >>(BigDecimal value, int shift) => value.Shift(-shift);

    (BigInteger Mantissa1, BigInteger Mantissa2, int Exponent) Align(BigDecimal value) => Align(this, value);
    static (BigInteger Mantissa1, BigInteger Mantissa2, int Exponent) Align(BigDecimal value1, BigDecimal value2)
    {
        var comparedExponents = value1.Exponent.CompareTo(value2.Exponent);
        return comparedExponents == 0
            ? (value1.Mantissa, value2.Mantissa, value1.Exponent)
            : comparedExponents < 0
                ? (value1.Mantissa, value2.Mantissa * BigInteger.Pow(10, value2.Exponent - value1.Exponent), value1.Exponent)
                : (value1.Mantissa * BigInteger.Pow(10, value1.Exponent - value2.Exponent), value2.Mantissa, value2.Exponent);
    }

    static BigInteger Shift(BigInteger mantissa, int shift) => shift < 0
        ? mantissa / BigInteger.Pow(10, -shift)
        : mantissa * BigInteger.Pow(10, shift);

}
