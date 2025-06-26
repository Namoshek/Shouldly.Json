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

### Basic JSON Validation

Use `ShouldBeValidJson()` to validate if the given string is valid JSON syntax. Be aware that besides arrays and objects, JSON also supports other types like strings, numbers, booleans, and null in the root of a string.

```csharp
var json = @"{""name"": ""John"", ""age"": 30}";

json.ShouldBeValidJson();
```

Use `ShouldBeJsonObject()` and `ShouldBeJsonArray()` to check if the given string is a valid JSON object or array.

```csharp
var json1 = @"{""name"": ""John"", ""age"": 30}";
var json2 = @"[1, 2, 3]";

json1.ShouldBeJsonObject();
json2.ShouldBeJsonArray();
```

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

### JSON Schema Validation

Use `ShouldMatchJsonSchema()` to validate a JSON string against a JSON Schema. This allows you to verify that JSON data conforms to an expected structure and format.

```csharp
var json = @"{""name"": ""John"", ""age"": 30}";
var schema = @"{
    ""type"": ""object"",
    ""properties"": {
        ""name"": { ""type"": ""string"" },
        ""age"": { ""type"": ""integer"", ""minimum"": 0 }
    },
    ""required"": [""name"", ""age""]
}";

json.ShouldMatchJsonSchema(schema);
```

### Property Existence Validation

Use `ShouldHaveJsonProperty()` to check if a JSON string has a specific property. You can also provide a JSON pointer to check the existence of a property at a specific path.

```csharp
var json = @"{""user"": {""name"": ""John""}}";

json.ShouldHaveJsonProperty("/user/name");
```

### Property Value Validation

Use `ShouldHaveJsonValue()` to check if a JSON string has a specific property with a specific value. You can also provide a JSON pointer to check the value of a property at a specific path.

```csharp
var json1 = @"{""user"": {""name"": ""John""}}";
var json2 = @"{""users"": [{""name"": ""John""}, {""name"": ""Jane""}]}";

json1.ShouldHaveJsonValue("/user/name", "John");
json2.ShouldHaveJsonValue("/users/2/name", "Jane");
```

Use any of the other `ShouldHaveJsonValue` methods to check the value of a property at a specific path with different comparators.

```csharp
var json = @"{""user"": {""name"": ""John"", ""age"": 30}}";

json.ShouldHaveJsonValueLessThan("/user/age", 31);
json.ShouldHaveJsonValueLessThanOrEqualTo("/user/age", 30);
json.ShouldHaveJsonValueGreaterThan("/user/age", 29);
json.ShouldHaveJsonValueGreaterThanOrEqualTo("/user/age", 30);
json.ShouldHaveJsonValueBetween("/user/age", 29, 31);
```

Special methods for date/time values are also available. They work very similar but provide better semantics for date/time values.
Also be aware that those methods use the upper bound exclusively while the other methods inclusively.

```csharp
var json = @"{""user"": {""name"": ""John"", ""birthDate"": ""1990-01-01""}}";

json.ShouldHaveJsonDateBefore("/user/birthDate", new DateTime(1990, 1, 2));
json.ShouldHaveJsonDateBeforeOrEqualTo("/user/birthDate", new DateTime(1990, 1, 2));
json.ShouldHaveJsonDateAfter("/user/birthDate", new DateTime(1990, 1, 1));
json.ShouldHaveJsonDateAfterOrEqualTo("/user/birthDate", new DateTime(1990, 1, 1));
json.ShouldHaveJsonDateBetween("/user/birthDate", new DateTime(1989, 1, 1), new DateTime(1991, 1, 1));
```

### Regex Pattern Matching

Use `ShouldHaveJsonValueMatchingRegex()` and `ShouldNotHaveJsonValueMatchingRegex()` to validate that string values at specific JSON pointer paths match or don't match regular expression patterns.

```csharp
var json = @"{
    ""name"": ""John"",
    ""email"": ""john@example.com"",
    ""phone"": ""555-1234"",
    ""invalid_email"": ""not-an-email""
}";

// Assert that values match specific patterns
json.ShouldHaveJsonValueMatchingRegex("/name", @"^[A-Z][a-z]+$");
json.ShouldHaveJsonValueMatchingRegex("/email", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
json.ShouldHaveJsonValueMatchingRegex("/phone", @"^\d{3}-\d{4}$");

// Assert that values do NOT match patterns
json.ShouldNotHaveJsonValueMatchingRegex("/invalid_email", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
json.ShouldNotHaveJsonValueMatchingRegex("/name", @"^[a-z]+$"); // Should not be all lowercase
```

### Array Length Validation

Use `ShouldHaveJsonArrayCount()` to check the length of an array. You can also provide a JSON pointer to check the length of an array at a specific path.

```csharp
var json1 = @"[1, 2, 3]";
var json2 = @"{""users"": [""John"", ""Jane""]}";

json1.ShouldHaveJsonArrayCount(3); // root array
json2.ShouldHaveJsonArrayCount(2, "/users"); // nested array
```

## License

This library is open-sourced software licensed under the [MIT license](LICENSE).
