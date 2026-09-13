# RomanNumber

[Back to the project overview](../README.md)

`RomanNumber` is a static utility class in `Revo.Numerics.Types` for converting positive integers to Roman numerals and parsing them back to `int`. It supports the symbols `I`, `V`, `X`, `L`, `C`, `D`, and `M`.

## Formatting integers

`ToRomanNumber()` is an extension method on `int`. Output uses uppercase letters and subtractive notation for four and nine in each decimal position.

```csharp
using System;
using Revo.Numerics.Types;

Console.WriteLine(2026.ToRomanNumber()); // MMXXVI
Console.WriteLine(49.ToRomanNumber());   // XLIX
Console.WriteLine(4.ToRomanNumber());    // IV
Console.WriteLine(4000.ToRomanNumber()); // MMMM
```

The formatter accepts positive `int` values, including values above 3999. Thousands are represented by repeated `M` characters; overbars and other extended numeral systems are not used. Very large inputs produce correspondingly long strings. Zero and negative numbers throw `ArgumentOutOfRangeException`.

## Parsing numerals

Use `Parse` for input expected to be valid, or `TryParse` to handle invalid input explicitly:

```csharp
using System;
using Revo.Numerics.Types;

Console.WriteLine(RomanNumber.Parse("MMXXVI")); // 2026
Console.WriteLine(RomanNumber.Parse("IIII"));   // 4

if (RomanNumber.TryParse("XLIX", out int number))
{
    Console.WriteLine(number); // 49
}

Console.WriteLine(RomanNumber.TryParse("IC", out _));   // False
Console.WriteLine(RomanNumber.TryParse("xiv", out _));  // False
Console.WriteLine(RomanNumber.TryParse(" XIV ", out _)); // False
```

Parsing is case-sensitive and does not trim whitespace. Null and empty strings are invalid. Numerals must contain valid thousands, hundreds, tens, and ones groups in that order; arbitrary subtraction such as `IC` for 99 is rejected.

`Parse` throws `ArgumentNullException` for null and `FormatException` for invalid text. `TryParse` returns `false` for invalid text; only use its output value when it returns `true`, because a failed parse may leave a partially accumulated value.

## Choosing accepted notation

`RomanParseOptions` is a flags enum. Pass it to `TryParse(string?, RomanParseOptions, out int)` to choose which forms of four and nine are accepted:

| Option | Accepted forms |
| --- | --- |
| `AllowSubtractiveNotation` | `IV`, `IX`, `XL`, `XC`, `CD`, `CM`. |
| `AllowAdditiveFourAndNine` | `IIII`, `VIIII`, `XXXX`, `LXXXX`, `CCCC`, `DCCCC`. |
| `Default` | Both flags combined; accepts both styles, including mixtures across decimal positions. |
| `None` | Neither form of four or nine; ordinary groups such as `III`, `V`, and `VIII` still work. |

`Parse(string)` and `TryParse(string?, out int)` use `Default`. There is no `Parse` overload taking options.

```csharp
using System;
using Revo.Numerics.Types;

var subtractiveOnly = RomanParseOptions.AllowSubtractiveNotation;
Console.WriteLine(RomanNumber.TryParse("IV", subtractiveOnly, out _)); // True
Console.WriteLine(RomanNumber.TryParse("IIII", subtractiveOnly, out _)); // False

var additiveOnly = RomanParseOptions.AllowAdditiveFourAndNine;
Console.WriteLine(RomanNumber.TryParse("VIIII", additiveOnly, out var nine)); // True
Console.WriteLine(nine); // 9
Console.WriteLine(RomanNumber.TryParse("IX", additiveOnly, out _)); // False

// Formatting normalizes an accepted additive spelling to subtractive notation.
Console.WriteLine(RomanNumber.Parse("LXXXXVIIII").ToRomanNumber()); // XCIX
```

Formatting always uses subtractive notation, regardless of which options were used for parsing. There is no additive-formatting option.

## API summary and range

| Method | Result |
| --- | --- |
| `number.ToRomanNumber()` | Roman numeral string for a positive `int`. |
| `RomanNumber.Parse(text)` | Parsed `int`, or an exception on invalid input. |
| `RomanNumber.TryParse(text, out number)` | Success flag using default notation options. |
| `RomanNumber.TryParse(text, options, out number)` | Success flag using explicit notation options. |

The parser allows repeated leading `M` characters, so it is not restricted to 3999. Its accumulator is an `int`, and the current implementation does not detect overflow for extremely long input. Keep the represented value within `int.MaxValue`.

Source: [RomanNumber](../src/Numerics/Numerics/Types/RomanNumber.cs) and [RomanParseOptions](../src/Numerics/Numerics/Types/RomanParseOptions.cs).
