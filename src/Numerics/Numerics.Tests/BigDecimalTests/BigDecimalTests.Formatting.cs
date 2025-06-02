using Revo.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    [Fact]
    [Trait("BigDecimal", "Formatting")]
    public void ToString_WrongFormat_FormatException()
    {
        Assert.Throws<FormatException>(() => BigDecimal.One.ToString("D", null));
        Assert.Throws<FormatException>(() => BigDecimal.One.ToString("N", null));
        Assert.Throws<FormatException>(() => BigDecimal.One.ToString("E02", null));
    }
    [Theory]
    [MemberData(nameof(ProvideToStringTestCases))]
    [Trait("BigDecimal", "Formatting")]
    public void ToString_FormattedCorrectly(TestableBigDecimal value, string expected)
        => Assert.Equal(expected, value.Value.ToString());

    public static TheoryData<TestableBigDecimal, string> ProvideToStringTestCases() => new()
    {
        {BigDecimal.Zero, "0"},
        {BigDecimal.One, "1"},
        {BigDecimal.NegativeOne, "-1"},
        {new BigDecimal(1234), "1234" },
        {new BigDecimal(-12345), "-12345" },
        {new BigDecimal(23, 17), "23 E17" },
        {new BigDecimal(-23, 17), "-23 E17" },
        {new BigDecimal(23, -17), "23 E-17" },
        {new BigDecimal(-23, -17), "-23 E-17" }
    };

    [Fact]
    [Trait("BigDecimal", "Formatting")]
    [Trait("BigDecimal", "Formatting")]
    public void TryFormat_WrongFormat_FormatException()
    {
        Assert.Throws<FormatException>(() => BigDecimal.One.TryFormat(new char[256], out _ , "N", null));
        Assert.Throws<FormatException>(() => BigDecimal.One.TryFormat(new char[256], out _ , "E02", null));
    }
    [Theory]
    [MemberData(nameof(ProvideFailingTryFormatTestCases))]
    [Trait("BigDecimal", "Formatting")]
    public void TryFormat_Failing(TestableBigDecimal value, int size)
    {
        Assert.False(value.Value.TryFormat(new char[size], out var written, "E", null));
        Assert.Equal(0, written);
    }
    [Theory]
    [MemberData(nameof(ProvideSuccessfulTryFormatTestCases))]
    [Trait("BigDecimal", "Formatting")]
    public void TryFormat_Successful(TestableBigDecimal value, int size, string expected)
    {
        var buffer = new char[size];
        Assert.True(value.Value.TryFormat(buffer, out var written, "E", null));
        var result = new string(buffer[..written]);
        Assert.Equal(expected, result);

        Assert.True(value.Value.TryFormat(buffer, out written, string.Empty, null));
        result = new string(buffer[..written]);
        Assert.Equal(expected, result);
    }

    public static TheoryData<TestableBigDecimal, int> ProvideFailingTryFormatTestCases() => new()
    {
        {BigDecimal.Zero, 0},
        {BigDecimal.One, 0},
        {BigDecimal.NegativeOne, 1},
        {new BigDecimal(1234), 3 },
        {new BigDecimal(-12345), 5 },
        {new BigDecimal(23, 17), 5 },
        {new BigDecimal(-23, 17), 6 },
        {new BigDecimal(23, -17), 2 },
        {new BigDecimal(-23, -17), 7 }
    };
    public static TheoryData<TestableBigDecimal, int, string> ProvideSuccessfulTryFormatTestCases() => new()
    {
        {BigDecimal.Zero, 1, "0"},
        {BigDecimal.One, 1, "1"},
        {BigDecimal.NegativeOne, 2, "-1"},
        {new BigDecimal(1234), 4, "1234" },
        {new BigDecimal(-12345), 6, "-12345" },
        {new BigDecimal(23, 17), 6, "23 E17" },
        {new BigDecimal(-23, 17), 7, "-23 E17" },
        {new BigDecimal(23, -17), 7, "23 E-17" },
        {new BigDecimal(-23, -17), 8, "-23 E-17" }
    };
}
