using Revo.Numerics;
using System.Globalization;
using System.Numerics;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    #region decimal
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimal_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.Equal(decimal.MaxValue, (decimal)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimal_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        var result = (decimal)sut;
        Assert.Equal(decimal.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimal_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(decimal.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.Equal(decimal.MinValue, (decimal)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimal_MinValue_MinValue()
    {
        var sut = new BigDecimal(decimal.MinValue);
        var result = (decimal)sut;
        Assert.Equal(decimal.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("10000000000")]
    [InlineData("0.0000000002")]
    [InlineData("0.0000000000000000000000000001")]
    [InlineData("-0.000000000000000002")]
    [InlineData("-0.0000000000002")]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimal_RoundTrip(string value)
    {
        var d = Decimal.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.Equal(d, (decimal)sut);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalChecked_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<decimal>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalChecked_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        Assert.True(BigDecimal.TryConvertToChecked<decimal>(sut, out var result));
        Assert.Equal(decimal.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalChecked_LessThanMinValue_False()
    {
        var sut = new BigDecimal(decimal.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<decimal>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalChecked_MinValue_MinValue()
    {
        var sut = new BigDecimal(decimal.MinValue);
        Assert.True(BigDecimal.TryConvertToChecked<decimal>(sut, out var result));
        Assert.Equal(decimal.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalChecked_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToChecked<decimal>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("10000000000")]
    [InlineData("0.0000000002")]
    [InlineData("0.0000000000000000000000000001")]
    [InlineData("-0.000000000000000002")]
    [InlineData("-0.0000000000002")]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalChecked_RoundTrip(string value)
    {
        var d = Decimal.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToChecked<decimal>(sut, out var result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalSaturating_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out decimal result));
        Assert.Equal(decimal.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalSaturating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out decimal result));
        Assert.Equal(decimal.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalSaturating_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(decimal.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out decimal result));
        Assert.Equal(decimal.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalSaturationg_MinValue_MinValue()
    {
        var sut = new BigDecimal(decimal.MinValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out decimal result));
        Assert.Equal(decimal.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalSaturating_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToSaturating<decimal>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("10000000000")]
    [InlineData("0.0000000002")]
    [InlineData("0.0000000000000000000000000001")]
    [InlineData("-0.000000000000000002")]
    [InlineData("-0.0000000000002")]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalSaturating_RoundTrip(string value)
    {
        var d = Decimal.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out decimal result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalTruncating_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating(sut, out decimal _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalTruncating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(decimal.MaxValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out decimal result));
        Assert.Equal(decimal.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalTruncating_LessThanMinValue_False()
    {
        var sut = new BigDecimal(decimal.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating<decimal>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalTruncating_MinValue_MinValue()
    {
        var sut = new BigDecimal(decimal.MinValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out decimal result));
        Assert.Equal(decimal.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("10000000000")]
    [InlineData("0.0000000002")]
    [InlineData("0.0000000000000000000000000001")]
    [InlineData("-0.000000000000000002")]
    [InlineData("-0.0000000000002")]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalTruncating_RoundTrip(string value)
    {
        var d = Decimal.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out decimal result));
        Assert.Equal(d, result);
    }
    [Theory]
    [MemberData(nameof(ProvideTruncatingDecimalTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void ToDecimalTruncating_Truncated(TestableBigDecimal value, decimal expected)
    {
        Assert.True(BigDecimal.TryConvertToTruncating(value.Value, out decimal result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ProvideFromDecimalTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void FromDecimal(decimal value, TestableBigDecimal expected)
    {
        Assert.True(BigDecimal.TryConvertFromTruncating(value, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromChecked(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromSaturating(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.Equal(expected.Value, (BigDecimal)value);
    }

    public static TheoryData<decimal, TestableBigDecimal> ProvideFromDecimalTestCases()
    {
        var data = new TheoryData<decimal, TestableBigDecimal>
        {
            { 123.00011122233344455566677789m, new BigDecimal(BigInteger.Parse("12300011122233344455566677789"), -26) },
            { -123.00011122233344455566677789m, new BigDecimal(BigInteger.Parse("-12300011122233344455566677789"), -26) }
        };
        return data;
    }

    public static TheoryData<TestableBigDecimal, decimal> ProvideTruncatingDecimalTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, decimal>
        {
            { new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30), 123.0001112223334445556667778889m },
            { new BigDecimal(BigInteger.Parse("-123000111222333444555666777888999"), -30), -123.0001112223334445556667778889m },
            { new BigDecimal(1, -30), 0m },
            { new BigDecimal(-1, -30), 0m }
        };
        return data;
    }
    #endregion
    #region int
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToInt_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(int.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.Equal(int.MaxValue, (int)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToInt_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(int.MaxValue);
        var result = (int)sut;
        Assert.Equal(int.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToInt_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(int.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.Equal(int.MinValue, (int)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToInt_MinValue_MinValue()
    {
        var sut = new BigDecimal(int.MinValue);
        var result = (int)sut;
        Assert.Equal(int.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("100000000")]
    [InlineData("-9875")]
    [Trait("BigDecimal", "Conversion")]
    public void ToInt_RoundTrip(string value)
    {
        var d = int.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.Equal(d, (int)sut);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(int.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<int>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(int.MaxValue);
        Assert.True(BigDecimal.TryConvertToChecked<int>(sut, out var result));
        Assert.Equal(int.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_LessThanMinValue_False()
    {
        var sut = new BigDecimal(int.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<int>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_MinValue_MinValue()
    {
        var sut = new BigDecimal(int.MinValue);
        Assert.True(BigDecimal.TryConvertToChecked<int>(sut, out var result));
        Assert.Equal(int.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToChecked<int>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-6789")]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_RoundTrip(string value)
    {
        var d = int.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToChecked<int>(sut, out var result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntSaturating_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(int.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out int result));
        Assert.Equal(int.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntSaturating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(int.MaxValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out int result));
        Assert.Equal(int.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntSaturating_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(int.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out int result));
        Assert.Equal(int.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntSaturationg_MinValue_MinValue()
    {
        var sut = new BigDecimal(int.MinValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out int result));
        Assert.Equal(int.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntSaturating_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToSaturating<int>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-9876")]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntSaturating_RoundTrip(string value)
    {
        var d = int.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out int result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntTruncating_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(int.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating(sut, out int _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntTruncating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(int.MaxValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out int result));
        Assert.Equal(int.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntTruncating_LessThanMinValue_False()
    {
        var sut = new BigDecimal(int.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating<int>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntTruncating_MinValue_MinValue()
    {
        var sut = new BigDecimal(int.MinValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out int result));
        Assert.Equal(int.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-9876")]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntTruncating_RoundTrip(string value)
    {
        var d = int.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out int result));
        Assert.Equal(d, result);
    }
    [Theory]
    [MemberData(nameof(ProvideTruncatingIntTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntTruncating_Truncated(TestableBigDecimal value, int expected)
    {
        Assert.True(BigDecimal.TryConvertToTruncating(value.Value, out int result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ProvideFromIntTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void FromInt(int value, TestableBigDecimal expected)
    {
        Assert.True(BigDecimal.TryConvertFromTruncating(value, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromChecked(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromSaturating(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.Equal(expected.Value, (BigDecimal)value);
    }

    public static TheoryData<int, TestableBigDecimal> ProvideFromIntTestCases()
    {
        var data = new TheoryData<int, TestableBigDecimal>
        {
            { 123000, new BigDecimal(123, 3) },
            { -987, new BigDecimal(-987, 0) }
        };
        return data;
    }

    public static TheoryData<TestableBigDecimal, int> ProvideTruncatingIntTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, int>
        {
            { new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30), 123},
            { new BigDecimal(BigInteger.Parse("-123000111222333444555666777888999"), -30), -123},
            { new BigDecimal(1, -30), 0 },
            { new BigDecimal(-1, -30), 0 }
        };
        return data;
    }
    #endregion
    #region uint
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUInt_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(uint.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.Equal(uint.MaxValue, (uint)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUInt_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(uint.MaxValue);
        var result = (uint)sut;
        Assert.Equal(uint.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUInt_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(uint.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.Equal(uint.MinValue, (uint)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUInt_MinValue_MinValue()
    {
        var sut = new BigDecimal(uint.MinValue);
        var result = (uint)sut;
        Assert.Equal(uint.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("100000000")]
    [Trait("BigDecimal", "Conversion")]
    public void ToUInt_RoundTrip(string value)
    {
        var d = uint.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.Equal(d, (uint)sut);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntChecked_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(uint.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<uint>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntChecked_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(uint.MaxValue);
        Assert.True(BigDecimal.TryConvertToChecked<uint>(sut, out var result));
        Assert.Equal(uint.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntChecked_LessThanMinValue_False()
    {
        var sut = new BigDecimal(uint.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<uint>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntChecked_MinValue_MinValue()
    {
        var sut = new BigDecimal(uint.MinValue);
        Assert.True(BigDecimal.TryConvertToChecked<uint>(sut, out var result));
        Assert.Equal(uint.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntChecked_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToChecked<uint>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntChecked_RoundTrip(string value)
    {
        var d = uint.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToChecked<uint>(sut, out var result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntSaturating_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(uint.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out uint result));
        Assert.Equal(uint.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntSaturating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(uint.MaxValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out uint result));
        Assert.Equal(uint.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntSaturating_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(uint.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out uint result));
        Assert.Equal(uint.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntSaturationg_MinValue_MinValue()
    {
        var sut = new BigDecimal(uint.MinValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out uint result));
        Assert.Equal(uint.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToIntChecked_Saturating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToSaturating<uint>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntSaturating_RoundTrip(string value)
    {
        var d = uint.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out uint result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntTruncating_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(uint.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating(sut, out uint _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntTruncating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(uint.MaxValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out uint result));
        Assert.Equal(uint.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntTruncating_LessThanMinValue_False()
    {
        var sut = new BigDecimal(uint.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating<uint>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntTruncating_MinValue_MinValue()
    {
        var sut = new BigDecimal(uint.MinValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out uint result));
        Assert.Equal(uint.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntTruncating_RoundTrip(string value)
    {
        var d = uint.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out uint result));
        Assert.Equal(d, result);
    }
    [Theory]
    [MemberData(nameof(ProvideTruncatingUIntTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void ToUIntTruncating_Truncated(TestableBigDecimal value, uint expected)
    {
        Assert.True(BigDecimal.TryConvertToTruncating(value.Value, out uint result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ProvideFromUIntTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void FromUInt(uint value, TestableBigDecimal expected)
    {
        Assert.True(BigDecimal.TryConvertFromTruncating(value, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromChecked(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromSaturating(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.Equal(expected.Value, (BigDecimal)value);
    }

    public static TheoryData<uint, TestableBigDecimal> ProvideFromUIntTestCases()
    {
        var data = new TheoryData<uint, TestableBigDecimal>
        {
            { 123000, new BigDecimal(123, 3) },
            { 987, new BigDecimal(987, 0) }
        };
        return data;
    }

    public static TheoryData<TestableBigDecimal, uint> ProvideTruncatingUIntTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, uint>
        {
            { new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30), 123},
            { new BigDecimal(1, -30), 0 }
        };
        return data;
    }
    #endregion
    #region long
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLong_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(long.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.Equal(long.MaxValue, (long)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLong_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(long.MaxValue);
        var result = (long)sut;
        Assert.Equal(long.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLong_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(long.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.Equal(long.MinValue, (long)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLong_MinValue_MinValue()
    {
        var sut = new BigDecimal(long.MinValue);
        var result = (long)sut;
        Assert.Equal(long.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("100000000")]
    [InlineData("-9875")]
    [Trait("BigDecimal", "Conversion")]
    public void ToLong_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.Equal(d, (long)sut);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongChecked_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(long.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<long>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongChecked_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(long.MaxValue);
        Assert.True(BigDecimal.TryConvertToChecked<long>(sut, out var result));
        Assert.Equal(long.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongChecked_LessThanMinValue_False()
    {
        var sut = new BigDecimal(long.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<long>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongChecked_MinValue_MinValue()
    {
        var sut = new BigDecimal(long.MinValue);
        Assert.True(BigDecimal.TryConvertToChecked<long>(sut, out var result));
        Assert.Equal(long.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLonghecked_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToChecked<long>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-6789")]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongChecked_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToChecked<long>(sut, out var result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongSaturating_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(long.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out long result));
        Assert.Equal(long.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongSaturating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(long.MaxValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out long result));
        Assert.Equal(long.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongSaturating_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(long.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out long result));
        Assert.Equal(long.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongSaturationg_MinValue_MinValue()
    {
        var sut = new BigDecimal(long.MinValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out long result));
        Assert.Equal(long.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongSaturating_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToSaturating<long>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-9876")]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongSaturating_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out long result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongTruncating_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(long.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating(sut, out long _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongTruncating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(long.MaxValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out long result));
        Assert.Equal(long.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongTruncating_LessThanMinValue_False()
    {
        var sut = new BigDecimal(long.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating<long>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongTruncating_MinValue_MinValue()
    {
        var sut = new BigDecimal(long.MinValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out long result));
        Assert.Equal(long.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-9876")]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongTruncating_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out long result));
        Assert.Equal(d, result);
    }
    [Theory]
    [MemberData(nameof(ProvideTruncatingLongTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void ToLongTruncating_Truncated(TestableBigDecimal value, long expected)
    {
        Assert.True(BigDecimal.TryConvertToTruncating(value.Value, out long result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ProvideFromLongTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void FromLong(long value, TestableBigDecimal expected)
    {
        Assert.True(BigDecimal.TryConvertFromTruncating(value, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromChecked(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromSaturating(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.Equal(expected.Value, (BigDecimal)value);
    }

    public static TheoryData<long, TestableBigDecimal> ProvideFromLongTestCases()
    {
        var data = new TheoryData<long, TestableBigDecimal>
        {
            { 123000L, new BigDecimal(123, 3) },
            { -987L, new BigDecimal(-987, 0) }
        };
        return data;
    }

    public static TheoryData<TestableBigDecimal, long> ProvideTruncatingLongTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, long>
        {
            { new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30), 123L},
            { new BigDecimal(BigInteger.Parse("-123000111222333444555666777888999"), -30), -123L},
            { new BigDecimal(1, -30), 0L },
            { new BigDecimal(-1, -30), 0L }
        };
        return data;
    }
    #endregion
    #region ulong
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULong_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.Equal(ulong.MaxValue, (ulong)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULong_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        var result = (ulong)sut;
        Assert.Equal(ulong.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULong_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(ulong.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.Equal(ulong.MinValue, (ulong)sut);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULong_MinValue_MinValue()
    {
        var sut = new BigDecimal(ulong.MinValue);
        var result = (ulong)sut;
        Assert.Equal(ulong.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("100000000")]
    [Trait("BigDecimal", "Conversion")]
    public void ToULong_RoundTrip(string value)
    {
        var d = ulong.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.Equal(d, (ulong)sut);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongChecked_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<ulong>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongChecked_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        Assert.True(BigDecimal.TryConvertToChecked<ulong>(sut, out var result));
        Assert.Equal(ulong.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongChecked_LessThanMinValue_False()
    {
        var sut = new BigDecimal(ulong.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToChecked<ulong>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongChecked_MinValue_MinValue()
    {
        var sut = new BigDecimal(ulong.MinValue);
        Assert.True(BigDecimal.TryConvertToChecked<ulong>(sut, out var result));
        Assert.Equal(ulong.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongChecked_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToChecked<ulong>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongChecked_RoundTrip(string value)
    {
        var d = uint.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToChecked<ulong>(sut, out var result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongSaturating_GreaterMaxValue_MaxValue()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out ulong result));
        Assert.Equal(ulong.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongSaturating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out ulong result));
        Assert.Equal(ulong.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongSaturating_LessThanMinValue_MinValue()
    {
        var sut = new BigDecimal(ulong.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out ulong result));
        Assert.Equal(ulong.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongSaturationg_MinValue_MinValue()
    {
        var sut = new BigDecimal(ulong.MinValue);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out ulong result));
        Assert.Equal(ulong.MinValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongSaturating_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToSaturating<ulong>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongSaturating_RoundTrip(string value)
    {
        var d = ulong.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out ulong result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongTruncating_GreaterMaxValue_False()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        sut = new BigDecimal(sut.Mantissa + 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating(sut, out ulong _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongTruncating_MaxValue_MaxValue()
    {
        var sut = new BigDecimal(ulong.MaxValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out ulong result));
        Assert.Equal(ulong.MaxValue, result);
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongTruncating_LessThanMinValue_False()
    {
        var sut = new BigDecimal(ulong.MinValue);
        sut = new BigDecimal(sut.Mantissa - 1, sut.Exponent);
        Assert.False(BigDecimal.TryConvertToTruncating<ulong>(sut, out _));
    }
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongTruncating_MinValue_MinValue()
    {
        var sut = new BigDecimal(ulong.MinValue);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out ulong result));
        Assert.Equal(ulong.MinValue, result);
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("1234")]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongTruncating_RoundTrip(string value)
    {
        var d = ulong.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out ulong result));
        Assert.Equal(d, result);
    }
    [Theory]
    [MemberData(nameof(ProvideTruncatingULongTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void ToULongTruncating_Truncated(TestableBigDecimal value, ulong expected)
    {
        Assert.True(BigDecimal.TryConvertToTruncating(value.Value, out ulong result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ProvideFromULongTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void FromULong(ulong value, TestableBigDecimal expected)
    {
        Assert.True(BigDecimal.TryConvertFromTruncating(value, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromChecked(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromSaturating(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.Equal(expected.Value, (BigDecimal)value);
    }

    public static TheoryData<ulong, TestableBigDecimal> ProvideFromULongTestCases()
    {
        var data = new TheoryData<ulong, TestableBigDecimal>
        {
            { 123000, new BigDecimal(123, 3) },
            { 987, new BigDecimal(987, 0) }
        };
        return data;
    }

    public static TheoryData<TestableBigDecimal, ulong> ProvideTruncatingULongTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, ulong>
        {
            { new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30), 123},
            { new BigDecimal(1, -30), 0 }
        };
        return data;
    }
    #endregion
    #region BigInteger
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("100000000")]
    [InlineData("-9875")]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigInteger_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.Equal(d, (BigInteger)sut);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigIntegerChecked_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToChecked<BigInteger>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-6789")]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigIntegerChecked_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToChecked<BigInteger>(sut, out var result));
        Assert.Equal(d, result);
    }

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigIntegerSaturating_Truncating_False()
    {
        var sut = new BigDecimal(1, -30);
        Assert.False(BigDecimal.TryConvertToSaturating<BigInteger>(sut, out _));
    }
    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-9876")]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigIntegerSaturating_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToSaturating(sut, out BigInteger result));
        Assert.Equal(d, result);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("-1")]
    [InlineData("1234")]
    [InlineData("-9876")]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigIntegerTruncating_RoundTrip(string value)
    {
        var d = long.Parse(value, CultureInfo.InvariantCulture);
        var sut = new BigDecimal(d);
        Assert.True(BigDecimal.TryConvertToTruncating(sut, out BigInteger result));
        Assert.Equal(d, result);
    }
    [Theory]
    [MemberData(nameof(ProvideTruncatingBigIntegerTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void ToBigIntegerTruncating_Truncated(TestableBigDecimal value, BigInteger expected)
    {
        Assert.True(BigDecimal.TryConvertToTruncating(value.Value, out BigInteger result));
        Assert.Equal(expected, result);
    }

    [Theory]
    [MemberData(nameof(ProvideFromBigIntegerTestCases))]
    [Trait("BigDecimal", "Conversion")]
    public void FromBigInteger(BigInteger value, TestableBigDecimal expected)
    {
        Assert.True(BigDecimal.TryConvertFromTruncating(value, out var result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromChecked(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.True(BigDecimal.TryConvertFromSaturating(value, out result));
        Assert.Equal(expected.Value, result);
        Assert.Equal(expected.Value, (BigDecimal)value);
    }

    public static TheoryData<BigInteger, TestableBigDecimal> ProvideFromBigIntegerTestCases()
    {
        var data = new TheoryData<BigInteger, TestableBigDecimal>
        {
            { 123000L, new BigDecimal(123, 3) },
            { -987L, new BigDecimal(-987, 0) }
        };
        return data;
    }

    public static TheoryData<TestableBigDecimal, BigInteger> ProvideTruncatingBigIntegerTestCases()
    {
        var data = new TheoryData<TestableBigDecimal, BigInteger>
        {
            { new BigDecimal(BigInteger.Parse("123000111222333444555666777888999"), -30), 123L},
            { new BigDecimal(BigInteger.Parse("-123000111222333444555666777888999"), -30), -123L},
            { new BigDecimal(1, -30), 0L },
            { new BigDecimal(-1, -30), 0L }
        };
        return data;
    }
    #endregion

    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void FromDoubleChecked_False() => Assert.False(BigDecimal.TryConvertFromChecked(0.1d, out _));
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void FromDoubleSaturating_False() => Assert.False(BigDecimal.TryConvertFromSaturating(0.1d, out _));
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void FromDoubleTruncating_False() => Assert.False(BigDecimal.TryConvertFromTruncating(0.1d, out _));
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDoubleChecked_False() => Assert.False(BigDecimal.TryConvertToChecked(123, out double _));
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDoubleSaturating_False() => Assert.False(BigDecimal.TryConvertToSaturating(123, out double _));
    [Fact]
    [Trait("BigDecimal", "Conversion")]
    public void ToDoubleTruncating_False() => Assert.False(BigDecimal.TryConvertToTruncating(123, out double _));
}
