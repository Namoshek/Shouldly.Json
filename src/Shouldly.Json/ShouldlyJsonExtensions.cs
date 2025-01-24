namespace Shouldly;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;
using Json.More;

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
}
