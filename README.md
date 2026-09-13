# Revo.Numerics

A collection of numeric data structures and operations for .NET.

## Components

| Component | Description | Documentation |
| --- | --- | --- |
| **BigDecimal** | An immutable decimal number with an arbitrarily large integer mantissa, decimal arithmetic, and configurable precision for division and square roots. Implements .NET generic math interfaces. | [BigDecimal guide](doc/BigDecimal.md) |
| **RomanNumber** | Converts positive integers to Roman numerals and parses Roman numerals, with options for subtractive and additive notation. | [RomanNumber guide](doc/RomanNumber.md) |
| **LinearSolver** | Solves square linear equation systems using LU decomposition with complete pivoting. Supports reusing a factorization for multiple right-hand sides. | [LinearSolver guide](doc/LinearSolver.md) |
| **NewtonPolynomInterpolator** | Interpolates points using Newton divided differences and returns monomial coefficients in ascending order of power. | [NewtonPolynomInterpolator guide](doc/NewtonPolynomInterpolator.md) |

## Getting started

The current source targets **.NET 10**. Install the .NET 10 SDK to build the library and run the examples.

Add a project reference from your application to `src/Numerics/Numerics/Numerics.csproj`. For example, from your application's directory, adjust the path to your checkout:

```sh
dotnet add reference "<path-to-checkout>/src/Numerics/Numerics/Numerics.csproj"
```

The guides contain C# examples, API summaries, and notes on supported inputs and error handling. The namespaces are:

```csharp
using Revo.Numerics;         // BigDecimal
using Revo.Numerics.Types;   // BigDecimalContext, RomanNumber, RomanParseOptions
using Revo.Numerics.Solvers; // LinearSolver and related types
using Revo.Numerics.Interpolation; // NewtonPolynomInterpolator, IPolynomInterpolator
```

## Build and test

Run these commands from the repository root:

```sh
dotnet build src/Numerics/Numerics.sln --configuration Release
dotnet test src/Numerics/Numerics.sln --configuration Release
```

The library project also generates XML API documentation on build.

## License

Copyright © 2026 René Vogt. Released under the [MIT License](LICENSE).
