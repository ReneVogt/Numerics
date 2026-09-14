namespace Revo.Numerics.Interpolation;

/// <summary>
/// Provides factory methods for interpolating functions through supplied points.
/// </summary>
public static class Interpolators
{
    /// <summary>
    /// Creates a polynomial interpolator using Newton divided differences.
    /// </summary>
    /// <param name="x">The distinct X coordinates of at least one point, in any order.</param>
    /// <param name="y">The corresponding Y coordinates, with the same length as <paramref name="x"/>.</param>
    /// <returns>
    /// An interpolator whose coefficients are in the monomial basis in ascending order of power:
    /// element i multiplies x raised to the power i. There is one coefficient per point,
    /// including trailing coefficients for lower-degree polynomials. A single point produces
    /// a constant polynomial with value <c>y[0]</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="x"/> or <paramref name="y"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The arrays are empty, have different lengths, or contain repeated X coordinates.</exception>
    /// <remarks>
    /// The input arrays are neither modified nor retained. Each access to
    /// <see cref="IInterpolator.Coefficients"/> returns a new array; modifying it does not affect
    /// subsequent coefficient access or evaluation. Construction takes O(n²) time and O(n) additional storage.
    /// Repeated X coordinates are detected by comparing their difference exactly to zero.
    /// Use finite coordinates; NaN and infinity are not explicitly rejected.
    /// Floating-point roundoff and ill-conditioned data can reduce accuracy, particularly when
    /// converting to the monomial basis. Evaluation outside the range of the points is extrapolation.
    /// </remarks>
    public static IInterpolator InterpolateNewtonPolynom(double[] x, double[] y) => NewtonPolynomInterpolator.Create(x, y);
    /// <inheritdoc cref="TrigonometricInterpolator.Create"/>
    public static IInterpolator InterpolateTrigonometric(double[] x, double[] y, double period) => TrigonometricInterpolator.Create(x, y, period);
}
