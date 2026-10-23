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

Use `ShouldNotBeValidJson()`, `ShouldNotBeJsonObject()` and `ShouldNotBeJsonArray()` for the opposite assertions. A `null` or empty string is not valid JSON, so it satisfies the negated assertions.

```csharp
"{invalid json}".ShouldNotBeValidJson();
"[1, 2, 3]".ShouldNotBeJsonObject();
@"{""name"": ""John""}".ShouldNotBeJsonArray();
```

> **Note:** methods taking a `jsonPointer` argument default to the root pointer `""`, not `"/"`. Pointers accept both standard notation (`/path/to/array`) and URI fragment notation (`#/path/to/array`).

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

Use `ShouldNotBeSemanticallySameJson()` to assert that two JSON structures differ. Failure messages of the semantic comparison report the JSON pointer path of the first difference found.

```csharp
var json1 = @"{""name"": ""John"", ""age"": 30}";
var json2 = @"{""name"": ""Jane"", ""age"": 30}";

json1.ShouldNotBeSemanticallySameJson(json2); // Succeeds
json1.ShouldNotBeSemanticallySameJson(json1); // Fails
json1.ShouldBeSemanticallySameJson(json2); // Fails: "JSON value mismatch at path '/name'"
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

Use `ShouldHaveJsonSubtree()` to check the reverse direction: the _expected_ JSON must be a subtree of the _actual_ JSON.

```csharp
var json1 = @"{""name"": ""John""}";
var json2 = @"{""name"": ""John"", ""age"": 30}";

json2.ShouldHaveJsonSubtree(json1); // Succeeds
json1.ShouldHaveJsonSubtree(json2); // Fails
```

Use `ShouldNotBeJsonSubtreeOf()` and `ShouldNotHaveJsonSubtree()` for the negated assertions.

```csharp
var json1 = @"{""name"": ""John""}";
var json2 = @"{""name"": ""John"", ""age"": 30}";

json1.ShouldNotBeJsonSubtreeOf(json1); // Fails (every JSON is a subtree of itself)
json2.ShouldNotBeJsonSubtreeOf(json1); // Succeeds (json2 has an extra property)

json2.ShouldNotHaveJsonSubtree(json1); // Fails (json1 is present in json2)
json1.ShouldNotHaveJsonSubtree(json2); // Succeeds
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

Use `ShouldNotMatchJsonSchema()` to assert that a JSON string violates a schema.

```csharp
@"{""name"": 42}".ShouldNotMatchJsonSchema(schema); // Succeeds
@"{""name"": ""John""}".ShouldNotMatchJsonSchema(schema); // Fails
```

### Property Existence Validation

Use `ShouldHaveJsonProperty()` to check if a JSON string has a specific property. You can also provide a JSON pointer to check the existence of a property at a specific path.

```csharp
var json = @"{""user"": {""name"": ""John""}}";

json.ShouldHaveJsonProperty("/user/name");
```

Use `ShouldNotHaveJsonProperty()` to assert that a property does not exist. Note that a property with a `null` value _does_ exist.

```csharp
var json = @"{""user"": {""name"": ""John"", ""age"": null}}";

json.ShouldNotHaveJsonProperty("/user/email"); // Succeeds
json.ShouldHaveJsonProperty("/user/age"); // Succeeds (the property exists, value is null)
```

### JSON Value Type Validation

Use `ShouldHaveJsonValueType()` to check the JSON type of the value at a path, or the named shortcuts for the six JSON types. Booleans are checked as a single type (`JsonValueKind.True` and `JsonValueKind.False` are both booleans). `null` is a value kind of its own, distinct from a missing pointer.

```csharp
var json = @"{""name"": ""John"", ""age"": 30, ""active"": true, ""meta"": {""id"": 1}, ""tags"": [""a""], ""deletedAt"": null}";

json.ShouldHaveJsonValueType("/name", JsonValueKind.String);
json.ShouldBeJsonStringAt("/name");
json.ShouldBeJsonNumberAt("/age");
json.ShouldBeJsonBooleanAt("/active");
json.ShouldBeJsonObjectAt("/meta");
json.ShouldBeJsonArrayAt("/tags");
json.ShouldHaveJsonNullValue("/deletedAt");

json.ShouldNotBeJsonStringAt("/age");
json.ShouldNotHaveJsonValueType("/name", JsonValueKind.Number);
json.ShouldNotHaveJsonNullValue("/name");
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

Use `ShouldNotHaveJsonValue()`, `ShouldNotHaveJsonValueBetween()` and `ShouldHaveJsonValueOneOf()` / `ShouldNotHaveJsonValueOneOf()` for negated and set-based comparisons. A pointer that does not exist satisfies the negated assertions.

```csharp
var json = @"{""user"": {""name"": ""John"", ""status"": ""active"", ""age"": 30}}";

json.ShouldNotHaveJsonValue("/user/name", "Jane");
json.ShouldNotHaveJsonValueBetween("/user/age", 40, 50);
json.ShouldHaveJsonValueOneOf("/user/status", new[] { "active", "pending" });
json.ShouldNotHaveJsonValueOneOf("/user/status", new[] { "deleted", "archived" });
json.ShouldNotHaveJsonValue("/user/name", "john", StringComparer.OrdinalIgnoreCase); // Fails (case-insensitive match)
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

Use `ShouldNotHaveJsonDateBetween()` for the negated range check (same bounds: start inclusive, end exclusive).

```csharp
json.ShouldNotHaveJsonDateBetween("/user/birthDate", new DateTime(1980, 1, 1), new DateTime(1985, 1, 1));
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

`ShouldAnyHaveJsonValueMatchingRegex()` and `ShouldAnyNotHaveJsonValueMatchingRegex()` apply a pattern to a property of the elements of a JSON array, requiring at least one match (or at least one non-match) respectively.

```csharp
var json = @"{""users"": [{""email"": ""a@b.com""}, {""email"": ""invalid""}]}";

