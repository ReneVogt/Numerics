namespace Revo.Numerics.Interpolation;

public sealed class TrigonometricInterpolator : IInterpolator
{
    public double[] Coefficients => throw new NotImplementedException();

    TrigonometricInterpolator(double[] x, double[] y, double period)
    {
        ArgumentNullException.ThrowIfNull(x, nameof(x));
        ArgumentNullException.ThrowIfNull(y, nameof(y));
        if (x.Length != y.Length) throw new ArgumentException("Arrays must have the same length.");
        if (x.Length == 0) throw new ArgumentException("At least one point is required for interpolation.");
        if (period <= 0) throw new ArgumentException("Period must be positive.");

        // Implementation for trigonometric interpolation
        throw new NotImplementedException();
    }

    public double Evaluate(double x) => throw new NotImplementedException();

    public static IInterpolator Create(double[] x, double[] y, double period) => new TrigonometricInterpolator(x, y, period);

}
