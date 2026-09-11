namespace Revo.Numerics.Solvers;

/// <summary>
/// In a derived class, Solves square linear equation systems using LU decomposition with complete pivoting.
/// </summary>
/// <remarks>
/// Coefficients are supplied in row-major order. Use <see cref="Create"/> to factorize a
/// coefficient matrix once and reuse the resulting solver for multiple right-hand sides.
/// The coefficients are copied during creation, and zero comparisons use an absolute tolerance.
/// </remarks>
public interface ILinearSolver
{
    /// <summary>
    /// In a derived class, gets a value indicating whether the factorized coefficient matrix is singular according to the configured tolerance.
    /// </summary>
    /// <remarks>
    /// This property depends only on the coefficient matrix and cannot distinguish between no solution and infinitely many
    /// solutions for a particular right-hand side. An empty coefficient matrix is not considered singular.
    /// </remarks>
    bool IsSingularMatrix { get; }

    /// <summary>
    /// In a derived class, solves the factorized linear equation system for the specified right-hand side.
    /// </summary>
    /// <param name="rightHandSide">The right-hand side vector.</param>
    /// <returns>The solution vector in the original variable order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rightHandSide"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="rightHandSide"/> does not equal the number of variables.
    /// </exception>
    /// <exception cref="SingularMatrixException">
    /// The system has no solution or infinitely many solutions. Inspect <see cref="SingularMatrixException.State"/>
    /// to distinguish the two cases.
    /// </exception>
    /// <remarks>The factorization is reused, and <paramref name="rightHandSide"/> is not modified.</remarks>
    double[] Solve(double[] rightHandSide);

    /// <summary>
    /// In a derived class, factorizes a square coefficient matrix and creates a reusable solver.
    /// </summary>
    /// <param name="numberOfVariables">The number of variables and rows in the coefficient matrix.</param>
    /// <param name="coefficients">The coefficient matrix in row-major order.</param>
    /// <param name="epsilon">The absolute tolerance used to consider a value zero.</param>
    /// <returns>A solver containing the LU factorization of <paramref name="coefficients"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="coefficients"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="coefficients"/> is not the square of <paramref name="numberOfVariables"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="numberOfVariables"/> is negative, or <paramref name="epsilon"/> is negative or not finite.
    /// </exception>
    /// <remarks>The supplied coefficients are copied and can be changed after this method returns.</remarks>
    static abstract ILinearSolver Create(int numberOfVariables, double[] coefficients, double epsilon = 1E-08);

    /// <summary>
    /// In a derived class, factorizes  a square coefficient matrix and solves the resulting equation system.
    /// </summary>
    /// <param name="coefficients">The coefficient matrix in row-major order.</param>
    /// <param name="rightHandSide">The right-hand side vector, whose length determines the number of variables.</param>
    /// <param name="epsilon">The absolute tolerance used to consider a value zero.</param>
    /// <returns>The solution vector in the original variable order.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="coefficients"/> or <paramref name="rightHandSide"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The length of <paramref name="coefficients"/> is not the square of the length of
    /// <paramref name="rightHandSide"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="epsilon"/> is negative or not finite.</exception>
    /// <exception cref="SingularMatrixException">
    /// The system has no solution or infinitely many solutions. Inspect <see cref="SingularMatrixException.State"/>
    /// to distinguish the two cases.
    /// </exception>
    /// <remarks>Use <see cref="Create"/> when solving the same coefficient matrix for multiple right-hand sides.</remarks>
    static abstract double[] Solve(double[] coefficients, double[] rightHandSide, double epsilon = 1E-08);
}