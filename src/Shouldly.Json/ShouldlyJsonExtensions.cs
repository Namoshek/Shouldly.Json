﻿namespace Shouldly;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;
using Json.More;
using Humanizer;

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
        
        if (actual == null && expected == null)
        {
            return;
        }

        if (actual == null || expected == null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
        }

        if (expected == null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(null, actual, errorMessage).ToString());
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
        
        if (actual == null && expected == null)
        {
            return;
        }

        if (actual == null || expected == null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, null, errorMessage).ToString());
        }

        if (expected == null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(null, actual, errorMessage).ToString());
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
        
        if (actual == null)
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
        
        if (actual == null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
        }

        try
        {
            var actualValue = JsonHelper.GetValueAtPointer<T>(actual, jsonPointer);
            
            if (actualValue == null && expectedValue == null)
            {
                return;
            }

            if (actualValue == null || expectedValue == null)
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
    /// Asserts that the numeric value at the specified JSON Pointer path is less than the given value.
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
    /// Asserts that the numeric value at the specified JSON Pointer path is less than or equal to the given value.
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
    /// Asserts that the numeric value at the specified JSON Pointer path is greater than the given value.
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
    /// Asserts that the numeric value at the specified JSON Pointer path is greater than or equal to the given value.
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
    /// Asserts that the numeric value at the specified JSON Pointer path is between the given minimum and maximum values (inclusive).
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
    /// Asserts that the date/time value at the specified JSON Pointer path is before the given value.
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
    /// Asserts that the date/time value at the specified JSON Pointer path is before or equal to the given value.
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
    /// Asserts that the date/time value at the specified JSON Pointer path is after the given value.
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
    /// Asserts that the date/time value at the specified JSON Pointer path is after or equal to the given value.
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
    /// Asserts that the date/time value at the specified JSON Pointer path is between the given start (inclusive) and end (exclusive) values.
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

    private static T GetAndValidateJsonValue<T>(string? json, string jsonPointer, string? customMessage) where T : struct
    {
        if (json == null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(null, customMessage ?? "JSON string is null").ToString());
        }

        try
        {
            return JsonHelper.GetValueAtPointer<T>(json, jsonPointer);
        }
        catch (Exception ex) when (ex is JsonException or PointerParseException)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(json, $"{customMessage ?? "Invalid JSON or pointer"}: {ex.Message}").ToString());
        }
    }
}
