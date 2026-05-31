using Revo.Numerics;

namespace Numerics.Tests.RomanNumberTests;

public sealed partial class RomanNumberTests
{
    [Fact]
    public void ToRomanNumber_ThrowsOnZero() => Assert.Throws<ArgumentOutOfRangeException>(() => RomanNumber.ToRomanNumber(0));
    [Fact]
    public void ToRomanNumber_ThrowsOnNegative() => Assert.Throws<ArgumentOutOfRangeException>(() => RomanNumber.ToRomanNumber(-1));

    [Theory]
    [
        InlineData(1, "I"),
        InlineData(5, "V"),
        InlineData(10, "X"),
        InlineData(14, "XIV"),
        InlineData(19, "XIX"),
        InlineData(39, "XXXIX"),
        InlineData(40, "XL"),
        InlineData(44, "XLIV"),
        InlineData(49, "XLIX"),
        InlineData(50, "L"),
        InlineData(89, "LXXXIX"),
        InlineData(90, "XC"),
        InlineData(94, "XCIV"),
        InlineData(99, "XCIX"),
        InlineData(100, "C"),
        InlineData(399, "CCCXCIX"),
        InlineData(400, "CD"),
        InlineData(444, "CDXLIV"),
        InlineData(499, "CDXCIX"),
        InlineData(500, "D"),
        InlineData(899, "DCCCXCIX"),
        InlineData(900, "CM"),
        InlineData(944, "CMXLIV"),
        InlineData(999, "CMXCIX"),
        InlineData(1000, "M"),
        InlineData(1984, "MCMLXXXIV"),
        InlineData(1994, "MCMXCIV"),
        InlineData(2024, "MMXXIV"),
        InlineData(3999, "MMMCMXCIX"),
        InlineData(3339, "MMMCCCXXXIX"),
        InlineData(27891, "MMMMMMMMMMMMMMMMMMMMMMMMMMMDCCCXCI")
    ]
    public void ToRomanNumber_CorrectRepresentation(int number, string expected) => Assert.Equal(expected, number.ToRomanNumber());

}
