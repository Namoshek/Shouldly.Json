namespace Shouldly;

using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using Json.Pointer;

public static partial class ShouldlyJsonExtensions
{
    /// <summary>
    /// Asserts that the string value at the specified JSON Pointer (RFC 6901) path matches the given regular expression.
    /// </summary>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the string value.</param>
    /// <param name="regex">The regular expression pattern to match against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">
    /// Thrown if the value at the JSON Pointer path does not exist, is not a string, or does not match the regex pattern.
    /// </exception>
    public static void ShouldHaveJsonValueMatchingRegex(this string? actual, string jsonPointer, string regex, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should match regex pattern '{regex}'";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        Regex regexPattern;
        try
        {
            regexPattern = new Regex(regex);
        }
        catch (ArgumentException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: Invalid regex pattern - {ex.Message}").ToString());
        }

        try
        {
            var result = JsonHelper.TryGetValueAtPointer<string>(actual, jsonPointer, allowNull: true);

            if (!result.Success)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message).ToString());
            }

            var actualValue = result.Value;

            if (actualValue is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: Value at JSON pointer '{jsonPointer}' is null").ToString());
            }

            if (!regexPattern.IsMatch(actualValue))
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"match regex '{regex}'", actualValue, errorMessage).ToString());
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
    /// Asserts that the string value at the specified JSON Pointer (RFC 6901) path does not match the given regular expression.
    /// </summary>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the string value.</param>
    /// <param name="regex">The regular expression pattern to check against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">
    /// Thrown if the value at the JSON Pointer path does not exist, is not a string, or matches the regex pattern.
    /// </exception>
    public static void ShouldNotHaveJsonValueMatchingRegex(this string? actual, string jsonPointer, string regex, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should not match regex pattern '{regex}'";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        Regex regexPattern;
        try
        {
            regexPattern = new Regex(regex);
        }
        catch (ArgumentException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: Invalid regex pattern - {ex.Message}").ToString());
        }

        try
        {
            var result = JsonHelper.TryGetValueAtPointer<string>(actual, jsonPointer, allowNull: true);

            if (!result.Success)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message).ToString());
            }

            var actualValue = result.Value;

            if (actualValue is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: Value at JSON pointer '{jsonPointer}' is null").ToString());
            }

            if (regexPattern.IsMatch(actualValue))
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"not match regex '{regex}'", actualValue, errorMessage).ToString());
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