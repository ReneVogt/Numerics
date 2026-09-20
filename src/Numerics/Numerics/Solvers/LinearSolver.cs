using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Revo.Numerics.Solvers;

/// <summary>
/// Solves square linear equation systems using LU decomposition with complete pivoting.
/// </summary>
/// <remarks>
/// Coefficients are supplied in row-major order. Use <see cref="Create"/> to factorize a
/// coefficient matrix once and reuse the resulting solver for multiple right-hand sides.
/// The coefficients are copied during creation, and zero comparisons use an absolute tolerance.
/// </remarks>
public sealed class LinearSolver : ILinearSolver
{
    readonly struct Matrix(int variableCount, double[] coefficients)
    {
        readonly double[] _data = (double[])coefficients.Clone();
        public int[] RowPivot { get; } = [.. Enumerable.Range(0, variableCount)];
        public int[] ColumnPivot { get; } = [.. Enumerable.Range(0, variableCount)];
        public double this[int row, int col]
        {
            get => _data[RowPivot[row] * variableCount + ColumnPivot[col]];
            set => _data[RowPivot[row] * variableCount + ColumnPivot[col]] = value;
        }
    }

    /// <summary>
    /// The default absolute tolerance used to consider a value zero.
    /// </summary>
    /// <remarks>A value is considered zero when its absolute value is less than or equal to this tolerance.</remarks>
    public const double DefaultEpsilon = 1e-8;

    readonly int _variableCount;
    readonly int _processedRows;
    readonly double _epsilon;
    readonly Matrix _lu;

    /// <summary>
    /// Gets a value indicating whether the factorized coefficient matrix is singular according to the configured tolerance.
    /// </summary>
    /// <remarks>
    /// This property depends only on the coefficient matrix and cannot distinguish between no solution and infinitely many
    /// solutions for a particular right-hand side. An empty coefficient matrix is not considered singular.
    /// </remarks>
    public bool IsSingularMatrix => _variableCount != 0 && IsZero(_lu[_variableCount - 1, _variableCount - 1]);

    LinearSolver(int numberOfVariables, double[] coefficients, double epsilon = DefaultEpsilon)
    {
        if (numberOfVariables < 0)
            throw new ArgumentOutOfRangeException(nameof(numberOfVariables), "The variable count must not be negative.");
        ArgumentNullException.ThrowIfNull(coefficients, nameof(coefficients));
        if (coefficients.Length != (long)numberOfVariables * numberOfVariables)
            throw new ArgumentException("The length of coefficients must be equal to the square of the variable count.", nameof(coefficients));
        if (epsilon < 0 || !double.IsFinite(epsilon))
            throw new ArgumentOutOfRangeException(nameof(epsilon), "Epsilon must be finite and not negative.");

        _variableCount = numberOfVariables;
        _lu = new(_variableCount, coefficients);
        _epsilon = epsilon;
        if (_variableCount == 0) return;

        // initialize pivot
        (var maxrow, var maxcol) = Enumerable.Range(0, _variableCount).SelectMany(row => Enumerable.Range(0, _variableCount).Select(col => (row, col))).MaxBy(x => Math.Abs(_lu[x.row, x.col]));

        Log("Initial matrix");

        // Calculate LU matrixw with pivoting
        for (var processingRow = 0; processingRow < _variableCount - 1 && !IsZero(_lu[maxrow, maxcol]); processingRow++)
        {
            _processedRows++;

            (_lu.RowPivot[maxrow], _lu.RowPivot[processingRow]) = (_lu.RowPivot[processingRow], _lu.RowPivot[maxrow]);
            (_lu.ColumnPivot[maxcol], _lu.ColumnPivot[processingRow]) = (_lu.ColumnPivot[processingRow], _lu.ColumnPivot[maxcol]);

            Log($"[{processingRow}] Pivoted ({maxrow}, {maxcol})");

            maxrow = maxcol = processingRow+1;
            var maxU = 0d;

            // eliminate
            for (var eliminatingRow = processingRow+1; eliminatingRow < _variableCount; eliminatingRow++)
            {
                var quotient = _lu[eliminatingRow, processingRow] / _lu[processingRow, processingRow];
                _lu[eliminatingRow, processingRow] = quotient;

                for (var eliminatingColumn = processingRow+1; eliminatingColumn < _variableCount; eliminatingColumn++)
                {
                    var u = _lu[eliminatingRow, eliminatingColumn] - quotient * _lu[processingRow, eliminatingColumn];
                    var uAbs = Math.Abs(u);
                    _lu[eliminatingRow, eliminatingColumn] = u;
                    if (uAbs > maxU)
                        (maxU, maxrow, maxcol) = (uAbs, eliminatingRow, eliminatingColumn);
                }
            }

            Log($"[{processingRow}] Eliminated");
        }

        ValidateLU(coefficients);
    }

