using System.Text;

namespace Revo.Numerics;

public static class RomanNumber
{
    static readonly (string Literal, ushort value)[] _map =
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

    public static string ToRomanNumber(this ushort number) 
    {
        if (number == 0) throw new ArgumentOutOfRangeException(paramName: nameof(number), actualValue: number, message: "A romand number must be greater than zero.");

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

    public static bool TryParse(string s, out ushort number)
    {
        number = 0;
        return false;
    }
    public static ushort Parse(string s) => TryParse(s, out var number) ? number : throw new FormatException($"The string '{s}' is not a valid roman number.");
}
