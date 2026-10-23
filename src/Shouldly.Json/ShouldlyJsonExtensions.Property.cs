namespace Shouldly;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;

public static partial class ShouldlyJsonExtensions
{
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
    /// Asserts that the JSON string does not have a property at the specified JSON Pointer (RFC 6901) path.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the property. Can be in standard notation (/path/to/property) or URI Fragment Identifier notation (#/path/to/property).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the property exists at the specified path or if the JSON is invalid.</exception>
    public static void ShouldNotHaveJsonProperty(this string? actual, string jsonPointer, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON should not have a property at pointer '{jsonPointer}'";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual ?? "");
            if (jsonNode == null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null").ToString());
            }

            var pointer = JsonPointer.Parse(jsonPointer);
            if (pointer.TryEvaluate(jsonNode, out var _))
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
}