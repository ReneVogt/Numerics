namespace Revo.Numerics.Solvers;

/// <summary>
/// The exception thrown when a singular coefficient matrix prevents a linear equation system from having a unique solution.
/// </summary>
public sealed class SingularMatrixException : ArithmeticException
{
    /// <summary>
    /// Gets the state indicating whether the system has no solution or infinitely many solutions.
    /// </summary>
    public LinearEquationSystemState State { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="SingularMatrixException"/> class with the specified state.
    /// </summary>
    /// <param name="state">The state indicating whether the system has no solution or infinitely many solutions.</param>
    public SingularMatrixException(LinearEquationSystemState state) : base(state == LinearEquationSystemState.InfiniteSolutions ? "The linear equation system has infinite solutions." : "The linear equation system has no solution.")
    {
        State = state;
    }
}
