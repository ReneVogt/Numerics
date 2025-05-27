using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    /// <summary>
    /// Gets a number that indicates the sign (negative, positive, or zero) of the current <see cref="BigDecimal"/>.
    /// </summary>
    public int Sign => Mantissa.Sign;

    /// <inheritdoc/>
    public static BigDecimal Abs(BigDecimal value) => new(BigInteger.Abs(value.Mantissa), value.Exponent);

    /// <inheritdoc/>
    public static bool IsZero(BigDecimal value) => value.Mantissa.IsZero;
    /// <inheritdoc/>
    public static bool IsPositive(BigDecimal value) => BigInteger.IsPositive(value.Mantissa);
    /// <inheritdoc/>
    public static bool IsNegative(BigDecimal value) => BigInteger.IsNegative(value.Mantissa);

    /// <inheritdoc/>
    public static bool IsInteger(BigDecimal value) => value.Exponent >= 0;
    /// <inheritdoc/>
    public static bool IsEvenInteger(BigDecimal value) => IsInteger(value) && value.Mantissa.IsEven;
    /// <inheritdoc/>
    public static bool IsOddInteger(BigDecimal value) => IsInteger(value) && !value.Mantissa.IsEven;

    /// <inheritdoc/>
    public static bool IsCanonical(BigDecimal value) => true;
    
    /// <inheritdoc/>
    public static bool IsFinite(BigDecimal value) => true;
    /// <inheritdoc/>
    public static bool IsNaN(BigDecimal value) => false;
    /// <inheritdoc/>
    public static bool IsComplexNumber(BigDecimal value) => false;
    /// <inheritdoc/>
    public static bool IsRealNumber(BigDecimal value) => true;
    /// <inheritdoc/>
    public static bool IsImaginaryNumber(BigDecimal value) => false;
    /// <inheritdoc/>
    public static bool IsInfinity(BigDecimal value) => false;
    /// <inheritdoc/>
    public static bool IsPositiveInfinity(BigDecimal value) => false;
    /// <inheritdoc/>
    public static bool IsNegativeInfinity(BigDecimal value) => false;

    /// <inheritdoc/>
    public static bool IsNormal(BigDecimal value) => true;
    /// <inheritdoc/>
    public static bool IsSubnormal(BigDecimal value) => false;

    /// <inheritdoc/>
    public static BigDecimal MaxMagnitude(BigDecimal x, BigDecimal y) => Abs(x) >= Abs(y) ? x : y;
    /// <inheritdoc/>
    public static BigDecimal MaxMagnitudeNumber(BigDecimal x, BigDecimal y) => MaxMagnitude(x, y);
    /// <inheritdoc/>
    public static BigDecimal MinMagnitude(BigDecimal x, BigDecimal y) => Abs(x) < Abs(y) ? x : y;
    /// <inheritdoc/>
    public static BigDecimal MinMagnitudeNumber(BigDecimal x, BigDecimal y) => MinMagnitude(x, y);
}