json.ShouldAnyHaveJsonValueMatchingRegex("/users", "/email", @"^[^@]+@[^@]+\.[^@]+$"); // Succeeds
json.ShouldAnyNotHaveJsonValueMatchingRegex("/users", "/email", @"^\d+$"); // Succeeds
```

### Array Length Validation

Use `ShouldHaveJsonArrayCount()` to check the length of an array. The `jsonPointer` argument is optional and defaults to the root pointer `""`, so omitting it checks an array in the root of the JSON string.

```csharp
var json1 = @"[1, 2, 3]";
var json2 = @"{""users"": [""John"", ""Jane""]}";

json1.ShouldHaveJsonArrayCount(3); // root array, pointer omitted
json2.ShouldHaveJsonArrayCount(2, "/users"); // nested array
```

`ShouldNotHaveJsonArrayCount()` is the exact inverse: it takes the same optional `jsonPointer` and fails precisely when `ShouldHaveJsonArrayCount()` would succeed, so omitting the pointer there also means "the root array".

```csharp
var json1 = "[1, 2, 3]";
var json2 = @"{""users"": [""John"", ""Jane""]}";

json1.ShouldNotHaveJsonArrayCount(2); // inverse of ShouldHaveJsonArrayCount(2) on the root array
json1.ShouldHaveJsonArrayCount(3); // the positive counterpart succeeds
json2.ShouldNotHaveJsonArrayCount(3, "/users"); // inverse with an explicit pointer
```

Range-based length assertions are available too, with the same default root pointer and inclusive bounds.

```csharp
var json1 = "[1, 2, 3]";
var json2 = @"{""users"": [""John"", ""Jane""]}";

json1.ShouldHaveJsonArrayCountBetween(2, 4); // 2, 3 or 4 elements
json1.ShouldNotHaveJsonArrayCountBetween(5, 8);

json2.ShouldBeJsonArrayNotEmpty("/users");
json2.ShouldNotBeJsonArrayEmpty("/users");
@"{""users"": []}".ShouldBeJsonArrayEmpty("/users");
```

### Array Content Validation

Use `ShouldContainJsonElement()` / `ShouldNotContainJsonElement()` to check whether a JSON array contains an element semantically equal to the given JSON. Elements are compared with the same semantics as `ShouldBeSemanticallySameJson()`: property order is ignored, array order matters, and `42` equals `42.0`.

```csharp
var json = @"{""users"": [{""name"": ""John"", ""age"": 30}, {""name"": ""Jane""}]}";

json.ShouldContainJsonElement("/users", @"{""age"": 30, ""name"": ""John""}"); // Succeeds
json.ShouldNotContainJsonElement("/users", @"{""name"": ""Bob""}"); // Succeeds
```

Use `ShouldHaveJsonArrayUniqueElements()` to assert that an array contains no semantically duplicate elements; duplicates are reported with their indices.

```csharp
"[1, 2, 3]".ShouldHaveJsonArrayUniqueElements();
"[1, 2, 1]".ShouldHaveJsonArrayUniqueElements(); // Fails: "duplicate elements at indices: 2"
"[42, 42.0]".ShouldHaveJsonArrayUniqueElements(); // Fails: numerically equal
"[1, 1, 2]".ShouldNotHaveJsonArrayUniqueElements(); // Succeeds
```

### Array Element Assertions

Use `ShouldAllHaveJsonProperty()` / `ShouldAllNotHaveJsonProperty()` to assert that every element of a JSON array has (or does not have) a property, and `ShouldAnyHaveJsonProperty()` / `ShouldNotAnyHaveJsonProperty()` for the "at least one element" variants. Failures report the offending indices.

```csharp
var json = @"{""users"": [{""name"": ""John"", ""age"": 30}, {""name"": ""Jane"", ""age"": 25}]}";

json.ShouldAllHaveJsonProperty("/users", "/name");
json.ShouldAllNotHaveJsonProperty("/users", "/email");
json.ShouldAnyHaveJsonProperty("/users", "/age");
json.ShouldNotAnyHaveJsonProperty("/users", "/email");
```

Use the `ShouldAllHaveJsonValue*` / `ShouldAllHaveJsonDate*` family to compare the value of a property of every array element, and the `ShouldAllNotHaveJsonValue*` / `ShouldAllNotHaveJsonDate*` family for negated checks. `ShouldAnyHaveJsonValue()` and `ShouldAllNotHaveJsonValue()` check whether at least one element matches a value, and whether no element matches it.

```csharp
var json = @"{""users"": [{""age"": 30}, {""age"": 25}, {""age"": 40}]}";

json.ShouldAllHaveJsonValueGreaterThan("/users", "/age", 20);
json.ShouldAllHaveJsonValueBetween("/users", "/age", 20, 50);
json.ShouldAllNotHaveJsonValueBetween("/users", "/age", 50, 60);
json.ShouldAllNotHaveJsonValue("/users", "/age", 35);
json.ShouldAnyHaveJsonValue("/users", "/age", 25);
```

The date variants work the same way, with the upper bound exclusive:

```csharp
var json = @"{""users"": [{""created"": ""2024-01-02""}, {""created"": ""2024-01-03""}]}";

json.ShouldAllHaveJsonDateAfter("/users", "/created", new DateTime(2024, 1, 1));
json.ShouldAllNotHaveJsonDateBetween("/users", "/created", new DateTime(2024, 2, 1), new DateTime(2024, 3, 1));
```

## License

This library is open-sourced software licensed under the [MIT license](LICENSE).
