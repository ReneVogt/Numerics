using Revo.Numerics.Solvers;

namespace Numerics.Tests.Solvers.LinearSolverTests;

public partial class LinearSolverTests
{
    public static TheoryData<double[], double[], double[]> PivotingCases => new()
    {
        // No permutation.
        { [5, 1, 2, 3], [7, 8], [1, 2] },
        // Row permutation only.
        { [1, 0, 5, 2], [1, 9], [1, 2] },
        // Column permutation only.
        { [1, 5, 0, 2], [11, 4], [1, 2] },
        // Both row and column permutations.
        { [1, 2, 3, 5], [5, 13], [1, 2] },
        // A second permutation in the remaining 2x2 block.
        { [10, 0, 0, 0, 1, 2, 0, 3, 4], [10, 8, 18], [1, 2, 3] }
    };

    [Theory]
    [MemberData(nameof(PivotingCases))]
    public void InstanceSolve_Pivoting_ReturnsVariablesInOriginalOrder(
        double[] coefficients,
        double[] rightHandSide,
        double[] expected)
    {
        var solver = LinearSolver.Create(rightHandSide.Length, coefficients);

        var actual = solver.Solve(rightHandSide);

        Assert.False(solver.IsSingularMatrix);
        AssertSolution(expected, actual);
    }
}
