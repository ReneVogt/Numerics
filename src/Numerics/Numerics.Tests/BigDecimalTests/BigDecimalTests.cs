using Revo.Numerics;
using System.Numerics;
using Xunit.Abstractions;

namespace Numerics.Tests.BigDecimalTests;

public sealed partial class BigDecimalTests
{
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Default_Zero()
    {
        BigDecimal sut = default;
        Assert.Equal(0, sut);        
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_MaxValue()
    {
        BigDecimal sut = new(decimal.MaxValue);
        Assert.Equal(new BigInteger(decimal.MaxValue), sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_MinValue()
    {
        BigDecimal sut = new(decimal.MinValue);
        Assert.Equal(new BigInteger(decimal.MinValue), sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_PositiveLargeWithDecimals()
    {
        var source = new Decimal(17000000.0000017d);
        BigDecimal sut = new(source);
        Assert.Equal(BigInteger.Parse("170000000000017"), sut.Mantissa);
        Assert.Equal(-7, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_NegativeLargeWithDecimals()
    {
        var source = new Decimal(-17000000.0000017d);
        BigDecimal sut = new(source);
        Assert.Equal(BigInteger.Parse("-170000000000017"), sut.Mantissa);
        Assert.Equal(-7, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_PositiveLargeNoDecimals()
    {
        var source = new Decimal(17000000d);
        BigDecimal sut = new(source);
        Assert.Equal(BigInteger.Parse("17"), sut.Mantissa);
        Assert.Equal(6, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_NegativeLargeNoDecimals()
    {
        var source = new Decimal(-17000000d);
        BigDecimal sut = new(source);
        Assert.Equal(BigInteger.Parse("-17"), sut.Mantissa);
        Assert.Equal(6, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_PositiveVerySmall()
    {
        var source = new Decimal(0.000000017d);
        BigDecimal sut = new(source);
        Assert.Equal(new BigInteger(17), sut.Mantissa);
        Assert.Equal(-9, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromDecimal_NegativeVerySmall()
    {
        var source = new Decimal(-0.000000017d);
        BigDecimal sut = new(source);
        Assert.Equal(new BigInteger(-17), sut.Mantissa);
        Assert.Equal(-9, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromInt_Positive()
    {
        BigDecimal sut = new(123000);
        Assert.Equal(123, sut.Mantissa);
        Assert.Equal(3, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromInt_Negative()
    {
        BigDecimal sut = new(-123000000);
        Assert.Equal(-123, sut.Mantissa);
        Assert.Equal(6, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromInt_Zero()
    {
        BigDecimal sut = new(0);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromUInt_Positive()
    {
        BigDecimal sut = new((uint)123000);
        Assert.Equal(123, sut.Mantissa);
        Assert.Equal(3, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromUInt_Zero()
    {
        BigDecimal sut = new((uint)0);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromLong_Positive()
    {
        BigDecimal sut = new(1234000000L);
        Assert.Equal(1234, sut.Mantissa);
        Assert.Equal(6, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromLong_Negative()
    {
        BigDecimal sut = new(-1234000000L);
        Assert.Equal(-1234, sut.Mantissa);
        Assert.Equal(6, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromLong_Zero()
    {
        BigDecimal sut = new(0L);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromULong_Positive()
    {
        BigDecimal sut = new((ulong)1700000000000L);
        Assert.Equal(17, sut.Mantissa);
        Assert.Equal(11, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_ULong_Zero()
    {
        BigDecimal sut = new((ulong)0);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromBigInteger_Positive()
    {
        BigDecimal sut = new(BigInteger.Parse("17000000000"));
        Assert.Equal(new BigInteger(17), sut.Mantissa);
        Assert.Equal(9, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromBigInteger_Negative()
    {
        BigDecimal sut = new(BigInteger.Parse("-17000000000"));
        Assert.Equal(new BigInteger(-17), sut.Mantissa);
        Assert.Equal(9, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromBigInteger_Zero()
    {
        BigDecimal sut = new(BigInteger.Zero);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromScratch_Zero()
    {
        BigDecimal sut = new(BigInteger.Zero, 123);
        Assert.Equal(0, sut.Mantissa);
        Assert.Equal(0, sut.Exponent);
    }
    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void Construct_FromScratch_Normalized()
    {
        BigDecimal sut = new(17000, 4);
        Assert.Equal(17, sut.Mantissa);
        Assert.Equal(7, sut.Exponent);
    }

    [Fact]
    [Trait("BigDecimal", "Construction")]
    public void GetHashCode_DifferentForSomeCases()
    {
        Assert.NotEqual(BigDecimal.One.GetHashCode(), BigDecimal.Zero.GetHashCode());
        Assert.NotEqual(BigDecimal.One.GetHashCode(), BigDecimal.NegativeOne.GetHashCode());
        Assert.NotEqual(BigDecimal.Zero.GetHashCode(), BigDecimal.NegativeOne.GetHashCode());
    }
    [Theory]
    [MemberData(nameof(ProvideDeconstructorTestCases))]
    [Trait("BigDecimal", "Construction")]
    public void Deconstruct(TestableBigDecimal value)
    {
        var (mantissa, exponent) = value.Value;
        Assert.Equal(value.Value.Sign, mantissa.Sign);
        Assert.Equal(value.Value.Mantissa, mantissa);
        Assert.Equal(value.Value.Exponent, exponent);
    }

    public static TheoryData<TestableBigDecimal> ProvideDeconstructorTestCases() => new()
    {
        new BigDecimal(-12, -30),
        new BigDecimal(-12, 31),
        new BigDecimal(17, -1),
        new BigDecimal(17, 123),
        BigDecimal.One,
        BigDecimal.Zero,
        BigDecimal.NegativeOne
    };

    public sealed class TestableBigDecimal(BigDecimal value) : IXunitSerializable
    {
        public BigDecimal Value { get; private set; } = value;

        public TestableBigDecimal() : this(BigDecimal.Zero) { }

        public void Deserialize(IXunitSerializationInfo info)
        {
            var mantissa = info.GetValue<BigInteger>("Mantissa");
            var exponent = info.GetValue<int>("Exponent");
            Value = new(mantissa, exponent);
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Mantissa", Value.Mantissa);
            info.AddValue("Exponent", Value.Exponent);
        }

        public static implicit operator TestableBigDecimal(BigDecimal value) => new(value);
    }
}