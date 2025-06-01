using System.Numerics;

namespace Revo.Numerics;

public readonly partial struct BigDecimal
{
    /// <summary>
    /// Calculates the square root of the current
    /// <see cref="BigDecimal"/> using up to <see cref="BigDecimalContext.Precision"/>
    /// decimal digits.
    /// </summary>
    /// <returns>The square root of the current <see cref="BigDecimal"/> truncated at
    /// <see cref="BigDecimalContext.Precision"/> decimal digits.</returns>.
    public BigDecimal Sqrt() => Sqrt(BigDecimalContext.Precision);
    /// <summary>
    /// Calculates the square root of the current
    /// <see cref="BigDecimal"/> using up to <see cref="BigDecimalContext.Precision"/>
    /// decimal digits.
    /// </summary>
    /// <returns>The square root of the current <see cref="BigDecimal"/> truncated at
    /// <see cref="BigDecimalContext.Precision"/> decimal digits.</returns>.
    public BigDecimal Sqrt(int precision) => throw new NotImplementedException();

    /// <summary>
    /// Calculates the square root of the given
    /// <see cref="BigDecimal"/> using up to <paramref name="precision"/>
    /// decimal digits.
    /// </summary>
    /// <param name="value">The <see cref="BigDecimal"/> value to calculate the square root of.</param>
    /// <param name="precision">The number of decimal digits to generate.</param>
    /// <returns>The square root of the given <paramref name="value"/> truncated at
    /// <paramref name="precision"/> decimal digits.</returns>.
    public static BigDecimal Sqrt(BigDecimal value) => value.Sqrt();
    /// <summary>
    /// Calculates the square root of the given
    /// <see cref="BigDecimal"/> using up to <paramref name="precision"/>#
    /// decimal digits.
    /// </summary>
    /// <param name="value">The <see cref="BigDecimal"/> value to calculate the square root of.</param>
    /// <param name="precision">The number of decimal digits to generate.</param>
    /// <returns>The square root of the given <paramref name="value"/> truncated at
    /// <paramref name="precision"/> decimal digits.</returns>.
    public static BigDecimal Sqrt(BigDecimal value, int precision) => value.Sqrt(precision);
}
