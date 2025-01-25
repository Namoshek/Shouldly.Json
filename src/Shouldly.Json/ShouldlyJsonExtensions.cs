namespace Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.More;
using Json.Pointer;
using Json.Schema;

public static class ShouldlyJsonExtensions
{
    /// <summary>
    /// Asserts that two JSON strings are semantically equivalent.
    /// This means that the order of properties in objects does not matter,
    /// but the order of elements in arrays does.
    /// </summary>
    /// <param name="actual">The actual JSON string to compare.</param>
    /// <param name="expected">The expected JSON string to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON strings are not semantically equivalent.</exception>
    public static void ShouldBeSemanticallySameJson(this string? actual, string? expected, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON strings should be semantically the same";
        
        if (actual is null && expected is null)
        {
            return;
        }

        if (actual is null || expected is null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
        }

        try
        {
            var actualNode = JsonNode.Parse(actual);
            var expectedNode = JsonNode.Parse(expected);

            if (!actualNode.IsEquivalentTo(expectedNode))
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON represented by the actual string is a subtree of the JSON represented by the expected string.
    /// A subtree means that all properties in the actual JSON must exist in the expected JSON with the same values,
    /// but the expected JSON can have additional properties.
    /// </summary>
    /// <param name="actual">The actual JSON string to check as a subtree.</param>
    /// <param name="expected">The expected JSON string to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the actual JSON is not a subtree of the expected JSON.</exception>
    public static void ShouldBeJsonSubtreeOf(this string? actual, string? expected, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should be a subtree of expected JSON";
        
        if (actual is null && expected is null)
        {
            return;
        }

        if (actual is null || expected is null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
        }

        try
        {
            var actualNode = JsonNode.Parse(actual);
            var expectedNode = JsonNode.Parse(expected);

            if (!JsonHelper.IsJsonSubtree(actualNode, expectedNode))
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    /// <summary>
    /// Asserts that a string is valid JSON.
    /// </summary>
    /// <remarks>
    /// Be aware that some primitives like `null`, `true` and `42` are also valid JSON.
    /// It is not necessary that the string is a JSON object or array.
    /// </remarks>
    /// <param name="actual">The JSON string to validate.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the string is not valid JSON.</exception>
    public static void ShouldBeValidJson(this string? actual, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "String should be valid JSON";
        
        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
        }

        try
        {
            _ = JsonDocument.Parse(actual);
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string contains a value at the specified JSON Pointer (RFC 6901) path that matches the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="expectedValue">The expected value to compare against.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">
    /// Thrown if the value at the JSON Pointer path does not exist or does not match the expected value.
    /// </exception>
    public static void ShouldHaveJsonValueAt<T>(
        this string? actual,
        string jsonPointer,
        T expectedValue,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON should have value '{expectedValue}' at pointer '{jsonPointer}'";
        
        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
        }

        try
        {
            var actualValue = JsonHelper.GetValueAtPointer<T>(actual, jsonPointer, allowNull: true);
            
            if (actualValue is null && expectedValue is null)
            {
                return;
            }

            if (actualValue is null)
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedValue, actualValue, errorMessage).ToString());
            }

            var areEqual = comparer != null 
                ? comparer.Equals(actualValue, expectedValue)
                : EqualityComparer<T>.Default.Equals(actualValue, expectedValue);

            if (!areEqual)
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedValue, actualValue, errorMessage).ToString());
            }
        }
        catch (Exception ex) when (ex is JsonException or PointerParseException)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the numeric value at the specified JSON Pointer (RFC 6901) path is less than the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not less than the given value.</exception>
    public static void ShouldHaveJsonValueLessThan<T>(this string? actual, string jsonPointer, T value, string? customMessage = null) 
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be less than {value}";
        
        if (actualValue.CompareTo(value) >= 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"less than {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the numeric value at the specified JSON Pointer (RFC 6901) path is less than or equal to the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not less than or equal to the given value.</exception>
    public static void ShouldHaveJsonValueLessThanOrEqualTo<T>(this string? actual, string jsonPointer, T value, string? customMessage = null) 
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be less than or equal to {value}";
        
        if (actualValue.CompareTo(value) > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"less than or equal to {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the numeric value at the specified JSON Pointer (RFC 6901) path is greater than the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not greater than the given value.</exception>
    public static void ShouldHaveJsonValueGreaterThan<T>(this string? actual, string jsonPointer, T value, string? customMessage = null) 
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be greater than {value}";
        
        if (actualValue.CompareTo(value) <= 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"greater than {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the numeric value at the specified JSON Pointer (RFC 6901) path is greater than or equal to the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not greater than or equal to the given value.</exception>
    public static void ShouldHaveJsonValueGreaterThanOrEqualTo<T>(this string? actual, string jsonPointer, T value, string? customMessage = null) 
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be greater than or equal to {value}";
        
        if (actualValue.CompareTo(value) < 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"greater than or equal to {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the numeric value at the specified JSON Pointer (RFC 6901) path is between the given minimum and maximum values (inclusive).
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="min">The minimum value to compare against.</param>
    /// <param name="max">The maximum value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not between the given minimum and maximum values.</exception>
    public static void ShouldHaveJsonValueBetween<T>(this string? actual, string jsonPointer, T min, T max, string? customMessage = null) 
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be between {min} and {max}";
        
        if (actualValue.CompareTo(min) < 0 || actualValue.CompareTo(max) > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"between {min} and {max}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the date/time value at the specified JSON Pointer (RFC 6901) path is before the given value.
    /// </summary>
    /// <remarks>
    /// This method is very similar to <see cref="ShouldHaveJsonValueLessThan{T}(string?, string, T, string?)"/>
    /// but gives better semantics for date/times.
    /// </remarks>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not before the given value.</exception>
    public static void ShouldHaveJsonDateBefore<T>(this string? actual, string jsonPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON date/time at pointer '{jsonPointer}' should be before {value}";

        if (actualValue.CompareTo(value) >= 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"before {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the date/time value at the specified JSON Pointer (RFC 6901) path is before or equal to the given value.
    /// </summary>
    /// <remarks>
    /// This method is very similar to <see cref="ShouldHaveJsonValueLessThanOrEqualTo{T}(string?, string, T, string?)"/>
    /// but gives better semantics for date/times.
    /// </remarks>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not before or equal to the given value.</exception>
    public static void ShouldHaveJsonDateBeforeOrEqualTo<T>(this string? actual, string jsonPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON date/time at pointer '{jsonPointer}' should be before or equal to {value}";

        if (actualValue.CompareTo(value) > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"before or equal to {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the date/time value at the specified JSON Pointer (RFC 6901) path is after the given value.
    /// </summary>
    /// <remarks>
    /// This method is very similar to <see cref="ShouldHaveJsonValueGreaterThan{T}(string?, string, T, string?)"/>
    /// but gives better semantics for date/times.
    /// </remarks>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not after the given value.</exception>
    public static void ShouldHaveJsonDateAfter<T>(this string? actual, string jsonPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON date/time at pointer '{jsonPointer}' should be after {value}";

        if (actualValue.CompareTo(value) <= 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"after {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the date/time value at the specified JSON Pointer (RFC 6901) path is after or equal to the given value.
    /// </summary>
    /// <remarks>
    /// This method is very similar to <see cref="ShouldHaveJsonValueGreaterThanOrEqualTo{T}(string?, string, T, string?)"/>
    /// but gives better semantics for date/times.
    /// </remarks>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not after or equal to the given value.</exception>
    public static void ShouldHaveJsonDateAfterOrEqualTo<T>(this string? actual, string jsonPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON date/time at pointer '{jsonPointer}' should be after or equal to {value}";
        
        if (actualValue.CompareTo(value) < 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"after or equal to {value}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the date/time value at the specified JSON Pointer (RFC 6901) path is between the given start (inclusive) and end (exclusive) values.
    /// </summary>
    /// <remarks>
    /// This method is very similar to <see cref="ShouldHaveJsonValueBetween{T}(string?, string, T, T, string?)"/>
    /// but uses an exclusive upper bound since time is continuous. It also gives better semantics for date/times.
    /// </remarks>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="start">The inclusive start value of the range.</param>
    /// <param name="end">The exclusive end value of the range.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the JSON Pointer path is not within the specified range.</exception>
    public static void ShouldHaveJsonDateBetween<T>(this string? actual, string jsonPointer, T start, T end, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON date at pointer '{jsonPointer}' should be between {start} (inclusive) and {end} (exclusive)";

        if (actualValue.CompareTo(start) < 0 || actualValue.CompareTo(end) >= 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"between {start} (inclusive) and {end} (exclusive)", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string has a property at the specified JSON Pointer (RFC 6901) path.
    /// The property can have any value, including null.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the property. Can be in standard notation (/path/to/property) or URI Fragment Identifier notation (#/path/to/property).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the property does not exist at the specified path.</exception>
    public static void ShouldHaveJsonProperty(this string? actual, string jsonPointer, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON should have a property at pointer '{jsonPointer}'";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null").ToString());
            }

            var pointer = JsonPointer.Parse(jsonPointer);
            if (!pointer.TryEvaluate(jsonNode, out var _))
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer '{jsonPointer}': {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string has an object as its root element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON string does not have an object as its root element.</exception>
    public static void ShouldBeJsonObject(this string? actual, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON string should have an object as root element";
        
        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode?.GetValueKind() != JsonValueKind.Object)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string has an array as its root element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON string does not have an array as its root element.</exception>
    public static void ShouldBeJsonArray(this string? actual, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON string should have an array as root element";
        
        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode?.GetValueKind() != JsonValueKind.Array)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has the expected number of elements.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="expectedCount">The expected number of elements in the array.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Can be in standard notation (/path/to/array) or URI Fragment Identifier notation (#/path/to/array). Defaults to root pointer "/".</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or has a different number of elements.</exception>
    public static void ShouldHaveJsonArrayCount(this string? actual, int expectedCount, string jsonPointer = "", string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should have {expectedCount} elements";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null").ToString());
            }

            var pointer = JsonPointer.Parse(jsonPointer);
            if (!pointer.TryEvaluate(jsonNode, out var result))
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"No value found at JSON pointer '{jsonPointer}'").ToString());
            }

            if (result?.GetValueKind() != JsonValueKind.Array)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Value at JSON pointer '{jsonPointer}' is not an array").ToString());
            }

            var arrayNode = result.AsArray();
            var actualCount = arrayNode.Count;

            if (actualCount != expectedCount)
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedCount, actualCount, errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer '{jsonPointer}': {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string is valid according to the provided JSON schema.
    /// </summary>
    /// <param name="actual">The JSON string to validate.</param>
    /// <param name="schema">The JSON schema to validate against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON is invalid or does not conform to the schema.</exception>
    public static void ShouldMatchJsonSchema(this string? actual, string schema, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should match the provided schema";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        JsonSchema? jsonSchema = null;
        try
        {
            jsonSchema = JsonSchema.FromText(schema);
            if (jsonSchema is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "Invalid JSON Schema").ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON Schema: {ex.Message}").ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);

            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null").ToString());
            }

            var evaluationResults = jsonSchema.Evaluate(jsonNode, new EvaluationOptions
            {
                OutputFormat = OutputFormat.List,
                RequireFormatValidation = true,
            });

            if (evaluationResults is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON Schema evaluation failed").ToString());
            }

            if (!evaluationResults.IsValid)
            {
                var errors = evaluationResults.Details.SelectMany(d => d.Errors?.Select(kvp => $"{kvp.Key}: {kvp.Value}") ?? []);

                throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}:\n{string.Join("\n", errors)}").ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (JsonSchemaException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(schema, $"Invalid JSON Schema: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Gets and validates a JSON value at the specified pointer path.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value to.</typeparam>
    /// <param name="json">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="customMessage">An optional custom message to include in the exception.</param>
    /// <returns>The value at the specified pointer path.</returns>
    /// <exception cref="ShouldAssertException">
    /// Thrown when:
    /// - The input JSON is null
    /// - The JSON is invalid
    /// - The pointer is invalid
    /// - The value cannot be found at the specified pointer
    /// - The value cannot be converted to the specified type
    /// </exception>
    private static T GetAndValidateJsonValue<T>(string? json, string jsonPointer, string? customMessage) where T : struct
    {
        try
        {
            return JsonHelper.GetValueAtPointer<T>(json, jsonPointer);
        }
        catch (ShouldAssertException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(json, customMessage ?? ex.Message).ToString());
        }
    }
}
