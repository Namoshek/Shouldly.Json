namespace Shouldly;

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;
using Json.More;

public static class ShouldlyJsonExtensions
{
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

    public static void ShouldHaveJsonValueAt<T>(this string? actual, string jsonPointer, T expectedValue, IEqualityComparer<T>? comparer = null, string? customMessage = null)
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
