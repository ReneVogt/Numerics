using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Revo.Numerics;

public static class RomanNumber
{
    static readonly (string Literal, int value)[] _map =
    [
        ("MMM", 3000),
        ("MM", 2000),
        ("M", 1000),
        ("CM", 900),
        ("DCCC", 800),
        ("DCC", 700),
        ("DC", 600),
        ("D", 500),
        ("CD", 400),
        ("CCC", 300),
        ("CC", 200),
        ("C", 100),
        ("XC", 90),
        ("LXXX", 80),
        ("LXX", 70),
        ("LX", 60),
        ("L", 50),
        ("XL", 40),
        ("XXX", 30),
        ("XX", 20),
        ("X", 10),
        ("IX", 9),
        ("VIII", 8),
        ("VII", 7),
        ("VI", 6),
        ("V", 5),
        ("IV", 4),
        ("III", 3),
        ("II", 2),
        ("I", 1)
    ];

    public static string ToRomanNumber(this int number) 
    {
        if (number <= 0) throw new ArgumentOutOfRangeException(paramName: nameof(number), actualValue: number, message: "A romand number must be greater than zero.");

        var builder = new StringBuilder();
        foreach((var literal, var value) in _map)
        {
            while(number >= value)
            {
                builder.Append(literal);
                number -= value;
            }
        }

        return builder.ToString();
    }

    public static int Parse(string s) => TryParse(s ?? throw new ArgumentNullException(nameof(s)), out var number) ? number : throw new FormatException($"The string '{s}' is not a valid roman number.");
    public static bool TryParse([NotNullWhen(true)] string? s, out int number) => TryParse(s, RomanParseOptions.Default, out number);
    public static bool TryParse([NotNullWhen(true)] string? s, RomanParseOptions options, out int number)
    {
        number = 0;
        if (s is null or { Length: 0 }) return false;

        var position = 0;
        while (position < s.Length && s[position] == 'M')
        {
            number += 1000;
            position++;
        }

        if (!TryParseDigitGroup(s, ref position, 100, 'C', 'D', 'M', options, out var h)) return false;
        number += h;
        if (!TryParseDigitGroup(s, ref position, 10, 'X', 'L', 'C', options, out var t)) return false;
        number += t;
        if (!TryParseDigitGroup(s, ref position, 1, 'I', 'V', 'X', options, out var o)) return false;
        number += o;

        if (position != s.Length) return false;

        return true;
    }
    static bool TryParseDigitGroup(string s, ref int position, int factor, char one, char five, char ten, RomanParseOptions options, out int groupValue)
    {
        groupValue = 0;

        var allowSub = options.HasFlag(RomanParseOptions.AllowSubtractiveNotation);
        var allowAdd49 = options.HasFlag(RomanParseOptions.AllowAdditiveFourAndNine);

        // 9: IX / XC / CM
        if (allowSub && position + 1 < s.Length && s[position] == one && s[position + 1] == ten)
        {
            groupValue = 9 * factor;
            position += 2;
            return true;
        }

        // 9 alternative: VIIII / LXXXX / DCCCC
        if (allowAdd49 && position < s.Length && s[position] == five)
        {
            var k = position + 1;
            var count = 0;
            while (k < s.Length && s[k] == one && count < 4) { k++; count++; }
            if (count == 4)
            {
                groupValue = 9 * factor;
                position = k;
                return true;
            }
        }

        // 4: IV / XL / CD
        if (allowSub && position + 1 < s.Length && s[position] == one && s[position + 1] == five)
        {
            groupValue = 4 * factor;
            position += 2;
            return true;
        }

        // 4 alternative: IIII / XXXX / CCCC
        if (allowAdd49 && position + 3 < s.Length && s[position] == one && s[position + 1] == one && s[position + 2] == one && s[position + 3] == one)
        {
            groupValue = 4 * factor;
            position += 4;
            return true;
        }

        // 5..8: V + I{0..3}
        if (position < s.Length && s[position] == five)
        {
            position++;
            var count = 0;
            while (position < s.Length && s[position] == one && count < 3) { position++; count++; }
            groupValue = (5 + count) * factor;
            return true;
        }

        // 0..3: I{0..3}
        {
            var count = 0;
            while (position < s.Length && s[position] == one && count < 3) { position++; count++; }
            groupValue = count * factor;
            return true;
        }
    }
}
