using System.Globalization;
using System.Text.RegularExpressions;

namespace Numerics.Tests.Interpolation.NewtonPolynomInterpolatorTests;

public sealed partial class NewtonPolynomInterpolatorTests
{
    [
        Theory,
        InlineData(
            "0 1",
            "0 1"),
        InlineData(
            "1 2 3",
            "0 1 4")
    ]
    public void Interpolate_CorrectResults(string x, string y)
    {
        var xs = MatrixSplitRegex().Split(x).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var ys = MatrixSplitRegex().Split(y).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();

        var solution = Revo.Numerics.Interpolation.NewtonPolynomInterpolator.Interpolate(xs, ys);
        Validate(xs, ys, solution);
    }

    static void Validate(double[] x, double[] y, double[] solution)
    {
        Assert.True(x.Zip(y).All(e => solution.Select((a, i) => (a, i)).Sum(s => s.a * Math.Pow(e.First, s.i)) == e.Second));
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MatrixSplitRegex();
}
