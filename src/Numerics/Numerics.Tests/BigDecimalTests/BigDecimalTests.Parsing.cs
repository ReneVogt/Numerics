using Revo.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    [Theory]
    [MemberData(nameof(ProvideToStringTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void ToString_Tests(TestableBigDecimal value, string expected) => Assert.Equal(expected, value.Value.ToString());
    [Theory]
    [MemberData(nameof(ProvideValidParsingTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void Parsing_Valid(TestableBigDecimal expected, string s) => Assert.Equal(expected.Value, BigDecimal.Parse(s));
    [Theory]
    [MemberData(nameof(ProvideInvalidParsingTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void Parsing_Invalid(string s) => Assert.Throws<FormatException>(() => BigDecimal.Parse(s));
    [Theory]
    [MemberData(nameof(ProvideRoundTripTestCases))]
    [Trait("BigDecimal", "Parsing")]
    public void Parsing_RoundTrip(TestableBigDecimal value) => Assert.Equal(value.Value, BigDecimal.Parse(value.Value.ToString()));

    public static TheoryData<TestableBigDecimal, string> ProvideToStringTestCases() => new()
    {
        { BigDecimal.Zero, "0" },
        { BigDecimal.One, "1" },
        { new BigDecimal(-1m), "-1" },
        { new BigDecimal(0.3m), "3 E-1" },
        { new BigDecimal(-12.001m), "-12001 E-3" },
        { new BigDecimal(123000m), "123 E3" }
    };
    public static TheoryData<TestableBigDecimal, string> ProvideValidParsingTestCases() => new()
    {
        { BigDecimal.Zero, "0" }
    };
    public static TheoryData<string> ProvideInvalidParsingTestCases() => new()
    {
        "1E3",
        "1E-3",
        "E1"
    };
    public static TheoryData<TestableBigDecimal> ProvideRoundTripTestCases() =>
    [
        BigDecimal.Zero,
        BigDecimal.One,
        new BigDecimal(-1m),
        new BigDecimal(999, 128),
        new BigDecimal(12, -34),
        new BigDecimal(-78912, 12334),
        new BigDecimal(-12000, -34),
        new BigDecimal(10000000, 30)
    ];
}
