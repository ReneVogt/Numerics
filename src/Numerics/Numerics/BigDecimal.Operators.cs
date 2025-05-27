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
    public BigDecimal Add(BigDecimal summand) => throw new NotImplementedException();
    public static BigDecimal operator -(BigDecimal left, BigDecimal right) => left.Subtract(right);
    public BigDecimal Subtract(BigDecimal subtrahend) => throw new NotImplementedException();
    public static BigDecimal operator *(BigDecimal left, BigDecimal right) => left.MultiplyBy(right);
    public BigDecimal MultiplyBy(BigDecimal factor) => throw new NotImplementedException();
    public static BigDecimal operator /(BigDecimal left, BigDecimal right) => left.DivideBy(right);
    public BigDecimal DivideBy(BigDecimal divisor) => DivideBy(divisor, BigDecimalContext.Precision);
    public BigDecimal DivideBy(BigDecimal divisor, BigInteger precision) => throw new NotImplementedException();
    public static BigDecimal operator %(BigDecimal left, BigDecimal right) => left.ModulusBy(right);
    public BigDecimal ModulusBy(BigDecimal divisor) => ModulusBy(divisor, BigDecimalContext.Precision);
    public BigDecimal ModulusBy(BigDecimal divisor, BigInteger precision) => throw new NotImplementedException();

    public static BigDecimal operator +(BigDecimal value) => value;
    public static BigDecimal operator ++(BigDecimal value) => value + 1;
    public static BigDecimal operator -(BigDecimal value) => throw new NotImplementedException();
    public static BigDecimal operator --(BigDecimal value) => value - 1;

    public static bool operator ==(BigDecimal left, BigDecimal right) => left.Equals(right);
    public static bool operator !=(BigDecimal left, BigDecimal right) => !left.Equals(right);
    public static bool operator <(BigDecimal left, BigDecimal right) => left.CompareTo(right) < 0;
    public static bool operator >(BigDecimal left, BigDecimal right) => left.CompareTo(right) > 0;
    public static bool operator <=(BigDecimal left, BigDecimal right) => left.CompareTo(right) <= 0;
    public static bool operator >=(BigDecimal left, BigDecimal right) => left.CompareTo(right) >= 0;
}
