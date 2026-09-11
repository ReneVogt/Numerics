using Revo.Numerics.Solvers;

namespace Numerics.Tests.Solvers.LinearSolverTests;

public partial class LinearSolverTests
{
    [Fact]
    public void Create_EmptyMatrix_IsNotSingularAndSolvesEmptySystem()
    {
        var solver = LinearSolver.Create(0, []);

        Assert.False(solver.IsSingularMatrix);
        Assert.Empty(solver.Solve([]));
    }

    [Fact]
    public void InstanceSolve_NullRightHandSide_ThrowsArgumentNullException()
    {
        var solver = LinearSolver.Create(1, [1]);

        var exception = Assert.Throws<ArgumentNullException>(() => solver.Solve(null!));

        Assert.Equal("rightHandSide", exception.ParamName);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    public void InstanceSolve_MismatchedRightHandSideLength_ThrowsArgumentException(int length)
    {
        var solver = LinearSolver.Create(2, [1, 0, 0, 1]);

        var exception = Assert.Throws<ArgumentException>(() => solver.Solve(new double[length]));

        Assert.Equal("rightHandSide", exception.ParamName);
    }

    [Fact]
    public void InstanceSolve_MultipleRightHandSides_ReusesFactorization()
    {
        var solver = LinearSolver.Create(2, [2, 1, 1, 3]);

        AssertSolution([1, 2], solver.Solve([4, 7]));
        AssertSolution([-1, 3], solver.Solve([1, 8]));
    }

    [Fact]
    public void InstanceSolve_SingularMatrix_DistinguishesRightHandSides()
    {
        var solver = LinearSolver.Create(2, [1, 1, 2, 2]);

        var infinite = Assert.Throws<SingularMatrixException>(() => solver.Solve([2, 4]));
        var none = Assert.Throws<SingularMatrixException>(() => solver.Solve([2, 5]));

        Assert.Equal(LinearEquationSystemState.InfiniteSolutions, infinite.State);
        Assert.Equal(LinearEquationSystemState.NoSolution, none.State);
    }

    [Fact]
    public void InstanceSolve_DoesNotDependOnCoefficientArrayAfterCreation()
    {
        double[] coefficients = [1, 0, 0, 1];
        var solver = LinearSolver.Create(2, coefficients);
        Array.Fill(coefficients, 0);

        AssertSolution([2, 3], solver.Solve([2, 3]));
    }

    [Fact]
    public void InstanceSolve_DoesNotModifyRightHandSide()
    {
        var solver = LinearSolver.Create(2, [1, 0, 5, 2]);
        double[] rightHandSide = [1, 9];

        AssertSolution([1, 2], solver.Solve(rightHandSide));
        Assert.Equal([1d, 9d], rightHandSide);
    }
}
