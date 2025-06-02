using System.Numerics;

namespace Revo.Numerics;

/// <summary>
/// Represents an arbitrarily large signed decimal
/// with an arbitrary precision.
/// </summary>
public readonly partial struct BigDecimal : INumber<BigDecimal>,
    ISignedNumber<BigDecimal>
{
    public BigInteger Mantissa { get; }
    public int Exponent { get; }

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
    public BigDecimal(BigInteger mantissa, int exponent) => (Mantissa, Exponent) = Normalize(mantissa, exponent);

    static (BigInteger Mantissa, int Exponent) Normalize(BigInteger mantissa, int exponent)
    {
        if (mantissa.IsZero) return (0, 0);

        for(; ;)
        {
            var (t, r) = BigInteger.DivRem(mantissa, 10);
            if (!r.IsZero) return (mantissa, exponent);
            mantissa = t;
            checked { exponent++; }
        }
    }

    public void Deconstruct(out BigInteger mantissa, out int exponent) => (mantissa, exponent) = (Mantissa, Exponent);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Mantissa, Exponent);
}