    /// <summary>
    /// Solves the factorized linear equation system for the specified right-hand side.
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
    public double[] Solve(double[] rightHandSide)
    {
        ArgumentNullException.ThrowIfNull(rightHandSide, nameof(rightHandSide));
        if (rightHandSide.Length != _variableCount)
            throw new ArgumentException("The length of right hand side vector must be equal to the variable count.", nameof(rightHandSide));

        if (_variableCount == 0) return [];

        // Ly = b -> calculate y
        var y = new double[_variableCount];
        for (var row = 0; row < _variableCount; row++)
        {
            var sum = rightHandSide[_lu.RowPivot[row]];
            // Only processed columns contain L multipliers; the trailing L block is the identity.
            for (var col = 0; col < Math.Min(row, _processedRows); col++)
                sum -= _lu[row, col] * y[col];

            y[row] = sum;
        }

        CheckSingularity(y);

        // Ux = y -> calculate x
        var x = new double[_variableCount];
        for (var row = _variableCount - 1; row >= 0; row--)
        {
            var sum = y[row];
            for (var col = row + 1; col < _variableCount; col++)
                sum -= _lu[row, col] * x[_lu.ColumnPivot[col]];

            x[_lu.ColumnPivot[row]] = sum / _lu[row, row];
        }
        return x;
    }
    
    bool IsZero(double value) => Math.Abs(value) <= _epsilon;

    void CheckSingularity(double[] y)
    {
        if (!IsSingularMatrix) return;
        if (Enumerable.Range(_processedRows, _variableCount - _processedRows).Any(row => !IsZero(y[row])))
            throw new SingularMatrixException(LinearEquationSystemState.NoSolution);
        else
            throw new SingularMatrixException(LinearEquationSystemState.InfiniteSolutions);
    }

    [Conditional("DEBUG")]
    [ExcludeFromCodeCoverage]
    void Log(string message)
    {
        if (!Debugger.IsAttached) return;
        Debug.WriteLine(message);

        if (_variableCount == 0) return;

        var formatted = Enumerable.Range(0, _variableCount).SelectMany(row => Enumerable.Range(0, _variableCount).Select(col => _lu[row, col].ToString("N4"))).ToArray();
        var maxLength = formatted.Max(s => s.Length);
        Debug.WriteLine(string.Join(Environment.NewLine, Enumerable.Range(0, _variableCount).Select(row => string.Join(string.Empty, Enumerable.Range(0, _variableCount).Select(col => formatted[row * _variableCount + col].PadLeft(maxLength+1))))));
        Debug.WriteLine(string.Empty);
    }
    [Conditional("DEBUG")]
    void ValidateLU(double[] a)
    {
        // Preserve the unprocessed residual block in U, with an identity block in L.
        var l = Enumerable.Range(0, _variableCount).SelectMany(row => Enumerable.Range(0, _variableCount).Select(col => row > col && col < _processedRows ? _lu[row, col] : (row == col ? 1.0 : 0.0))).ToArray();
        var u = Enumerable.Range(0, _variableCount).SelectMany(row => Enumerable.Range(0, _variableCount).Select(col => row <= col || col >= _processedRows ? _lu[row, col] : 0.0)).ToArray();
        var lu = Enumerable.Range(0, _variableCount).SelectMany(row => Enumerable.Range(0, _variableCount).Select(col => Enumerable.Range(0, _variableCount).Sum(k => l[row * _variableCount + k] * u[k * _variableCount + col]))).ToArray();
        var pivotedA = Enumerable.Range(0, _variableCount).SelectMany(row => Enumerable.Range(0, _variableCount).Select(col => a[_lu.RowPivot[row] * _variableCount + _lu.ColumnPivot[col]])).ToArray();
        Debug.Assert(lu.Zip(pivotedA).All(x => Math.Abs(x.First - x.Second) <= _epsilon));
    }

    /// <summary>
    /// Factorizes a square coefficient matrix and creates a reusable solver.
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
    public static ILinearSolver Create(int numberOfVariables, double[] coefficients, double epsilon = DefaultEpsilon) => new LinearSolver(numberOfVariables, coefficients, epsilon);

    /// <summary>
    /// Factorizes a square coefficient matrix and solves the resulting equation system.
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
    public static double[] Solve(double[] coefficients, double[] rightHandSide, double epsilon = DefaultEpsilon) => Create(rightHandSide?.Length ?? throw new ArgumentNullException(nameof(rightHandSide)), coefficients, epsilon).Solve(rightHandSide);
}
