using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    static readonly BigDecimal _zero = new(0, 0);
    static readonly BigDecimal _one = new(1, 0);
    static readonly BigDecimal _negativeOne = new(-1, 0);

    /// <summary>
    /// Gets a <see cref="BigDecimal"/> representing
    /// the value 1.
    /// </summary>
    public static BigDecimal One => _one;
    /// <summary>
    /// Gets a <see cref="BigDecimal"/> representing
    /// the value 0.
    /// </summary>
    public static BigDecimal Zero => _zero;
    /// <summary>
    /// Gets a <see cref="BigDecimal"/> representing
    /// the value -1.
    /// </summary>
    public static BigDecimal NegativeOne => _negativeOne;

    /// <summary>
    /// The radix of <see cref="BigDecimal"/>.
    /// This is always 10.
    /// </summary>
    public static int Radix => 10;

    /// <summary>
    /// Gets the additive identity (0) of the <see cref="BigDecimal"/> type.
    /// </summary>
    public static BigDecimal AdditiveIdentity => _zero;
    /// <summary>
    /// Gets the multiplicative identity (1) of the <see cref="BigDecimal"/> type.
    /// </summary>
    public static BigDecimal MultiplicativeIdentity => _one;

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
    public static bool IsEvenInteger(BigDecimal value) 
        => value.Exponent > 0 || value.Exponent == 0 && value.Mantissa.IsEven;
    /// <inheritdoc/>
    public static bool IsOddInteger(BigDecimal value) => value.Exponent == 0 && !value.Mantissa.IsEven;

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

    /// <summary>
    /// Gets the number of digits in the mantissa.
    /// </summary>
    public int NumberOfDigits => GetNumberOfDigits(Mantissa);
    static int GetNumberOfDigits(BigInteger i) => i.IsZero ? 1 : (int)BigInteger.Log10(BigInteger.Abs(i))+1;

    public IEnumerable<int> EnumerateIntegralDigits()
    {
        if (Mantissa.IsZero)
        {
            yield return 0;
            yield break;
        }

        var m = Shift(BigInteger.Abs(Mantissa), Exponent);
        if (m.IsZero)
        {
            yield return 0;
            yield break;
        }
        while(!m.IsZero)
        {
            (m, var r) = BigInteger.DivRem(m, 10);
            yield return (int)r;
        }
    }
    public IEnumerable<int> EnumerateFractionalDigits()
    {
        if (Exponent >= 0) yield break;        
        var m = Shift(1, -Exponent);
        var r = BigInteger.Remainder(BigInteger.Abs(Mantissa), m);
        m /= 10;
        while(m > 0)
        {
            (var d, r) = BigInteger.DivRem(r, m);
            yield return (int)d;
            m /= 10;
        }
    }
}
