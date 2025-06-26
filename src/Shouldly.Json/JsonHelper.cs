namespace Shouldly;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.More;
using Json.Pointer;

internal static class JsonHelper
{
    internal static bool IsJsonSubtree(JsonNode? actual, JsonNode? expected)
    {
        if (actual == null || expected == null)
        {
            return actual == expected;
        }

        return (actual, expected) switch
        {
            (JsonObject actualObj, JsonObject expectedObj) => IsJsonObjectSubtree(actualObj, expectedObj),
            (JsonArray actualArr, JsonArray expectedArr) => IsJsonArraySubtree(actualArr, expectedArr),
            (JsonValue actualVal, JsonValue expectedVal) => TryParseAsDates(actualVal, expectedVal, out var actualDate, out var expectedDate)
                ? actualDate == expectedDate
                : actualVal.IsEquivalentTo(expectedVal),

            _ => false,
        };
    }

    private static bool TryParseAsDates(JsonValue actualVal, JsonValue expectedVal, out DateTimeOffset actualDate, out DateTimeOffset expectedDate)
    {
        if (actualVal.GetValueKind() == JsonValueKind.String
            && expectedVal.GetValueKind() == JsonValueKind.String
            && actualVal.TryGetValue<DateTimeOffset>(out actualDate)
            && expectedVal.TryGetValue<DateTimeOffset>(out expectedDate))
        {
            return true;
        }

        actualDate = default;
        expectedDate = default;

        return false;
    }

    internal static bool IsJsonObjectSubtree(JsonObject actual, JsonObject expected)
    {
        // For each property in actual, it must exist in expected and match.
        foreach (var (key, actualValue) in actual)
        {
            if (!expected.TryGetPropertyValue(key, out JsonNode? expectedValue))
            {
                return false;
            }

            if (!IsJsonSubtree(actualValue, expectedValue))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool IsJsonArraySubtree(JsonArray actual, JsonArray expected)
    {
        // Arrays must match exactly in length and content.
        if (actual.Count != expected.Count)
        {
            return false;
        }

        for (int i = 0; i < actual.Count; i++)
        {
            if (!IsJsonSubtree(actual[i], expected[i]))
            {
                return false;
            }
        }

        return true;
    }

    internal static T? GetValueAtPointer<T>(string? json, string pointer, bool allowNull = false)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ShouldAssertException("JSON string cannot be null or empty");
        }

        try
        {
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode == null)
            {
                throw new ShouldAssertException("JSON string parsed to null");
            }

            var jsonPointer = JsonPointer.Parse(pointer);
            if (!jsonPointer.TryEvaluate(jsonNode, out var result))
            {
                throw new ShouldAssertException($"No value found at JSON pointer '{pointer}'");
            }

            if (result is null)
            {
                return allowNull ? default : throw new ShouldAssertException($"No value found at JSON pointer '{pointer}'");
            }

            try
            {
                var value = result.Deserialize<T>();

                if (value == null)
                {
                    throw new ShouldAssertException($"Value at JSON pointer '{pointer}' could not be deserialized to type {typeof(T).Name}");
                }

                return value;
            }
            catch (JsonException ex)
            {
                throw new ShouldAssertException($"Value at JSON pointer '{pointer}' is not a valid {typeof(T).Name}: {ex.Message}");
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException($"Invalid JSON: {ex.Message}");
        }
        catch (PointerParseException ex)
        {
            throw new ShouldAssertException($"Invalid JSON pointer '{pointer}': {ex.Message}");
        }
    }
}
