namespace Shouldly;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;

public static partial class ShouldlyJsonExtensions
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

            if (actualNode == null && expectedNode == null)
            {
                return;
            }

            if (actualNode == null || expectedNode == null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "Both JSON strings parsed to null").ToString());
            }

            var comparisonResult = JsonComparisonEngine.CompareSemanticEquality(actualNode, expectedNode);

            if (!comparisonResult.IsEqual)
            {
                var enhancedMessage = comparisonResult.GetErrorMessage(errorMessage);

                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, enhancedMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string is not semantically equal to the expected JSON.
    /// This means that property order in objects and whitespace differences are ignored in the comparison,
    /// but array order is considered.
    /// </summary>
    /// <param name="actual">The actual JSON string to compare.</param>
    /// <param name="expected">The expected JSON string to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON strings are semantically equal or if either JSON is invalid.</exception>
    public static void ShouldNotBeSemanticallySameJson(this string? actual, string? expected, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON strings should not be semantically the same";

        if (actual is null && expected is null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
        }

        if (actual is null || expected is null)
        {
            return;
        }

        try
        {
            var actualNode = JsonNode.Parse(actual);
            var expectedNode = JsonNode.Parse(expected);

            if (actualNode == null && expectedNode == null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "Both JSON strings parsed to null").ToString());
            }

            if (actualNode == null || expectedNode == null)
            {
                return;
            }

            var comparisonResult = JsonComparisonEngine.CompareSemanticEquality(actualNode, expectedNode);

            if (comparisonResult.IsEqual)
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
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

            var comparisonResult = JsonComparisonEngine.CompareSubtree(actualNode, expectedNode);

            if (!comparisonResult.IsEqual)
            {
                var enhancedMessage = comparisonResult.GetErrorMessage(errorMessage);

                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, enhancedMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string is not a subtree of the expected JSON.
    /// A subtree means that all properties in the actual JSON must exist in the expected JSON with the same values,
    /// but the expected JSON can have additional properties.
    /// </summary>
    /// <param name="actual">The actual JSON string to check as a subtree.</param>
    /// <param name="expected">The expected JSON string to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the actual JSON is a subtree of the expected JSON or if either JSON is invalid.</exception>
    public static void ShouldNotBeJsonSubtreeOf(this string? actual, string? expected, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should not be a subtree of expected JSON";

        if (actual is null && expected is null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
        }

        if (actual is null || expected is null)
        {
            return;
        }

        try
        {
            var actualNode = JsonNode.Parse(actual);
            var expectedNode = JsonNode.Parse(expected);

            var comparisonResult = JsonComparisonEngine.CompareSubtree(actualNode, expectedNode);

            if (comparisonResult.IsEqual)
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
    /// Asserts that the JSON represented by the expected string is a subtree of the JSON represented by the actual string.
    /// A subtree means that all properties in the actual JSON must exist in the expected JSON with the same values,
    /// but the actual JSON can have additional properties.
    /// </summary>
    /// <param name="actual">The actual JSON string to compare against.</param>
    /// <param name="expected">The expected JSON string to check as a subtree.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the expected JSON is not a subtree of the actual JSON.</exception>
    public static void ShouldHaveJsonSubtree(this string? actual, string? expected, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should have expected JSON as subtree";

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

            var comparisonResult = JsonComparisonEngine.CompareSubtree(expectedNode, actualNode);

            if (!comparisonResult.IsEqual)
            {
                var enhancedMessage = comparisonResult.GetErrorMessage(errorMessage);

                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, enhancedMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON represented by the expected string is not a subtree of the JSON represented by the actual string.
    /// A subtree means that all properties in the expected JSON must exist in the actual JSON with the same values.
    /// </summary>
    /// <param name="actual">The actual JSON string to compare against.</param>
    /// <param name="expected">The expected JSON string to check as a subtree.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the expected JSON is a subtree of the actual JSON or if either JSON is invalid.</exception>
    public static void ShouldNotHaveJsonSubtree(this string? actual, string? expected, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should not have expected JSON as subtree";

        if (actual is null && expected is null)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
        }

        if (actual is null || expected is null)
        {
            return;
        }

        try
        {
            var actualNode = JsonNode.Parse(actual);
            var expectedNode = JsonNode.Parse(expected);

            var comparisonResult = JsonComparisonEngine.CompareSubtree(expectedNode, actualNode);

            if (comparisonResult.IsEqual)
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }
}