namespace Revo.Numerics.Interpolation;

/// <summary>
/// Represents an interpolating function that can be evaluated and exposes its coefficients.
/// </summary>
public interface IInterpolator
{
    /// <summary>
    /// Gets the coefficients of the interpolating function.
    /// </summary>
    /// <value>The coefficients in the basis and order defined by the interpolation method.</value>
    /// <remarks>
    /// For instances returned by <see cref="Interpolators.InterpolateNewtonPolynom"/>,
    /// each access returns a new array of monomial coefficients in ascending order of power.
    /// The monomial coefficients are computed on first access and cached internally.
    /// For instances returned by <see cref="Interpolators.InterpolateTrigonometric"/>,
    /// each access returns a new array of cosine/sine pairs as described by
    /// <see cref="TrigonometricInterpolator.Coefficients"/>.
    /// </remarks>
    double[] Coefficients { get; }

    /// <summary>
    /// Evaluates the interpolating function at the specified coordinate.
    /// </summary>
    /// <param name="x">The X coordinate at which to evaluate the function.</param>
    /// <returns>The value of the interpolating function at <paramref name="x"/>.</returns>
    double Evaluate(double x);
}
