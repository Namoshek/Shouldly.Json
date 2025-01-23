# Shouldly.Json [![Nuget](https://img.shields.io/nuget/v/Shouldly.Json?style=flat-square)](https://nuget.org/packages/Shouldly.Json)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Namoshek_Shouldly.Json&metric=coverage)](https://sonarcloud.io/summary/new_code?id=Namoshek_Shouldly.Json)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Namoshek_Shouldly.Json&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=Namoshek_Shouldly.Json)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Namoshek_Shouldly.Json&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=Namoshek_Shouldly.Json)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Namoshek_Shouldly.Json&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=Namoshek_Shouldly.Json)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Namoshek_Shouldly.Json&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=Namoshek_Shouldly.Json)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=Namoshek_Shouldly.Json&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=Namoshek_Shouldly.Json)

[`Shouldly.Json`](https://www.nuget.org/packages/Shouldly.Json/) was created by, and is maintained
by [Marvin Mall](https://github.com/namoshek).
It provides extension methods for the [Shouldly](https://github.com/shouldly/shouldly) library to compare JSON strings.

## Installation

The package can be found on [nuget.org](https://www.nuget.org/packages/Shouldly.Json/).
You can install the package with:

```pwsh
Install-Package Shouldly.Json
```

## Usage

The library provides multiple methods for comparing JSON strings.

### Semantic JSON Equality

Use `ShouldBeSemanticallySameJson()` to compare two JSON strings for semantic equality. This means:
- Property order in objects doesn't matter
- Whitespace and formatting doesn't matter
- Numbers with same value but different representation (e.g., 1.0 and 1.00) are considered equal
- Array order matters

```csharp
var json1 = @"{""name"": ""John"", ""age"": 30}";
var json2 = @"{""age"": 30, ""name"": ""John""}";
var json3 = @"{""name"": ""John"", ""age"": 31}";

json1.ShouldBeSemanticallySameJson(json2); // Succeeds
json1.ShouldBeSemanticallySameJson(json3); // Fails
```

### JSON Subtree Matching

Use `ShouldBeJsonSubtreeOf()` to verify that one JSON structure is contained within another. This means:
- The actual JSON must be a subset of the expected JSON
- Missing properties in objects are allowed
- Arrays must match exactly (including length and order), but objects in arrays are handled like other objects

```csharp
var json1 = @"{""name"": ""John""}";
var json2 = @"{""name"": ""John"", ""age"": 30}";
var json3 = @"{""name"": ""Adrian""}";
var json4 = @"{""firstName"": ""John""}";

json1.ShouldBeJsonSubtreeOf(json2); // Succeeds
json1.ShouldBeJsonSubtreeOf(json3); // Fails
json1.ShouldBeJsonSubtreeOf(json4); // Fails
```

## License

This library is open-sourced software licensed under the [MIT license](LICENSE).
