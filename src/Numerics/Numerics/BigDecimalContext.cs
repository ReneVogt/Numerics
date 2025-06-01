namespace Revo.Numerics;

/// <summary>
/// Defines behaviour (precision) of
/// the <see cref="BigDecimal"/> type.
/// </summary>
public static class BigDecimalContext
{
    static readonly ThreadLocal<int> _precision = new(() => 28);

    /// <summary>
    /// The maximum precision used by the <see cref="BigDecimal"/> type.
    /// Changing this value does not affect any existing BigDecimal values.
    /// It is only used for expanding operations like divisions or roots to
    /// avoid infinite expansion.
    /// 
    /// This value is thread local.
    /// </summary>
    public static int Precision
    {
        get => _precision.Value;
        set => _precision.Value = value;
    }
}
