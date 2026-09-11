using Revo.Numerics.Solvers;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Numerics.Tests.Solvers.LinearSolverTests;

public partial class LinearSolverTests
{
    [Fact]
    public void Empty()
    {
        var result = LinearSolver.Solve([], []);
        Assert.Empty(result);
    }
    [Fact]
    public void Empty_ThrowsOnNonEmptyB()
    {
        Assert.Throws<ArgumentException>(() => LinearSolver.Solve([], [1]));
    }
    [Fact]
    public void ThrowsOnNullCoefficients()
    {
        Assert.Throws<ArgumentNullException>(() => LinearSolver.Solve(null!, []));
    }
    [Fact]
    public void ThrowsOnNullRightHandSide()
    {
        Assert.Throws<ArgumentNullException>(() => LinearSolver.Solve([], null!));
    }
    [Fact]
    public void ThrowsOnMismatchedDimensions()
    {
        Assert.Throws<ArgumentException>(() => LinearSolver.Solve([1, 2], [1, 2, 3]));
    }

    [
        Theory,
        InlineData(1, """
        0 0
        """),
        InlineData(2, """
        1 1 2
        2 2 4
        """),
        InlineData(4, """
        1 2 3 4 7
        2 4 6 8 14
        4 8 12 16 28
        6 12 18 24 42
        """)
    ]
    public void InfiniteSolutions(int n, string completeMatrix)
    {
        var matrix = MatrixSplitRegex().Split(completeMatrix).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var coeffs = Enumerable.Range(0, n).SelectMany(row => Enumerable.Range(0, n).Select(col => matrix[row * (n + 1) + col])).ToArray();
        var rhs = Enumerable.Range(0, n).Select(row => matrix[row * (n + 1) + n]).ToArray();
        var solver = LinearSolver.Create(n, coeffs);
        Assert.True(solver.IsSingularMatrix);
        Assert.Equal(LinearEquationSystemState.InfiniteSolutions, Assert.Throws<SingularMatrixException>(() => solver.Solve(rhs)).State);
    }

    [
        Theory,
        InlineData(1, """
        0 1
        """),
        InlineData(2, """
        1 1 3
        2 2 4
        """),
        InlineData(4, """
        1 2 3 4 7
        2 4 6 8 15
        4 8 12 16 28
        6 12 18 24 42
        """)
    ]
    public void NoSolutions(int n, string completeMatrix)
    {
        var matrix = MatrixSplitRegex().Split(completeMatrix).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
        var coeffs = Enumerable.Range(0, n).SelectMany(row => Enumerable.Range(0, n).Select(col => matrix[row * (n + 1) + col])).ToArray();
        var rhs = Enumerable.Range(0, n).Select(row => matrix[row * (n + 1) + n]).ToArray();

        var solver = LinearSolver.Create(n, coeffs);
        Assert.True(solver.IsSingularMatrix);
        Assert.Equal(LinearEquationSystemState.NoSolution, Assert.Throws<SingularMatrixException>(() => solver.Solve(rhs)).State);
    }

    [
        Theory,
        InlineData(1, """
        1 2
        """),
        InlineData(1, """
        -1 1
        """),
        InlineData(1, """
        1 -1
        """),
        InlineData(2, """
        2 1 0 
        1 2 1
        """),
        InlineData(5, """
        0  3  7  9 17  7
        1 13  5 13 23  8
        7 -3  1  5 90 15
        6  2 -7  1 11 11
        3  1  2  4  5  6
        """)
    ]
    public void Solvable(int n, string completeMatrix)
    {
        var matrix = MatrixSplitRegex().Split(completeMatrix).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();        
        var coeffs = Enumerable.Range(0, n).SelectMany(row => Enumerable.Range(0, n).Select(col => matrix[row * (n + 1) + col])).ToArray();
        var rhs = Enumerable.Range(0, n).Select(row => matrix[row * (n + 1) + n]).ToArray();
        
        var result = LinearSolver.Solve(coeffs, rhs);
        Assert.Equal(n, result.Length);
        Validate(coeffs, rhs, result);
    }

    static void Validate(double[] coefficients, double[] rightHandSide, double[] result)
    {
        var n = rightHandSide.Length;
        for (var i = 0; i < n; i++)
        {
            var sum = 0.0;
            for (var j = 0; j < n; j++)
                sum += coefficients[i * n + j] * result[j];
            Assert.Equal(rightHandSide[i], sum, 5);
        }
    }

    static void AssertSolution(double[] expected, double[] actual)
    {
        Assert.Equal(expected.Length, actual.Length);
        for (var i = 0; i < expected.Length; i++)
            Assert.Equal(expected[i], actual[i], 12);
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex MatrixSplitRegex();
}
