namespace Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.Pointer;

public static partial class ShouldlyJsonExtensions
{
    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has the expected number of elements.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="expectedCount">The expected number of elements in the array.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Can be in standard notation (/path/to/array) or URI Fragment Identifier notation (#/path/to/array). Defaults to the root pointer "".</param>
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
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path does not have the specified number of elements.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="unexpectedCount">The number of elements that the array should not have.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Can be in standard notation (/path/to/array) or URI Fragment Identifier notation (#/path/to/array). Defaults to the root pointer "".</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array has exactly the unexpected number of elements, or if the JSON is invalid.</exception>
    public static void ShouldNotHaveJsonArrayCount(this string? actual, int unexpectedCount, string jsonPointer = "", string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should not have {unexpectedCount} elements";

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

            if (actualCount == unexpectedCount)
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"not {unexpectedCount}", actualCount, errorMessage).ToString());
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
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path contains an element semantically equal to the given JSON.
    /// </summary>
    /// <remarks>
    /// Elements are compared with the same semantics as <see cref="ShouldBeSemanticallySameJson(string?,string?,string?)"/>:
    /// property order is ignored, array order is significant, and <c>42</c> equals <c>42.0</c>.
    /// </remarks>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array.</param>
    /// <param name="expectedElement">The JSON that the array must contain.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or does not contain the element.</exception>
    public static void ShouldContainJsonElement(this string? actual, string jsonPointer, string? expectedElement, string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should contain the element '{expectedElement}'";

        var expectedNode = ParseExpectedElement(expectedElement, errorMessage);

        for (var i = 0; i < array.Count; i++)
        {
            if (JsonComparisonEngine.CompareSemanticEquality(array[i], expectedNode).IsEqual)
            {
                return;
            }
        }

        throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedElement, array.ToJsonString(), errorMessage).ToString());
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path contains no element semantically equal to the given JSON.
    /// </summary>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array.</param>
    /// <param name="unexpectedElement">The JSON that the array must not contain.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or contains the element.</exception>
    public static void ShouldNotContainJsonElement(this string? actual, string jsonPointer, string? unexpectedElement, string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should not contain the element '{unexpectedElement}'";

        var unexpectedNode = ParseExpectedElement(unexpectedElement, errorMessage);

        var matchingIndices = new List<int>();
        for (var i = 0; i < array.Count; i++)
        {
            if (JsonComparisonEngine.CompareSemanticEquality(array[i], unexpectedNode).IsEqual)
            {
                matchingIndices.Add(i);
            }
        }

        if (matchingIndices.Count > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"not '{unexpectedElement}'",
                $"matching indices: {string.Join(", ", matchingIndices)}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has no elements.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no array at the path or it has elements.</exception>
    public static void ShouldBeJsonArrayEmpty(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should be empty";

        if (array.Count != 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(0, array.Count, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has at least one element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no array at the path or it is empty.</exception>
    public static void ShouldBeJsonArrayNotEmpty(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should have at least one element";

        if (array.Count == 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage("at least 1 element", 0, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has at least one element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no array at the path or it is empty.</exception>
    public static void ShouldNotBeJsonArrayEmpty(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        ShouldBeJsonArrayNotEmpty(actual, jsonPointer, customMessage ?? $"JSON array at pointer '{jsonPointer}' should not be empty");
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has a number of elements within the given bounds (inclusive).
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="minCount">The minimum allowed number of elements.</param>
    /// <param name="maxCount">The maximum allowed number of elements.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or has a count outside the range.</exception>
    public static void ShouldHaveJsonArrayCountBetween(this string? actual, int minCount, int maxCount, string jsonPointer = "", string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should have between {minCount} and {maxCount} elements";

        if (array.Count < minCount || array.Count > maxCount)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"between {minCount} and {maxCount}", array.Count, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path has a number of elements outside the given bounds (inclusive).
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="minCount">The minimum value of the disallowed range.</param>
    /// <param name="maxCount">The maximum value of the disallowed range.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or has a count within the disallowed range.</exception>
    public static void ShouldNotHaveJsonArrayCountBetween(this string? actual, int minCount, int maxCount, string jsonPointer = "", string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should not have between {minCount} and {maxCount} elements";

        if (array.Count >= minCount && array.Count <= maxCount)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"not between {minCount} and {maxCount}", array.Count, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path contains no duplicate elements.
    /// </summary>
    /// <remarks>
    /// Elements are compared with semantic JSON equality, so <c>42</c> and <c>42.0</c> count as duplicates.
    /// </remarks>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or contains duplicates.</exception>
    public static void ShouldHaveJsonArrayUniqueElements(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should have unique elements";

        var duplicateIndices = FindDuplicateIndices(array);

        if (duplicateIndices.Count > 0)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual,
                $"{errorMessage} (duplicate elements at indices: {string.Join(", ", duplicateIndices)})").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON array at the specified JSON Pointer (RFC 6901) path contains at least one duplicate element.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array. Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the array does not exist or all elements are unique.</exception>
    public static void ShouldNotHaveJsonArrayUniqueElements(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        var array = GetArrayAtPointer(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON array at pointer '{jsonPointer}' should have duplicate elements";

        if (FindDuplicateIndices(array).Count == 0)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that at least one element of the JSON array carries the property at the given property pointer path.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within an array element.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if no element has the property.</exception>
    public static void ShouldAnyHaveJsonProperty(this string? actual, string arrayPointer, string propertyPointer, string? customMessage = null)
    {
        var (array, propPointer) = ParseArrayAndPropertyPointers(ParseRoot(actual), arrayPointer, propertyPointer, actual);
        var errorMessage = customMessage ?? $"At least one element in JSON array at pointer '{arrayPointer}' should have a property at pointer '{propertyPointer}'";

        for (var i = 0; i < array.Count; i++)
        {
            var element = array[i];
            if (element is not null && propPointer.TryEvaluate(element, out _))
            {
                return;
            }
        }

        throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage} (found at no index)").ToString());
    }

    /// <summary>
    /// Asserts that no element of the JSON array carries the property at the given property pointer path.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within an array element.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element has the property.</exception>
    public static void ShouldNotAnyHaveJsonProperty(this string? actual, string arrayPointer, string propertyPointer, string? customMessage = null)
    {
        var (array, propPointer) = ParseArrayAndPropertyPointers(ParseRoot(actual), arrayPointer, propertyPointer, actual);
        var errorMessage = customMessage ?? $"No element in JSON array at pointer '{arrayPointer}' should have a property at pointer '{propertyPointer}'";

        var matchingIndices = new List<int>();
        for (var i = 0; i < array.Count; i++)
        {
            var element = array[i];
            if (element is not null && propPointer.TryEvaluate(element, out _))
            {
                matchingIndices.Add(i);
            }
        }

        if (matchingIndices.Count > 0)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual,
                $"{errorMessage} (found at indices: {string.Join(", ", matchingIndices)})").ToString());
        }
    }

    /// <summary>
    /// Asserts that at least one element of the JSON array has the expected value at the given property pointer path.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within an array element.</param>
    /// <param name="expectedValue">The value that at least one element must have.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if no element has the expected value.</exception>
    public static void ShouldAnyHaveJsonValue<T>(
        this string? actual,
        string arrayPointer,
        string propertyPointer,
        T expectedValue,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var (array, propPointer) = ParseArrayAndPropertyPointers(ParseRoot(actual), arrayPointer, propertyPointer, actual);
        var errorMessage = customMessage ?? $"At least one element in JSON array at pointer '{arrayPointer}' should have value '{expectedValue}' at property '{propertyPointer}'";

        var observed = new List<string>();
        for (var i = 0; i < array.Count; i++)
        {
            var element = array[i];
            if (element is null || !propPointer.TryEvaluate(element, out var result))
            {
                observed.Add($"index {i}: (missing)");
                continue;
            }

            object? actualValue;
            try
            {
                actualValue = result is null ? null : result.Deserialize<T>();
            }
            catch (JsonException)
            {
                observed.Add($"index {i}: (type mismatch)");
                continue;
            }

            var areEqual = comparer != null
                ? comparer.Equals((T?)actualValue, expectedValue)
                : EqualityComparer<T>.Default.Equals((T?)actualValue!, expectedValue);

            if (areEqual)
            {
                return;
            }

            observed.Add($"index {i}: '{actualValue}'");
        }

        throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
            expectedValue,
            $"observed values at {string.Join(", ", observed)}",
            errorMessage).ToString());
    }

    /// <summary>
    /// Asserts that no element of the JSON array has the unexpected value at the given property pointer path.
    /// </summary>
    /// <typeparam name="T">The type of the unexpected value.</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within an array element.</param>
    /// <param name="unexpectedValue">The value that no element may have.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element has the unexpected value.</exception>
    public static void ShouldAllNotHaveJsonValue<T>(
        this string? actual,
        string arrayPointer,
        string propertyPointer,
        T unexpectedValue,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var (array, propPointer) = ParseArrayAndPropertyPointers(ParseRoot(actual), arrayPointer, propertyPointer, actual);
        var errorMessage = customMessage ?? $"No element in JSON array at pointer '{arrayPointer}' should have value '{unexpectedValue}' at property '{propertyPointer}'";

        var failures = new List<string>();
        for (var i = 0; i < array.Count; i++)
        {
            var element = array[i];
            if (element is null || !propPointer.TryEvaluate(element, out var result))
            {
                continue;
            }

            object? actualValue;
            try
            {
                actualValue = result is null ? null : result.Deserialize<T>();
            }
            catch (JsonException)
            {
                continue;
            }

            var areEqual = comparer != null
                ? comparer.Equals((T?)actualValue, unexpectedValue)
                : EqualityComparer<T>.Default.Equals((T?)actualValue!, unexpectedValue);

            if (areEqual)
            {
                failures.Add($"index {i}: '{actualValue}'");
            }
        }

        if (failures.Count > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"not '{unexpectedValue}'",
                $"matching values at {string.Join(", ", failures)}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that at least one string value of the JSON array matches the given regular expression.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the string property within an array element.</param>
    /// <param name="regex">The regular expression that at least one element must match.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if no element matches the pattern.</exception>
    public static void ShouldAnyHaveJsonValueMatchingRegex(this string? actual, string arrayPointer, string propertyPointer, string regex, string? customMessage = null)
    {
        var regexPattern = CompileRegex(actual, regex, customMessage ?? $"At least one element in JSON array at pointer '{arrayPointer}' should have value at '{propertyPointer}' matching regex pattern '{regex}'");
        var (array, propPointer) = ParseArrayAndPropertyPointers(ParseRoot(actual), arrayPointer, propertyPointer, actual);
        var errorMessage = customMessage ?? $"At least one element in JSON array at pointer '{arrayPointer}' should have value at '{propertyPointer}' matching regex pattern '{regex}'";

        var observed = new List<string>();
        for (var i = 0; i < array.Count; i++)
        {
            var element = array[i];
            if (element is null || !propPointer.TryEvaluate(element, out var result))
            {
                observed.Add($"index {i}: (missing)");
                continue;
            }

            string? actualValue;
            try
            {
                actualValue = result?.Deserialize<string>();
            }
            catch (JsonException)
            {
                observed.Add($"index {i}: (not a string)");
                continue;
            }

            if (actualValue is null)
            {
                observed.Add($"index {i}: (null)");
                continue;
            }

            if (regexPattern.IsMatch(actualValue))
            {
                return;
            }

            observed.Add($"index {i}: '{actualValue}'");
        }

        throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
            $"match regex '{regex}' at some index",
            $"observed values at {string.Join(", ", observed)}",
            errorMessage).ToString());
    }

    /// <summary>
    /// Asserts that no string value of the JSON array matches the given regular expression.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the string property within an array element.</param>
    /// <param name="regex">The regular expression that no element may match.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element matches the pattern.</exception>
    public static void ShouldAnyNotHaveJsonValueMatchingRegex(this string? actual, string arrayPointer, string propertyPointer, string regex, string? customMessage = null)
    {
        var regexPattern = CompileRegex(actual, regex, customMessage ?? $"No element in JSON array at pointer '{arrayPointer}' should have value at '{propertyPointer}' matching regex pattern '{regex}'");
        var (array, propPointer) = ParseArrayAndPropertyPointers(ParseRoot(actual), arrayPointer, propertyPointer, actual);
        var errorMessage = customMessage ?? $"No element in JSON array at pointer '{arrayPointer}' should have value at '{propertyPointer}' matching regex pattern '{regex}'";

        var failures = new List<string>();
        for (var i = 0; i < array.Count; i++)
        {
            var element = array[i];
            if (element is null || !propPointer.TryEvaluate(element, out var result))
            {
                continue;
            }

            string? actualValue;
            try
            {
                actualValue = result?.Deserialize<string>();
            }
            catch (JsonException)
            {
                continue;
            }

            if (actualValue is null)
            {
                continue;
            }

            if (regexPattern.IsMatch(actualValue))
            {
                failures.Add($"index {i}: '{actualValue}'");
            }
        }

        if (failures.Count > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"not match regex '{regex}'",
                $"matching values at {string.Join(", ", failures)}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that no numeric value of the JSON array is within the given inclusive range.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within an array element.</param>
    /// <param name="min">The minimum value of the disallowed range.</param>
    /// <param name="max">The maximum value of the disallowed range.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value is within the disallowed range.</exception>
    public static void ShouldAllNotHaveJsonValueBetween<T>(this string? actual, string arrayPointer, string propertyPointer, T min, T max, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"No element in JSON array at '{arrayPointer}' should have value at '{propertyPointer}' between {min} and {max}";

        var failedItems = values.Where(x => x.Value.CompareTo(min) >= 0 && x.Value.CompareTo(max) <= 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);

        if (failedItems.Count > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"not between {min} and {max}",
                $"matching values: {string.Join(", ", failedItems)}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that no date/time value of the JSON array is within the given start (inclusive) to end (exclusive) range.
    /// </summary>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within an array element.</param>
    /// <param name="start">The inclusive start of the disallowed range.</param>
    /// <param name="end">The exclusive end of the disallowed range.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element date/time is within the disallowed range.</exception>
    public static void ShouldAllNotHaveJsonDateBetween<T>(this string? actual, string arrayPointer, string propertyPointer, T start, T end, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"No element in JSON array at '{arrayPointer}' should have date at '{propertyPointer}' between {start} (inclusive) and {end} (exclusive)";

        var failedItems = values.Where(x => x.Value.CompareTo(start) >= 0 && x.Value.CompareTo(end) < 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);

        if (failedItems.Count > 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"not between {start} (inclusive) and {end} (exclusive)",
                $"matching values: {string.Join(", ", failedItems)}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all elements in the JSON array at the specified JSON Pointer (RFC 6901) path have a property
    /// at the given property pointer path.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element in the array does not have the property at the given path.</exception>
    public static void ShouldAllHaveJsonProperty(this string? actual, string arrayPointer, string propertyPointer, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"All elements in JSON array at pointer '{arrayPointer}' should have a property at pointer '{propertyPointer}'";

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

            var (array, propPointer) = ParseArrayAndPropertyPointers(jsonNode, arrayPointer, propertyPointer, actual);

            var failedIndices = new List<int>();
            for (var i = 0; i < array.Count; i++)
            {
                var element = array[i];
                if (element is null || !propPointer.TryEvaluate(element, out _))
                {
                    failedIndices.Add(i);
                }
            }

            if (failedIndices.Count > 0)
            {
                var indicesStr = string.Join(", ", failedIndices);
                throw new ShouldAssertException(new ActualShouldlyMessage(actual,
                    $"{errorMessage} (failed at indices: {indicesStr})").ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that no element in the JSON array at the specified JSON Pointer (RFC 6901) path has a property
    /// at the given property pointer path.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element in the array has the property at the given path.</exception>
    public static void ShouldAllNotHaveJsonProperty(this string? actual, string arrayPointer, string propertyPointer, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"All elements in JSON array at pointer '{arrayPointer}' should not have a property at pointer '{propertyPointer}'";

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

            var (array, propPointer) = ParseArrayAndPropertyPointers(jsonNode, arrayPointer, propertyPointer, actual);

            var failedIndices = new List<int>();
            for (var i = 0; i < array.Count; i++)
            {
                var element = array[i];
                if (element is not null && propPointer.TryEvaluate(element, out _))
                {
                    failedIndices.Add(i);
                }
            }

            if (failedIndices.Count > 0)
            {
                var indicesStr = string.Join(", ", failedIndices);
                throw new ShouldAssertException(new ActualShouldlyMessage(actual,
                    $"{errorMessage} (failed at indices: {indicesStr})").ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that all elements in the JSON array at the specified JSON Pointer (RFC 6901) path have a property
    /// at the given property pointer that matches the expected value.
    /// </summary>
    /// <typeparam name="T">The type of the expected value.</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="expectedValue">The expected value to compare against.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element in the array does not have the expected value at the given property path.</exception>
    public static void ShouldAllHaveJsonValue<T>(
        this string? actual,
        string arrayPointer,
        string propertyPointer,
        T expectedValue,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"All elements in JSON array at pointer '{arrayPointer}' should have value '{expectedValue}' at property '{propertyPointer}'";

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

            var (array, propPointer) = ParseArrayAndPropertyPointers(jsonNode, arrayPointer, propertyPointer, actual);

            var failures = new List<string>();
            for (var i = 0; i < array.Count; i++)
            {
                var element = array[i];
                if (element is null || !propPointer.TryEvaluate(element, out var result))
                {
                    failures.Add($"index {i}: (missing)");
                    continue;
                }

                object? actualValue;
                try
                {
                    if (result is null)
                    {
                        actualValue = null;
                    }
                    else
                    {
                        actualValue = result.Deserialize<T>();
                    }
                }
                catch (JsonException)
                {
                    failures.Add($"index {i}: (type mismatch)");
                    continue;
                }

                var areEqual = comparer != null
                    ? comparer.Equals((T?)actualValue, expectedValue)
                    : EqualityComparer<T>.Default.Equals((T?)actualValue!, expectedValue);

                if (!areEqual)
                {
                    failures.Add($"index {i}: '{actualValue}'");
                }
            }

            if (failures.Count > 0)
            {
                var failureDesc = string.Join(", ", failures);
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                    expectedValue,
                    $"differing values at {failureDesc}",
                    errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that all numeric values in the JSON array at the specified JSON Pointer (RFC 6901) path are less than the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value is not less than the given value.</exception>
    public static void ShouldAllHaveJsonValueLessThan<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have value at '{propertyPointer}' less than {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) >= 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"less than {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all numeric values in the JSON array at the specified JSON Pointer (RFC 6901) path are less than or equal to the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value is not less than or equal to the given value.</exception>
    public static void ShouldAllHaveJsonValueLessThanOrEqualTo<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have value at '{propertyPointer}' less than or equal to {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) > 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"less than or equal to {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all numeric values in the JSON array at the specified JSON Pointer (RFC 6901) path are greater than the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value is not greater than the given value.</exception>
    public static void ShouldAllHaveJsonValueGreaterThan<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have value at '{propertyPointer}' greater than {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) <= 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"greater than {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all numeric values in the JSON array at the specified JSON Pointer (RFC 6901) path are greater than or equal to the given value.
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value is not greater than or equal to the given value.</exception>
    public static void ShouldAllHaveJsonValueGreaterThanOrEqualTo<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have value at '{propertyPointer}' greater than or equal to {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) < 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"greater than or equal to {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all numeric values in the JSON array at the specified JSON Pointer (RFC 6901) path are between the given minimum and maximum values (inclusive).
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="min">The minimum value to compare against.</param>
    /// <param name="max">The maximum value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value is not between the given minimum and maximum values.</exception>
    public static void ShouldAllHaveJsonValueBetween<T>(this string? actual, string arrayPointer, string propertyPointer, T min, T max, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have value at '{propertyPointer}' between {min} and {max}";

        var failedItems = values.Where(x => x.Value.CompareTo(min) < 0 || x.Value.CompareTo(max) > 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"between {min} and {max} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all date/time values in the JSON array at the specified JSON Pointer (RFC 6901) path are before the given value.
    /// </summary>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element date/time is not before the given value.</exception>
    public static void ShouldAllHaveJsonDateBefore<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have date/time at '{propertyPointer}' before {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) >= 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"before {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all date/time values in the JSON array at the specified JSON Pointer (RFC 6901) path are before or equal to the given value.
    /// </summary>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element date/time is not before or equal to the given value.</exception>
    public static void ShouldAllHaveJsonDateBeforeOrEqualTo<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have date/time at '{propertyPointer}' before or equal to {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) > 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"before or equal to {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all date/time values in the JSON array at the specified JSON Pointer (RFC 6901) path are after the given value.
    /// </summary>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element date/time is not after the given value.</exception>
    public static void ShouldAllHaveJsonDateAfter<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have date/time at '{propertyPointer}' after {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) <= 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"after {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all date/time values in the JSON array at the specified JSON Pointer (RFC 6901) path are after or equal to the given value.
    /// </summary>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="value">The value to compare against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element date/time is not after or equal to the given value.</exception>
    public static void ShouldAllHaveJsonDateAfterOrEqualTo<T>(this string? actual, string arrayPointer, string propertyPointer, T value, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have date/time at '{propertyPointer}' after or equal to {value}";

        var failedItems = values.Where(x => x.Value.CompareTo(value) < 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"after or equal to {value} at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all date/time values in the JSON array at the specified JSON Pointer (RFC 6901) path are between the given start (inclusive) and end (exclusive) values.
    /// </summary>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="start">The inclusive start value of the range.</param>
    /// <param name="end">The exclusive end value of the range.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element date/time is not within the specified range.</exception>
    public static void ShouldAllHaveJsonDateBetween<T>(this string? actual, string arrayPointer, string propertyPointer, T start, T end, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var (values, extractionFailures) = GetAllStructValuesFromArrayElements<T>(actual, arrayPointer, propertyPointer);
        var errorMessage = customMessage ?? $"All elements in JSON array at '{arrayPointer}' should have date at '{propertyPointer}' between {start} (inclusive) and {end} (exclusive)";

        var failedItems = values.Where(x => x.Value.CompareTo(start) < 0 || x.Value.CompareTo(end) >= 0).Select(f => $"index {f.Index}: {f.Value}").ToList();
        failedItems.AddRange(extractionFailures);
        if (failedItems.Count > 0)
        {
            var failureDesc = string.Join(", ", failedItems);
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                $"between {start} (inclusive) and {end} (exclusive) at all indices",
                $"failing values: {failureDesc}",
                errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that all string values in the JSON array at the specified JSON Pointer (RFC 6901) path match the given regular expression.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the string property within each array element.</param>
    /// <param name="regex">The regular expression pattern to match against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value does not match the regex pattern.</exception>
    public static void ShouldAllHaveJsonValueMatchingRegex(this string? actual, string arrayPointer, string propertyPointer, string regex, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"All elements in JSON array at pointer '{arrayPointer}' should have value at '{propertyPointer}' matching regex pattern '{regex}'";

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
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null").ToString());
            }

            var (array, propPointer) = ParseArrayAndPropertyPointers(jsonNode, arrayPointer, propertyPointer, actual);

            var failures = new List<string>();
            for (var i = 0; i < array.Count; i++)
            {
                var element = array[i];
                if (element is null || !propPointer.TryEvaluate(element, out var result))
                {
                    failures.Add($"index {i}: (missing)");
                    continue;
                }

                string? actualValue;
                try
                {
                    actualValue = result?.Deserialize<string>();
                }
                catch (JsonException)
                {
                    failures.Add($"index {i}: (not a string)");
                    continue;
                }

                if (actualValue is null)
                {
                    failures.Add($"index {i}: (null)");
                    continue;
                }

                if (!regexPattern.IsMatch(actualValue))
                {
                    failures.Add($"index {i}: '{actualValue}'");
                }
            }

            if (failures.Count > 0)
            {
                var failureDesc = string.Join(", ", failures);
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                    $"match regex '{regex}' at all indices",
                    $"failing values: {failureDesc}",
                    errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that no string value in the JSON array at the specified JSON Pointer (RFC 6901) path matches the given regular expression.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the string property within each array element.</param>
    /// <param name="regex">The regular expression pattern to check against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if any element value matches the regex pattern.</exception>
    public static void ShouldAllNotHaveJsonValueMatchingRegex(this string? actual, string arrayPointer, string propertyPointer, string regex, string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"All elements in JSON array at pointer '{arrayPointer}' should not have value at '{propertyPointer}' matching regex pattern '{regex}'";

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
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null").ToString());
            }

            var (array, propPointer) = ParseArrayAndPropertyPointers(jsonNode, arrayPointer, propertyPointer, actual);

            var failures = new List<string>();
            for (var i = 0; i < array.Count; i++)
            {
                var element = array[i];
                if (element is null || !propPointer.TryEvaluate(element, out var result))
                {
                    failures.Add($"index {i}: (missing)");
                    continue;
                }

                string? actualValue;
                try
                {
                    actualValue = result?.Deserialize<string>();
                }
                catch (JsonException)
                {
                    failures.Add($"index {i}: (not a string)");
                    continue;
                }

                if (actualValue is null)
                {
                    failures.Add($"index {i}: (null)");
                    continue;
                }

                if (regexPattern.IsMatch(actualValue))
                {
                    failures.Add($"index {i}: '{actualValue}'");
                }
            }

            if (failures.Count > 0)
            {
                var failureDesc = string.Join(", ", failures);
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(
                    $"not match regex '{regex}' at all indices",
                    $"matching values: {failureDesc}",
                    errorMessage).ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Parses the JSON string, asserting basic validity, and returns the root node.
    /// </summary>
    /// <param name="actual">The JSON string to parse.</param>
    /// <param name="shouldlyMethod">Name of the calling assertion, captured automatically.</param>
    /// <returns>The parsed root JSON node.</returns>
    /// <exception cref="ShouldAssertException">Thrown if the JSON string is null or invalid.</exception>
    private static JsonNode ParseRoot(string? actual, [CallerMemberName] string shouldlyMethod = null!)
    {
        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null", shouldlyMethod).ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null", shouldlyMethod).ToString());
            }

            return jsonNode;
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}", shouldlyMethod).ToString());
        }
    }

    /// <summary>
    /// Evaluates a pointer and returns the JSON array found there.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the array.</param>
    /// <param name="customMessage">An optional custom message to include in the exception.</param>
    /// <param name="shouldlyMethod">Name of the calling assertion, captured automatically.</param>
    /// <returns>The JSON array at the pointer.</returns>
    /// <exception cref="ShouldAssertException">Thrown if the JSON is invalid, the pointer resolves to nothing, or the value is not an array.</exception>
    private static JsonArray GetArrayAtPointer(string? actual, string jsonPointer, string? customMessage, [CallerMemberName] string shouldlyMethod = null!)
    {
        var result = JsonHelper.TryGetNodeAtPointer(actual, jsonPointer);

        if (!result.Success)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message, shouldlyMethod).ToString());
        }

        if (result.Value?.GetValueKind() != JsonValueKind.Array)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Value at JSON pointer '{jsonPointer}' is not an array", shouldlyMethod).ToString());
        }

        return result.Value.AsArray();
    }

    /// <summary>
    /// Parses the element JSON used by containment assertions.
    /// </summary>
    /// <param name="expected">The expected element JSON, which may be null.</param>
    /// <param name="errorMessage">The message prefix to use when the expected JSON is invalid.</param>
    /// <returns>The parsed expected node, or null when the expected JSON is null.</returns>
    /// <exception cref="ShouldAssertException">Thrown if the expected JSON is invalid.</exception>
    private static JsonNode? ParseExpectedElement(string? expected, string errorMessage)
    {
        if (expected is null)
        {
            return null;
        }

        try
        {
            return JsonNode.Parse(expected);
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException($"{errorMessage}: Invalid JSON: {ex.Message}");
        }
    }

    /// <summary>
    /// Compiles a regular expression, reporting invalid patterns as assertion failures.
    /// </summary>
    /// <param name="actual">The JSON string, used for error messages.</param>
    /// <param name="regex">The regular expression pattern.</param>
    /// <param name="errorMessage">The message prefix to use when the pattern is invalid.</param>
    /// <returns>The compiled regular expression.</returns>
    /// <exception cref="ShouldAssertException">Thrown if the pattern is invalid.</exception>
    private static Regex CompileRegex(string? actual, string regex, string errorMessage)
    {
        try
        {
            return new Regex(regex);
        }
        catch (ArgumentException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}: Invalid regex pattern - {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Finds the indices of elements that duplicate an earlier element, using semantic JSON equality.
    /// </summary>
    /// <param name="array">The array to scan.</param>
    /// <returns>The indices of the duplicate elements.</returns>
    private static List<int> FindDuplicateIndices(JsonArray array)
    {
        var duplicates = new List<int>();
        for (var i = 1; i < array.Count; i++)
        {
            for (var j = 0; j < i; j++)
            {
                if (JsonComparisonEngine.CompareSemanticEquality(array[i], array[j]).IsEqual)
                {
                    duplicates.Add(i);
                    break;
                }
            }
        }

        return duplicates;
    }

    /// <summary>
    /// Parses and validates both the array pointer and property pointer, returning the evaluated array and the parsed property pointer.
    /// </summary>
    /// <param name="rootNode">The root JSON node to evaluate against.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="actual">The original JSON string, used for error messages.</param>
    /// <param name="shouldlyMethod">Name of the calling assertion, captured automatically.</param>
    /// <returns>A tuple containing the JSON array and the parsed property pointer.</returns>
    private static (JsonArray Array, JsonPointer PropertyPointer) ParseArrayAndPropertyPointers(
        JsonNode rootNode,
        string arrayPointer,
        string propertyPointer,
        string? actual,
        [CallerMemberName] string shouldlyMethod = null!)
    {
        JsonPointer arrayPtr;
        try
        {
            arrayPtr = JsonPointer.Parse(arrayPointer);
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer '{arrayPointer}': {ex.Message}", shouldlyMethod).ToString());
        }

        if (!arrayPtr.TryEvaluate(rootNode, out var arrayResult))
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual,
                $"No value found at JSON pointer '{arrayPointer}'", shouldlyMethod).ToString());
        }

        if (arrayResult?.GetValueKind() != JsonValueKind.Array)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual,
                $"Value at JSON pointer '{arrayPointer}' is not an array", shouldlyMethod).ToString());
        }

        JsonPointer propPointer;
        try
        {
            propPointer = JsonPointer.Parse(propertyPointer);
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer '{propertyPointer}': {ex.Message}", shouldlyMethod).ToString());
        }

        return (arrayResult.AsArray(), propPointer);
    }

    /// <summary>
    /// Gets all struct-typed values at the given property pointer from every element in the array at the given array pointer.
    /// Elements that are missing the property or cannot be converted are reported instead of aborting the collection.
    /// </summary>
    /// <typeparam name="T">The struct type to deserialize each value to.</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="arrayPointer">The JSON Pointer path to the array.</param>
    /// <param name="propertyPointer">The JSON Pointer path to the property within each array element.</param>
    /// <param name="shouldlyMethod">Name of the calling assertion, captured automatically.</param>
    /// <returns>A list of (index, value) pairs plus a list of per-index extraction failure descriptions.</returns>
    private static (List<(int Index, T Value)> Values, List<string> Failures) GetAllStructValuesFromArrayElements<T>(
        string? actual,
        string arrayPointer,
        string propertyPointer,
        [CallerMemberName] string shouldlyMethod = null!) where T : struct
    {
        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null", shouldlyMethod).ToString());
        }

        try
        {
            var jsonNode = JsonNode.Parse(actual);
            if (jsonNode is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string parsed to null", shouldlyMethod).ToString());
            }

            var (array, propPointer) = ParseArrayAndPropertyPointers(jsonNode, arrayPointer, propertyPointer, actual);

            var values = new List<(int Index, T Value)>();
            var failures = new List<string>();
            for (var i = 0; i < array.Count; i++)
            {
                var element = array[i];
                if (element is null || !propPointer.TryEvaluate(element, out var result))
                {
                    failures.Add($"index {i}: (missing)");
                    continue;
                }

                if (result is null)
                {
                    failures.Add($"index {i}: (null)");
                    continue;
                }

                try
                {
                    values.Add((i, result.Deserialize<T>()));
                }
                catch (JsonException)
                {
                    failures.Add($"index {i}: (not a valid {typeof(T).Name})");
                }
            }

            return (values, failures);
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}", shouldlyMethod).ToString());
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON pointer: {ex.Message}", shouldlyMethod).ToString());
        }
    }
}
