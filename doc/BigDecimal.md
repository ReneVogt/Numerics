# BigDecimal

[Back to the project overview](../README.md)

`BigDecimal` is an immutable, signed decimal value type in the `Revo.Numerics` namespace. It combines a `System.Numerics.BigInteger` mantissa with an `int` exponent:

```text
value = Mantissa × 10^Exponent
```

This allows decimal values with more digits than the built-in `decimal` type. Addition, subtraction, and multiplication are not rounded to a fixed precision. Division and square roots use a configurable number of fractional decimal digits.

## Creating values

Construct values from `decimal`, `int`, `uint`, `long`, `ulong`, or `BigInteger`, or supply a mantissa and exponent directly. These numeric types also have implicit conversions to `BigDecimal`.

```csharp
using System;
using System.Numerics;
using Revo.Numerics;

BigDecimal price = 12.50m;
BigDecimal quantity = 3;
var fraction = new BigDecimal(new BigInteger(125), -2); // 1.25
var large = new BigDecimal(BigInteger.Pow(10, 50) + 1);

Console.WriteLine(price.Mantissa); // 125
Console.WriteLine(price.Exponent); // -1
Console.WriteLine(price * quantity == new BigDecimal(37.5m)); // True
Console.WriteLine(fraction == new BigDecimal(1.25m)); // True
```

Values are normalized by removing trailing decimal zeros from the mantissa and increasing the exponent accordingly. For example, `(1250, -3)` becomes `(125, -2)`. Zero is always represented as `(0, 0)`, including `default(BigDecimal)`. Original decimal scale and trailing zeros are not preserved.

Use decimal literals with the `m` suffix for fractional values. There are no direct `double` or `float` conversions.

## Arithmetic and comparisons

The usual `+`, `-`, `*`, `/`, `%`, unary signs, `++`, `--`, and comparison operators are available. Named instance methods include `Add`, `Subtract`, `MultiplyBy`, `DivideBy`, and `ModulusBy`.

```csharp
using System;
using Revo.Numerics;

BigDecimal value = 10.5m;
Console.WriteLine(value + 2 == new BigDecimal(12.5m)); // True
Console.WriteLine(value % 4 == new BigDecimal(2.5m)); // True

var (quotient, remainder) = BigDecimal.DivRem(value, 4);
Console.WriteLine(quotient == 2); // True
Console.WriteLine(remainder == new BigDecimal(2.5m)); // True

Console.WriteLine(value.Shift(2) == 1050); // True
Console.WriteLine((value >> 1) == new BigDecimal(1.05m)); // True
```

`DivRem` returns an integer quotient truncated toward zero and the corresponding remainder. The remainder has the dividend's sign unless it is zero.

`Shift(n)` and `<< n` multiply by `10^n`; `>> n` divides by `10^n`. These are **decimal shifts**, not binary shifts.

## Precision for division and square roots

Use explicit precision for a single operation:

```csharp
using System;
using System.Globalization;
using Revo.Numerics;

BigDecimal one = 1;
var third = one.DivideBy(3, precision: 6);
var root = BigDecimal.Sqrt(2, precision: 6);

Console.WriteLine(third.ToString("E", CultureInfo.InvariantCulture)); // 333333 E-6
Console.WriteLine(root.ToString("E", CultureInfo.InvariantCulture));  // 1414213 E-6
Console.WriteLine(one.DivideBy(8, precision: 2) == new BigDecimal(0.12m)); // True
```

Precision counts digits **after the decimal point**, not significant digits. Results are truncated rather than rounded; division truncates toward zero. Use non-negative precision values. Trailing zeros may disappear during normalization, so the result can contain fewer fractional digits than requested.

The `/` operator, `DivideBy(divisor)`, `Sqrt()`, and `BigDecimal.Sqrt(value)` use `BigDecimalContext.Precision`. Its default is **28**, and it is **thread-local**:

```csharp
using System;
using Revo.Numerics;
using Revo.Numerics.Types;

int previousPrecision = BigDecimalContext.Precision;
try
{
    BigDecimalContext.Precision = 4;
    BigDecimal one = 1;
    Console.WriteLine(one / 3 == new BigDecimal(0.3333m)); // True
}
finally
{
    BigDecimalContext.Precision = previousPrecision;
}
```

Changing the context does not alter existing values or limit addition, subtraction, or multiplication. A thread-local setting does not automatically follow asynchronous execution to another thread; explicit precision is useful when that matters.

## Parsing and formatting

The text representation is an integer mantissa optionally followed by a space, uppercase `E`, and an integer exponent. For example, `"12345 E-2"` represents `123.45`. With exponent zero, the suffix is omitted.

