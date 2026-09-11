namespace Revo.Numerics.Solvers;

/// <summary>
/// Describes the number of solutions of a linear equation system.
/// </summary>
public enum LinearEquationSystemState
{
    /// <summary>The system has exactly one solution.</summary>
    SingleSolution,

    /// <summary>The system has infinitely many solutions.</summary>
    InfiniteSolutions,

    /// <summary>The system has no solution.</summary>
    NoSolution
}
