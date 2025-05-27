using System.Numerics;

namespace Revo.Numerics;

/// <summary>
/// Represents an arbitrarily large signed decimal
/// with an arbitrary precision.
/// </summary>
public readonly partial struct BigDecimal : INumber<BigDecimal>,
    ISignedNumber<BigDecimal>
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

    public BigInteger Mantissa { get; }
    public BigInteger Exponent { get; }

    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given value.
    /// </summary>
    /// <param name="value">The value that should be represented by the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(decimal value)
    {
        if (value == 0) return;

        var bits = decimal.GetBits(value);
        var low = new BigInteger((uint)bits[0]);
        var middle = new BigInteger((uint)bits[1]);
        var high = new BigInteger((uint)bits[2]);

        var isNegative = (bits[3] & 0x80000000) != 0;
        var scale = (bits[3] >> 16) & 0x7F;

        var mantissa = high << 64 | middle << 32 | low;
        if (isNegative) mantissa = -mantissa;

        var exponent = -scale;
       
        (Mantissa, Exponent) = Normalize(mantissa, exponent);
    }
    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given value.
    /// </summary>
    /// <param name="value">The value that should be represented by the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(int value) : this(value, 0)
    {
    }
    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given value.
    /// </summary>
    /// <param name="value">The value that should be represented by the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(uint value) : this(value, 0)
    {
    }
    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given value.
    /// </summary>
    /// <param name="value">The value that should be represented by the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(long value) : this(value, 0)
    {
    }
    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given value.
    /// </summary>
    /// <param name="value">The value that should be represented by the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(ulong value) : this(value, 0)
    {
    }
    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given value.
    /// </summary>
    /// <param name="value">The value that should be represented by the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(BigInteger value) : this(value, 0)
    {
    }
    /// <summary>
    /// Initielizes a new <see cref="BigDecimal"/> with the
    /// given <paramref name="mantissa"/> and <paramref name="exponent"/>.
    /// </summary>
    /// <param name="mantissa">The mantissa for the new <see cref="BigDecimal"/>.</param>
    /// <param name="exponent">The exponent for the new <see cref="BigDecimal"/>.</param>
    public BigDecimal(BigInteger mantissa, BigInteger exponent) => (Mantissa, Exponent) = Normalize(mantissa, exponent);

    static (BigInteger Mantissa, BigInteger Exponent) Normalize(BigInteger mantissa, BigInteger exponent)
    {
        if (mantissa.IsZero) return (0, 0);

        // We know, this is not the most efficient way,
        // but handling a BigInteger exponent is tricky.
        while(BigInteger.Remainder(mantissa, 10) == 0)
        {
            mantissa /= 10;
            exponent++;
        }

        return (mantissa, exponent);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Mantissa, Exponent);
}