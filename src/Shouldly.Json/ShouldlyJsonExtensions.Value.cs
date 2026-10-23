namespace Shouldly;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Json.Pointer;

public static partial class ShouldlyJsonExtensions
{
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
    public static void ShouldHaveJsonValue<T>(
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
            var result = JsonHelper.TryGetValueAtPointer<T>(actual, jsonPointer, allowNull: true);

            if (!result.Success)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message).ToString());
            }

            var actualValue = result.Value;

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
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path does not match the unexpected value.
    /// </summary>
    /// <remarks>
    /// A path that does not exist satisfies this assertion; use <see cref="ShouldHaveJsonProperty(string?,string,string?)"/>
    /// when the property must exist.
    /// </remarks>
    /// <typeparam name="T">The type of the unexpected value.</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="unexpectedValue">The value the JSON must not contain at the path.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the path matches the unexpected value.</exception>
    public static void ShouldNotHaveJsonValue<T>(
        this string? actual,
        string jsonPointer,
        T unexpectedValue,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var errorMessage = customMessage ?? $"JSON should not have value '{unexpectedValue}' at pointer '{jsonPointer}'";

        var result = JsonHelper.TryGetValueAtPointer<T>(actual, jsonPointer, allowNull: true);

        if (!result.Success)
        {
            if (result.Failure is JsonPointerFailure.NotFound or JsonPointerFailure.EmptyJson)
            {
                return;
            }

            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message).ToString());
        }

        var actualValue = result.Value;

        var areEqual = comparer != null
            ? comparer.Equals(actualValue, unexpectedValue)
            : EqualityComparer<T>.Default.Equals(actualValue!, unexpectedValue!);

        if (areEqual)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"not '{unexpectedValue}'", actualValue, errorMessage).ToString());
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
    /// Asserts that the numeric value at the specified JSON Pointer (RFC 6901) path is not between the given minimum and maximum values (inclusive).
    /// </summary>
    /// <typeparam name="T">The numeric type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="min">The minimum value of the disallowed range.</param>
    /// <param name="max">The maximum value of the disallowed range.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the path is within the disallowed range.</exception>
    public static void ShouldNotHaveJsonValueBetween<T>(this string? actual, string jsonPointer, T min, T max, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should not be between {min} and {max}";

        if (actualValue.CompareTo(min) >= 0 && actualValue.CompareTo(max) <= 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"not between {min} and {max}", actualValue, errorMessage).ToString());
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
    /// Asserts that the date/time value at the specified JSON Pointer (RFC 6901) path is not between the given start (inclusive) and end (exclusive) values.
    /// </summary>
    /// <remarks>
    /// This method mirrors <see cref="ShouldHaveJsonDateBetween{T}(string?, string, T, T, string?)"/>
    /// and therefore also uses an exclusive upper bound.
    /// </remarks>
    /// <typeparam name="T">The date/time type to compare (must implement IComparable{T}).</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="start">The inclusive start of the disallowed range.</param>
    /// <param name="end">The exclusive end of the disallowed range.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the path is within the disallowed range.</exception>
    public static void ShouldNotHaveJsonDateBetween<T>(this string? actual, string jsonPointer, T start, T end, string? customMessage = null)
        where T : struct, IComparable<T>
    {
        var actualValue = GetAndValidateJsonValue<T>(actual, jsonPointer, customMessage);
        var errorMessage = customMessage ?? $"JSON date at pointer '{jsonPointer}' should not be between {start} (inclusive) and {end} (exclusive)";

        if (actualValue.CompareTo(start) >= 0 && actualValue.CompareTo(end) < 0)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"not between {start} (inclusive) and {end} (exclusive)", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path equals one of the given values.
    /// </summary>
    /// <typeparam name="T">The type of the candidate values.</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="values">The candidate values, at least one of which must match.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the path matches none of the given values.</exception>
    public static void ShouldHaveJsonValueOneOf<T>(
        this string? actual,
        string jsonPointer,
        IReadOnlyCollection<T> values,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var result = JsonHelper.TryGetValueAtPointer<T>(actual, jsonPointer, allowNull: true);

        if (!result.Success)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message).ToString());
        }

        var expectedDesc = DescribeValues(values);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be one of {expectedDesc}";

        var actualValue = result.Value;

        if (!MatchesAny(actualValue, values, comparer))
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expectedDesc, actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path equals none of the given values.
    /// </summary>
    /// <remarks>
    /// A path that does not exist satisfies this assertion.
    /// </remarks>
    /// <typeparam name="T">The type of the candidate values.</typeparam>
    /// <param name="actual">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="values">The candidate values, none of which may match.</param>
    /// <param name="comparer">An optional equality comparer to use for the comparison.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the value at the path matches one of the given values.</exception>
    public static void ShouldNotHaveJsonValueOneOf<T>(
        this string? actual,
        string jsonPointer,
        IReadOnlyCollection<T> values,
        IEqualityComparer<T>? comparer = null,
        string? customMessage = null)
    {
        var result = JsonHelper.TryGetValueAtPointer<T>(actual, jsonPointer, allowNull: true);

        if (!result.Success)
        {
            if (result.Failure is JsonPointerFailure.NotFound or JsonPointerFailure.EmptyJson)
            {
                return;
            }

            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message).ToString());
        }

        var expectedDesc = DescribeValues(values);
        var errorMessage = customMessage ?? $"JSON value at pointer '{jsonPointer}' should be none of {expectedDesc}";

        var actualValue = result.Value;

        if (MatchesAny(actualValue, values, comparer))
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage($"none of {expectedDesc}", actualValue, errorMessage).ToString());
        }
    }

    /// <summary>
    /// Gets and validates a JSON value at the specified pointer path.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value to.</typeparam>
    /// <param name="json">The JSON string to search.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="customMessage">An optional custom message to include in the exception.</param>
    /// <param name="shouldlyMethod">Name of the calling assertion, captured automatically.</param>
    /// <returns>The value at the specified pointer path.</returns>
    /// <exception cref="ShouldAssertException">
    /// Thrown when:
    /// - The input JSON is null
    /// - The JSON is invalid
    /// - The pointer is invalid
    /// - The value cannot be found at the specified pointer
    /// - The value cannot be converted to the specified type
    /// </exception>
    private static T GetAndValidateJsonValue<T>(string? json, string jsonPointer, string? customMessage, [CallerMemberName] string shouldlyMethod = null!) where T : struct
    {
        var result = JsonHelper.TryGetValueAtPointer<T>(json, jsonPointer);

        if (!result.Success)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(json, customMessage ?? result.Message, shouldlyMethod).ToString());
        }

        return result.Value!;
    }

    /// <summary>
    /// Formats a collection of values for display in an assertion message.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="values">The values to format.</param>
    /// <returns>A comma-separated representation of the values.</returns>
    private static string DescribeValues<T>(IReadOnlyCollection<T> values)
    {
        return string.Join(", ", values);
    }

    /// <summary>
    /// Determines whether a value equals any of the candidate values.
    /// </summary>
    /// <typeparam name="T">The type of the value and candidates.</typeparam>
    /// <param name="actualValue">The value found in the JSON.</param>
    /// <param name="values">The candidate values.</param>
    /// <param name="comparer">An optional equality comparer, or null to use the default.</param>
    /// <returns>True when at least one candidate matches.</returns>
    private static bool MatchesAny<T>(T? actualValue, IReadOnlyCollection<T> values, IEqualityComparer<T>? comparer)
    {
        return values.Any(value => comparer != null
            ? comparer.Equals(actualValue, value)
            : EqualityComparer<T>.Default.Equals(actualValue!, value!));
    }
}