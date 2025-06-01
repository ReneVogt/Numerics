using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    /// <inheritdoc/>
    public static bool TryConvertFromChecked<TOther>(TOther value, [MaybeNullWhen(false)] out BigDecimal result)
        where TOther : INumberBase<TOther>
    {
        result = _zero;
        switch(value)
        {
            case decimal v:
                result = new BigDecimal(v);
                return true;
            case int v:
                result = new BigDecimal((BigInteger)v);
                return true;
            case uint v:
                result = new BigDecimal((BigInteger)v);
                return true;
            case long v:
                result = new BigDecimal((BigInteger)v);
                return true;
            case ulong v:
                result = new BigDecimal((BigInteger)v);
                return true;
            case BigInteger v:
                result = new BigDecimal(v);
                return true;
        }

        return false;
    }
    /// <inheritdoc/>
    public static bool TryConvertFromSaturating<TOther>(TOther value, [MaybeNullWhen(false)] out BigDecimal result) 
        where TOther : INumberBase<TOther>
        => TryConvertFromChecked(value, out result);
    /// <inheritdoc/>
    public static bool TryConvertFromTruncating<TOther>(TOther value, [MaybeNullWhen(false)] out BigDecimal result) 
        where TOther : INumberBase<TOther>
        => TryConvertFromChecked(value, out result);

    static readonly BigInteger _decimalMaxValue = new(decimal.MaxValue);
    static readonly BigInteger _decimalMinValue = new(decimal.MinValue);
    static readonly BigInteger _intMaxValue = new(int.MaxValue);
    static readonly BigInteger _intMinValue = new(int.MinValue);
    static readonly BigInteger _uintMaxValue = new(uint.MaxValue);
    static readonly BigInteger _uintMinValue = new (uint.MinValue);
    static readonly BigInteger _longMaxValue = new(long.MaxValue);
    static readonly BigInteger _longMinValue = new(long.MinValue);
    static readonly BigInteger _ulongMaxValue = new(ulong.MaxValue);
    static readonly BigInteger _ulongMinValue = new (ulong.MinValue);

    static readonly Dictionary<Type, Func<BigDecimal, object?>> _checkedConversions = new()
    {
        [typeof(decimal)] = TryConvertToDecimalChecked,
        [typeof(int)] = TryConvertToIntChecked,
        [typeof(uint)] = TryConvertToUintChecked,
        [typeof(long)] = TryConvertToLongChecked,
        [typeof(ulong)] = TryConvertToUlongChecked,
        [typeof(BigInteger)] = TryConvertToBigIntegerChecked
    };
    static readonly Dictionary<Type, Func<BigDecimal, object?>> _saturatingConversions = new()
    {
        [typeof(decimal)] = TryConvertToDecimalSaturating,
        [typeof(int)] = TryConvertToIntSaturating,
        [typeof(uint)] = TryConvertToUintSaturating,
        [typeof(long)] = TryConvertToLongSaturating,
        [typeof(ulong)] = TryConvertToUlongSaturating,
        [typeof(BigInteger)] = TryConvertToBigIntegerSaturating
    };
    static readonly Dictionary<Type, Func<BigDecimal, object?>> _truncatingConversions = new()
    {
        [typeof(decimal)] = TryConvertToDecimalTruncating,
        [typeof(int)] = TryConvertToIntTruncating,
        [typeof(uint)] = TryConvertToUintTruncating,
        [typeof(long)] = TryConvertToLongTruncating,
        [typeof(ulong)] = TryConvertToUlongTruncating,
        [typeof(BigInteger)] = TryConvertToBigIntegerTruncating
    };

    /// <inheritdoc/>
    public static bool TryConvertToChecked<TOther>(BigDecimal value, [MaybeNullWhen(false)] out TOther result) where TOther : INumberBase<TOther>
    {
        result = default;
        if (!_checkedConversions.TryGetValue(typeof(TOther), out var conversion)) return false;
        var converted = conversion(value);
        if (converted is null) return false;
        result = (TOther)converted;
        return true;
    }
    /// <inheritdoc/>
    public static bool TryConvertToSaturating<TOther>(BigDecimal value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        result = default;
        if (!_saturatingConversions.TryGetValue(typeof(TOther), out var conversion)) return false;
        var converted = conversion(value);
        if (converted is null) return false;
        result = (TOther)converted;
        return true;
    }
    /// <inheritdoc/>
    public static bool TryConvertToTruncating<TOther>(BigDecimal value, [MaybeNullWhen(false)] out TOther result)
        where TOther : INumberBase<TOther>
    {
        result = default;
        if (!_truncatingConversions.TryGetValue(typeof(TOther), out var conversion)) return false;
        var converted = conversion(value);
        if (converted is null) return false;
        result = (TOther)converted;
        return true;
    }

    public static implicit operator BigDecimal(decimal value)
    {
        var s = TryConvertFromChecked(value, out var result);
        Debug.Assert(s);
        return result;
    }
    public static explicit operator decimal(BigDecimal value)
    {
        if (value > _decimalMaxValue) return decimal.MaxValue;
        if (value < _decimalMinValue) return decimal.MinValue;
        var result = TryConvertToDecimalTruncating(value);
        Debug.Assert(result is not null);
        return (decimal)result;
    }
    public static implicit operator BigDecimal(int value)
    {
        var s = TryConvertFromChecked(value, out var result);
        Debug.Assert(s);
        return result;
    }
    public static explicit operator int(BigDecimal value)
    {
        if (value > _intMaxValue) return int.MaxValue;
        if (value < _intMinValue) return int.MinValue;
        return (int)Shift(value.Mantissa, value.Exponent);
    }
    public static implicit operator BigDecimal(uint value)
    {
        var s = TryConvertFromChecked(value, out var result);
        Debug.Assert(s);
        return result;
    }
    public static explicit operator uint(BigDecimal value)
    {
        if (value > _uintMaxValue) return uint.MaxValue;
        if (value < _uintMinValue) return uint.MinValue;
        return (uint)Shift(value.Mantissa, value.Exponent);
    }
    public static implicit operator BigDecimal(long value)
    {
        var s = TryConvertFromChecked(value, out var result);
        Debug.Assert(s);
        return result;
    }
    public static explicit operator long(BigDecimal value)
    {
        if (value > _longMaxValue) return long.MaxValue;
        if (value < _longMinValue) return long.MinValue;
        return (long)Shift(value.Mantissa, value.Exponent);
    }
    public static implicit operator BigDecimal(ulong value)
    {
        var s = TryConvertFromChecked(value, out var result);
        Debug.Assert(s);
        return result;
    }
    public static explicit operator ulong(BigDecimal value)
    {
        if (value > _ulongMaxValue) return ulong.MaxValue;
        if (value < _ulongMinValue) return ulong.MinValue;
        return (ulong)Shift(value.Mantissa, value.Exponent);
    }
    public static implicit operator BigDecimal(BigInteger value)
    {
        var s = TryConvertFromChecked(value, out var result);
        Debug.Assert(s);
        return result;
    }
    public static explicit operator BigInteger(BigDecimal value) => (int)Shift(value.Mantissa, value.Exponent);

    public static object? TryConvertToDecimalChecked(BigDecimal value)
    {
        if (value > _decimalMaxValue) return null;
        if (value < _decimalMinValue) return null;
        if (value.Exponent < -28) return null;
        return SafelyConvertToDecimal(value);
    }
    public static object? TryConvertToDecimalSaturating(BigDecimal value)
    {
        if (value.Exponent < -28) return null;
        if (value > _decimalMaxValue) return decimal.MaxValue;
        if (value < _decimalMinValue) return decimal.MinValue;
        return SafelyConvertToDecimal(value);
    }
    public static object? TryConvertToDecimalTruncating(BigDecimal value)
    {
        if (value > _decimalMaxValue) return null;
        if (value < _decimalMinValue) return null;
        if (value.Exponent < -28)
        {
            var shift = value.Exponent + 28;
            var exponent = value.Exponent - shift;
            var mantissa = Shift(value.Mantissa, shift);
            return SafelyConvertToDecimal(new(mantissa, exponent));
        }
        return SafelyConvertToDecimal(value);
    }
    static decimal SafelyConvertToDecimal(BigDecimal value)
    {
        var mantissa = BigInteger.Abs(value.Mantissa);
        byte scale = 0;
        if (value.Exponent > 0)
            mantissa *= BigInteger.Pow(10, (int)value.Exponent);
        else if (value.Exponent < 0)
        {
            scale = unchecked((byte)(BigInteger.Abs(value.Exponent)));
            var remainder = BigInteger.Zero;               
            while(mantissa > _decimalMaxValue)
            {
                mantissa = BigInteger.DivRem(mantissa, 10, out remainder);
                scale--;
            }
            if (remainder >= 5) mantissa += 1;
            Debug.Assert(scale >= 0);
        }

        var low = unchecked((int)(uint)(mantissa & 0xFFFFFFFF));
        var middle = unchecked((int)(uint)((mantissa >> 32) & 0xFFFFFFFF));
        var high = unchecked((int)(uint)((mantissa >> 64) & 0xFFFFFFFF));

        return new Decimal(low, middle, high, value.Sign < 0, scale);
    }

    public static object? TryConvertToIntChecked(BigDecimal value)
    {
        if (value > _intMaxValue) return null;
        if (value < _intMinValue) return null;
        if (value.Exponent < 0) return null;
        return (int)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToIntSaturating(BigDecimal value)
    {
        if (value.Exponent < 0) return null;
        if (value > _intMaxValue) return int.MaxValue;
        if (value < _intMinValue) return int.MinValue;
        return (int)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToIntTruncating(BigDecimal value)
    {
        if (value > _intMaxValue) return null;
        if (value < _intMinValue) return null;
        return (int)Shift(value.Mantissa, value.Exponent);
    }

    public static object? TryConvertToUintChecked(BigDecimal value)
    {
        if (value > _uintMaxValue) return null;
        if (value < _uintMinValue) return null;
        if (value.Exponent < 0) return null;
        return (uint)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToUintSaturating(BigDecimal value)
    {
        if (value.Exponent < 0) return null;
        if (value > _uintMaxValue) return uint.MaxValue;
        if (value < _uintMinValue) return uint.MinValue;
        return (uint)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToUintTruncating(BigDecimal value)
    {
        if (value > _uintMaxValue) return null;
        if (value < _uintMinValue) return null;
        return (uint)Shift(value.Mantissa, value.Exponent);
    }

    public static object? TryConvertToLongChecked(BigDecimal value)
    {
        if (value > _longMaxValue) return null;
        if (value < _longMinValue) return null;
        if (value.Exponent < 0) return null;
        return (long)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToLongSaturating(BigDecimal value)
    {
        if (value.Exponent < 0) return null;
        if (value > _longMaxValue) return long.MaxValue;
        if (value < _longMinValue) return long.MinValue;
        return (long)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToLongTruncating(BigDecimal value)
    {
        if (value > _longMaxValue) return null;
        if (value < _longMinValue) return null;
        return (long)Shift(value.Mantissa, value.Exponent);
    }

    public static object? TryConvertToUlongChecked(BigDecimal value)
    {
        if (value > _ulongMaxValue) return null;
        if (value < _ulongMinValue) return null;
        if (value.Exponent < 0) return null;
        return (ulong)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToUlongSaturating(BigDecimal value)
    {
        if (value.Exponent < 0) return null;
        if (value > _ulongMaxValue) return ulong.MaxValue;
        if (value < _ulongMinValue) return ulong.MinValue;
        return (ulong)Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToUlongTruncating(BigDecimal value)
    {
        if (value > _ulongMaxValue) return null;
        if (value < _ulongMinValue) return null;
        return (ulong)Shift(value.Mantissa, value.Exponent);
    }

    public static object? TryConvertToBigIntegerChecked(BigDecimal value)
    {
        if (value.Exponent < 0) return null;
        if (value.Exponent > int.MaxValue) return null;
        return Shift(value.Mantissa, value.Exponent);
    }
    public static object? TryConvertToBigIntegerSaturating(BigDecimal value) => TryConvertToBigIntegerChecked(value);
    public static object? TryConvertToBigIntegerTruncating(BigDecimal value) => Shift(value.Mantissa, value.Exponent);
}
