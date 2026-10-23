namespace Shouldly;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;

public static partial class ShouldlyJsonExtensions
{
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
            using var document = JsonDocument.Parse(actual);
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the string is not valid JSON.
    /// </summary>
    /// <remarks>
    /// Be aware that some primitives like `null`, `true` and `42` are valid JSON,
    /// so they make this assertion fail.
    /// </remarks>
    /// <param name="actual">The string to validate.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the string is valid JSON.</exception>
    public static void ShouldNotBeValidJson(this string? actual, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "String should not be valid JSON";

        if (actual is null)
        {
            return;
        }

        try
        {
            using var document = JsonDocument.Parse(actual);
        }
        catch (JsonException)
        {
            return;
        }

        throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
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
    /// Asserts that the JSON string does not have an object as its root element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON string has an object as its root element or is not valid JSON.</exception>
    public static void ShouldNotBeJsonObject(this string? actual, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON string should not have an object as root element";

        if (actual is null)
        {
            return;
        }

        JsonNode? jsonNode;
        try
        {
            jsonNode = JsonNode.Parse(actual);
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }

        if (jsonNode?.GetValueKind() == JsonValueKind.Object)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
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
    /// Asserts that the JSON string does not have an array as its root element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON string has an array as its root element or is not valid JSON.</exception>
    public static void ShouldNotBeJsonArray(this string? actual, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON string should not have an array as root element";

        if (actual is null)
        {
            return;
        }

        JsonNode? jsonNode;
        try
        {
            jsonNode = JsonNode.Parse(actual);
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }

        if (jsonNode?.GetValueKind() == JsonValueKind.Array)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
        }
    }
}