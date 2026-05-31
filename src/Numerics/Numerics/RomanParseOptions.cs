namespace Revo.Numerics;

[Flags]
public enum RomanParseOptions
{
    None = 0,
    AllowSubtractiveNotation = 1 << 0,      // IV, IX, XL, XC, CD, CM
    AllowAdditiveFourAndNine = 1 << 1,      // IIII, VIIII, XXXX, LXXXX, CCCC, DCCCC
    Default = AllowSubtractiveNotation | AllowAdditiveFourAndNine
}