```csharp
using System;
using System.Globalization;
using Revo.Numerics;

var value = BigDecimal.Parse("12345 E-2", CultureInfo.InvariantCulture);
Console.WriteLine(value == new BigDecimal(123.45m)); // True
Console.WriteLine(value.ToString("E", CultureInfo.InvariantCulture)); // 12345 E-2

Console.WriteLine(BigDecimal.TryParse("42", out var integer)); // True
Console.WriteLine(BigDecimal.TryParse("123.45", out _)); // False
Console.WriteLine(BigDecimal.TryParse("1.23e4", out _)); // False
```

- The default parsing style is `NumberStyles.Integer`. Overloads accept a `NumberStyles` value and format provider for parsing the integer components.
- The exponent syntax requires the space immediately before uppercase `E`. It is not the usual floating-point scientific notation.
- `ToString()` uses the current culture. Use `ToString("E", CultureInfo.InvariantCulture)` for a stable interchange representation.
- `ToString(format, provider)` accepts `"E"` or `null`; other formats, including `"F2"`, `"G"`, and an empty string, throw `FormatException`.
- Span-based `Parse`, `TryParse`, and `TryFormat` are available. `TryFormat` accepts `"E"` or an empty format span, and returns `false` if the destination is too small.

## Conversions and generic math

`BigDecimal` implements `INumber<BigDecimal>` and `ISignedNumber<BigDecimal>`, so it can be used in generic arithmetic constrained by those interfaces.

The `TryConvertFromChecked`, `TryConvertFromSaturating`, and `TryConvertFromTruncating` methods directly support `decimal`, `int`, `uint`, `long`, `ulong`, and `BigInteger`. The corresponding `TryConvertTo...` methods support the same target types and report unsupported or unrepresentable conversions with `false`.

```csharp
using System;
using System.Numerics;
using Revo.Numerics;

BigDecimal value = 12.75m;
Console.WriteLine(BigDecimal.TryConvertToChecked<int>(value, out _)); // False
Console.WriteLine(BigDecimal.TryConvertToTruncating<int>(value, out var integer)); // True
Console.WriteLine(integer); // 12

var large = new BigDecimal(BigInteger.Pow(10, 50));
bool converted = BigDecimal.TryConvertToChecked<BigInteger>(large, out var result);
Console.WriteLine(converted && result == BigInteger.Pow(10, 50)); // True
```

For integral targets, checked conversion requires an integral value in range; truncating conversion discards the fractional part but still requires the value to be in range. Saturating conversion requires an integral value and clamps it to the target's range.

Explicit casts to `int`, `uint`, `long`, and `ulong` truncate fractions and clamp out-of-range values. The explicit `decimal` cast also clamps to its range and reduces precision as needed. For large `BigInteger` results, use `TryConvertToChecked<BigInteger>` or `TryConvertToTruncating<BigInteger>`: the current explicit `BigInteger` cast passes through `int` and can overflow.

## Other useful members

| Member | Purpose |
| --- | --- |
| `Zero`, `One`, `NegativeOne` | Common constants. |
| `Mantissa`, `Exponent`, `Deconstruct` | Inspect the normalized representation. |
| `Sign`, `Abs`, `IsZero`, `IsInteger`, `IsEvenInteger`, `IsOddInteger` | Inspect sign and numeric properties. |
| `NumberOfDigits` | Number of digits in the mantissa, rather than the formatted value. |
| `EnumerateIntegralDigits()` | Integral digits of the absolute value, least significant first: `123.45` yields `3, 2, 1`. |
| `EnumerateFractionalDigits()` | Fractional digits of the absolute value, left to right: `123.45` yields `4, 5`. |

## Limits and errors

- The mantissa is arbitrary-size, but the exponent is a 32-bit integer. Memory, execution time, and exponent arithmetic impose practical limits; very large exponent differences can require large intermediate integers.
- There are no NaN or infinity values.
- Division, remainder, and `DivRem` with a zero divisor throw `DivideByZeroException`.
- For a negative argument, instance `Sqrt` throws `ArithmeticException`; static `BigDecimal.Sqrt` throws `ArgumentOutOfRangeException`.
- `Parse` throws `FormatException` for invalid text and `ArgumentNullException` for a null string. `TryParse` handles ordinary invalid input without a parsing exception, but representation overflow can still throw during normalization.

Source: [BigDecimal implementation](../src/Numerics/Numerics/Types/BigDecimal.cs) and its adjacent partial files; [BigDecimalContext](../src/Numerics/Numerics/Types/BigDecimalContext.cs).
