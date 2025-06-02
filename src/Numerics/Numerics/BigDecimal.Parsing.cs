using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    const NumberStyles _defaultNumberStyles = NumberStyles.Integer;

    /// <inheritdoc/>
    public static BigDecimal Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider) => TryParse(s, style, provider, out var result) ? result : throw new FormatException();
    /// <inheritdoc/>
    public static BigDecimal Parse(string s, NumberStyles style, IFormatProvider? provider) => Parse((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), style, provider);
    /// <inheritdoc/>
    public static BigDecimal Parse(ReadOnlySpan<char> s, IFormatProvider? provider) => Parse(s, _defaultNumberStyles, provider);
    /// <inheritdoc/>
    public static BigDecimal Parse(string s, IFormatProvider? provider) => Parse((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), _defaultNumberStyles, provider);
    /// <summary>
    /// Parses a string into a <see cref="BigDecimal"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <returns>The result of parsing s.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="s"/> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="s"/> is not in the correct format.</exception>
    public static BigDecimal Parse(string s) => Parse((s ?? throw new ArgumentNullException(nameof(s))).AsSpan(), _defaultNumberStyles, null);
    
    /// <inheritdoc/>
    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out BigDecimal result)
    {
        result = default;
        var exponentStart = s.IndexOf('E');
        if (exponentStart < 0)
        {
            if (!BigInteger.TryParse(s, style, provider, out var value)) return false;
            result = new(value);
            return true;
        }

        if (exponentStart < 2 || exponentStart >= s.Length - 1 || s[exponentStart-1] != ' ' ||
            !BigInteger.TryParse(s[0..(exponentStart-1)], style, provider, out var mantissa) ||
            !int.TryParse(s[(exponentStart+1)..], style, provider, out var exponent))
            return false;

        result = new(mantissa, exponent);
        return true;
    }
    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out BigDecimal result)
    {
        result = default;
        return s is not null && TryParse(s.AsSpan(), style, provider, out result);
    }
    /// <inheritdoc/>
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out BigDecimal result)
        => TryParse(s, _defaultNumberStyles, provider, out result);
    /// <inheritdoc/>
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out BigDecimal result)
    {
        result = default;
        return s is not null && TryParse(s.AsSpan(), _defaultNumberStyles, provider, out result);
    }
    /// <summary>
    /// Tries to parse a string into a <see cref="BigDecimal"/>.
    /// </summary>
    /// <param name="s">The string to parse.</param>
    /// <param name="result">When this method returns, contains the result of successfully parsing s or an
    /// undefined value on failure.</param>
    /// <returns><c>true</c> if s was successfully parsed; otherwise, <c>false</c>.</returns>
    public static bool TryParse([NotNullWhen(true)] string? s, [MaybeNullWhen(false)] out BigDecimal result)
    {
        result = default;
        return s is not null && TryParse(s.AsSpan(), _defaultNumberStyles, null, out result);
    }

    /// <inheritdoc/>
    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        charsWritten = 0;
        if (!MemoryExtensions.Equals(format, _expectedFormatSpecifier, StringComparison.Ordinal)) throw new FormatException();

        if (!Mantissa.TryFormat(destination, out charsWritten, _bigintFormatSpecifier, provider)) return false;
        if (Exponent == 0) return true;
        if (destination.Length < charsWritten + 3)
        {
            charsWritten = 0;
            return false;
        }

        destination[charsWritten++] = ' ';
        destination[charsWritten++] = 'E';

        if (!Exponent.TryFormat(destination[charsWritten..], out var exponentChars, _bigintFormatSpecifier, provider))
        {
            charsWritten = 0;
            return false;
        }

        charsWritten += exponentChars;
        return true;
    }

    const string _bigintFormatSpecifier = "D";
    const string _expectedFormatSpecifier = "E";

    public override string ToString() => ToString(_expectedFormatSpecifier, CultureInfo.CurrentCulture);
    /// <inheritdoc/>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        format ??= _expectedFormatSpecifier;

        if (format != _expectedFormatSpecifier) throw new FormatException();
        var s = Mantissa.ToString(_bigintFormatSpecifier, formatProvider);
        if (Exponent != 0) s += " E" + Exponent.ToString(_bigintFormatSpecifier, formatProvider);
        return s;
    }
}
