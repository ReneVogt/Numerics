using Revo.Numerics;

namespace Numerics.Tests.RomanNumberTests;

public sealed partial class RomanNumberTests
{
    [Theory]
    [
        InlineData(""),
        InlineData("IL"),
        InlineData("IC"),
        InlineData("ID"),
        InlineData("IM"),
        InlineData("XD"),
        InlineData("XM"),
        InlineData("VX"),
        InlineData("VL"),
        InlineData("VC"),
        InlineData("LC"),
        InlineData("DM"),
        InlineData("IIV"),
        InlineData("IIX"),
        InlineData("XXL"),
        InlineData("XXC"),
        InlineData("CCD"),
        InlineData("CCM"),
        InlineData("IVI"),
        InlineData("IXI"),
        InlineData("XLX"),
        InlineData("XCX"),
        InlineData("CDC"),
        InlineData("CMC"),
        InlineData("VV"),
        InlineData("LL"),
        InlineData("DD"),
        InlineData("IIIII"),
        InlineData("XXXXX"),
        InlineData("CCCCC")
    ]
    public void Parse_ThrowsOnInvalidInput(string input) => Assert.Throws<FormatException>(() => RomanNumber.Parse(input));

    [Theory]
    [
        InlineData("I", 1),
        InlineData("IIII", 4),
        InlineData("VIIII", 9),
        InlineData("XXXX", 40),
        InlineData("LXXXX", 90),
        InlineData("CCCC", 400),
        InlineData("DCCCC", 900),
        InlineData("MDCCCCLXXXXVIIII", 1999),
        InlineData("MCMXCIX", 1999)
    ]
    public void Parse_Correct_Specific(string input, ushort expected) => Assert.Equal(expected, RomanNumber.Parse(input));
    [Theory]
    [MemberData(nameof(ParseTestCases))]
    public void Parse_Correct(string input, ushort expected) => Assert.Equal(expected, RomanNumber.Parse(input));

    public static TheoryData<string, ushort> ParseTestCases()
    {
        var ones = new Dictionary<string, int>
        {
            ["I"] = 1,
            ["II"] = 2,
            ["III"] = 3,
            ["IIII"] = 4,
            ["IV"] = 4,
            ["V"] = 5,
            ["VI"] = 6,
            ["VII"] = 7,
            ["VIII"] = 8,
            ["VIIII"] = 9,
            ["IX"] = 9,
        };

        var tens = new Dictionary<string, int>
        {
            ["X"] = 10,
            ["XX"] = 20,
            ["XXX"] = 30,
            ["XXXX"] = 40,
            ["XL"] = 40,
            ["L"] = 50,
            ["LX"] = 60,
            ["LXX"] = 70,
            ["LXXX"] = 80,
            ["LXXXX"] = 90,
            ["XC"] = 90,
        };

        var hundreds = new Dictionary<string, int>
        {
            ["C"] = 100,
            ["CC"] = 200,
            ["CCC"] = 300,
            ["CCCC"] = 400,
            ["CD"] = 400,
            ["D"] = 500,
            ["DC"] = 600,
            ["DCC"] = 700,
            ["DCCC"] = 800,
            ["DCCCC"] = 900,
            ["CM"] = 900,
        };

        var thousands = Enumerable.Range(0, 2)
            .ToDictionary(n => new string('M', n), n => n * 1000);

        var data = new TheoryData<string, ushort>();

        foreach (var th in thousands)
            foreach (var h in hundreds)
                foreach (var t in tens)
                    foreach (var o in ones)
                    {
                        var roman = th.Key + h.Key + t.Key + o.Key;
                        var value = th.Value + h.Value + t.Value + o.Value;

                        if (value > 0)
                            data.Add(roman, (ushort)value);
                    }

        return data;
    }
}
