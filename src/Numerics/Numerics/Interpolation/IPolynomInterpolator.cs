namespace Revo.Numerics.Interpolation;

/// <summary>
/// Defines polynomial interpolation through a set of points, returning coefficients in the monomial basis.
/// </summary>
public interface IPolynomInterpolator
{
    /// <summary>
    /// Calculates the coefficients of the polynomial through the supplied points.
    /// </summary>
    /// <param name="x">The distinct X coordinates of the points, in any order.</param>
    /// <param name="y">The corresponding Y coordinates, with the same length as <paramref name="x"/>.</param>
    /// <returns>
    /// The coefficients in ascending order of power: element i multiplies x raised to the power i.
    /// The result has one element per point, including trailing zero coefficients for lower-degree polynomials.
    /// Empty inputs return an empty array; a single point returns its Y coordinate as the constant coefficient.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="x"/> or <paramref name="y"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The arrays have different lengths, or X coordinates are repeated.</exception>
    /// <remarks>
    /// The input arrays are not modified. Use finite coordinates; floating-point roundoff can affect the result.
    /// </remarks>
    double[] Interpolate(double[] x, double[] y);
}
