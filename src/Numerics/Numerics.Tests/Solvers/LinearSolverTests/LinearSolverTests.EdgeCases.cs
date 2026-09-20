using Revo.Numerics.Solvers;

namespace Numerics.Tests.Solvers.LinearSolverTests;

public partial class LinearSolverTests
{
    [Theory]
    [InlineData(0.1, 1)]
    [InlineData(0.1, -1)]
    [InlineData(LinearSolver.DefaultEpsilon, 1)]
    [InlineData(LinearSolver.DefaultEpsilon, -1)]
    public void InstanceSolve_SubToleranceBlockWithoutPivot_PreservesRightHandSide(double epsilon, double sign)
    {
        var solver = LinearSolver.Create(2, [0, 0, epsilon / 2, 0], epsilon);
        Assert.True(solver.IsSingularMatrix);

        foreach (var factor in new[] { 0.99, 1, 1 + epsilon / 4 })
        {
            var rightHandSide = new[] { sign * epsilon, sign * epsilon * factor };
            var original = (double[])rightHandSide.Clone();

            var exception = Assert.Throws<SingularMatrixException>(() => solver.Solve(rightHandSide));

            Assert.Equal(factor <= 1 ? LinearEquationSystemState.InfiniteSolutions : LinearEquationSystemState.NoSolution, exception.State);
            Assert.Equal(original, rightHandSide);
        }
    }

    [Theory]
    [InlineData(0.124, LinearEquationSystemState.InfiniteSolutions)]
    [InlineData(0.125, LinearEquationSystemState.InfiniteSolutions)]
    [InlineData(0.129, LinearEquationSystemState.NoSolution)]
    [InlineData(-0.124, LinearEquationSystemState.InfiniteSolutions)]
    [InlineData(-0.125, LinearEquationSystemState.InfiniteSolutions)]
    [InlineData(-0.129, LinearEquationSystemState.NoSolution)]
    public void InstanceSolve_SubToleranceBlockAfterPivot_OnlyAppliesCompletedElimination(
        double trailingRightHandSide,
        LinearEquationSystemState expected)
    {
        // The first pivot swaps both rows and columns. The residual 0.0625 is not an L multiplier.
        var solver = LinearSolver.Create(3, [0, 1, 0, 0, 4, 0, 0.0625, 2, 0], epsilon: 0.125);
        Assert.True(solver.IsSingularMatrix);

        foreach (var pivotRightHandSide in new[] { 0d, 1d, -1d })
        {
            var rightHandSide = new[]
            {
                0.25 * pivotRightHandSide + Math.CopySign(0.125, trailingRightHandSide),
                pivotRightHandSide,
                0.5 * pivotRightHandSide + trailingRightHandSide
            };
            var original = (double[])rightHandSide.Clone();

            var exception = Assert.Throws<SingularMatrixException>(() => solver.Solve(rightHandSide));

            Assert.Equal(expected, exception.State);
            Assert.Equal(original, rightHandSide);
        }
    }

    [Fact]
    public void InstanceSolve_ZeroMatrixAndZeroRightHandSide_HasInfiniteSolutions()
    {
        var solver = LinearSolver.Create(2, [0, 0, 0, 0]);

        var exception = Assert.Throws<SingularMatrixException>(() => solver.Solve([0, 0]));

        Assert.Equal(LinearEquationSystemState.InfiniteSolutions, exception.State);
    }

    [Fact]
    public void InstanceSolve_ZeroMatrixAndNonzeroRightHandSide_HasNoSolution()
    {
        var solver = LinearSolver.Create(2, [0, 0, 0, 0]);

        var exception = Assert.Throws<SingularMatrixException>(() => solver.Solve([0, 1]));

        Assert.Equal(LinearEquationSystemState.NoSolution, exception.State);
    }

    [Theory]
    [InlineData(0, LinearEquationSystemState.InfiniteSolutions)]
    [InlineData(1, LinearEquationSystemState.NoSolution)]
    public void InstanceSolve_RankDeficientMatrix_DetectsSolutionState(
        double finalRightHandSideValue,
        LinearEquationSystemState expected)
    {
        var solver = LinearSolver.Create(3, [1, 0, 0, 0, 1, 0, 0, 0, 0]);

        var exception = Assert.Throws<SingularMatrixException>(() => solver.Solve([2, 3, finalRightHandSideValue]));

        Assert.Equal(expected, exception.State);
    }

    [Fact]
    public void InstanceSolve_PivotAtEpsilon_IsTreatedAsZero()
    {
        var solver = LinearSolver.Create(2, [1, 0, 0, LinearSolver.DefaultEpsilon]);

        var infinite = Assert.Throws<SingularMatrixException>(() => solver.Solve([1, 0]));
        var none = Assert.Throws<SingularMatrixException>(() => solver.Solve([1, 10 * LinearSolver.DefaultEpsilon]));

        Assert.Equal(LinearEquationSystemState.InfiniteSolutions, infinite.State);
        Assert.Equal(LinearEquationSystemState.NoSolution, none.State);
    }

    [Fact]
    public void InstanceSolve_CustomEpsilonBelowPivot_ReturnsSingleSolution()
    {
        var solver = LinearSolver.Create(2, [1, 0, 0, 1e-9], 1e-10);

        AssertSolution([1, 1], solver.Solve([1, 1e-9]));
    }

    [Fact]
    public void InstanceSolve_ZeroEpsilon_ReturnsSingleSolution()
    {
        var solver = LinearSolver.Create(2, [1, 0, 0, 1], 0);

        AssertSolution([3, 4], solver.Solve([3, 4]));
    }

    [Fact]
    public void Create_NegativeVariableCount_ThrowsArgumentOutOfRangeException()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => LinearSolver.Create(-1, [0]));

        Assert.Equal("numberOfVariables", exception.ParamName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.NegativeInfinity)]
    [InlineData(double.PositiveInfinity)]
    public void Create_InvalidEpsilon_ThrowsArgumentOutOfRangeException(double epsilon)
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => LinearSolver.Create(1, [1], epsilon));

        Assert.Equal("epsilon", exception.ParamName);
    }

    [Fact]
    public void Create_NullCoefficients_ThrowsArgumentNullException()
    {
        var exception = Assert.Throws<ArgumentNullException>(() => LinearSolver.Create(1, null!));

        Assert.Equal("coefficients", exception.ParamName);
    }

    [Fact]
    public void Create_MismatchedCoefficientCount_ThrowsArgumentException()
    {
        var exception = Assert.Throws<ArgumentException>(() => LinearSolver.Create(2, [1, 2, 3]));

        Assert.Equal("coefficients", exception.ParamName);
    }
}
