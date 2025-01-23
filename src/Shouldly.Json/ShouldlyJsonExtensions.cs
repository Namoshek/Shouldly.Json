namespace Shouldly;

using System.Text.Json;
using System.Text.Json.Nodes;

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

            if (!AreJsonNodesEqual(actualNode, expectedNode))
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    internal static bool IsSemanticallySameJson(this string? actual, string? expected)
    {
        if (actual == null && expected == null)
        {
            return true;
        }

        if (actual == null || expected == null)
        {
            return false;
        }

        try
        {
            var actualNode = JsonNode.Parse(actual);
            var expectedNode = JsonNode.Parse(expected);
            
            return AreJsonNodesEqual(actualNode, expectedNode);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool AreJsonNodesEqual(JsonNode? actual, JsonNode? expected)
    {
        if (actual == null || expected == null)
        {
            return actual == expected;
        }

        return (actual, expected) switch
        {
            (JsonObject actualObj, JsonObject expectedObj) => AreJsonObjectsEqual(actualObj, expectedObj),
            (JsonArray actualArr, JsonArray expectedArr) => AreJsonArraysEqual(actualArr, expectedArr),
            (JsonValue actualVal, JsonValue expectedVal) => AreJsonValuesEqual(actualVal, expectedVal),

            _ => false,
        };
    }

    private static bool AreJsonObjectsEqual(JsonObject actual, JsonObject expected)
    {
        if (actual.Count != expected.Count)
        {
            return false;
        }

        foreach (var (key, expectedValue) in expected)
        {
            if (!actual.TryGetPropertyValue(key, out JsonNode? actualValue))
            {
                return false;
            }

            if (!AreJsonNodesEqual(actualValue, expectedValue))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreJsonArraysEqual(JsonArray actual, JsonArray expected)
    {
        if (actual.Count != expected.Count)
        {
            return false;
        }

        for (int i = 0; i < actual.Count; i++)
        {
            if (!AreJsonNodesEqual(actual[i], expected[i]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool AreJsonValuesEqual(JsonValue actual, JsonValue expected)
    {
        // Handle null values.
        if (actual.TryGetValue<JsonElement>(out var actualElement) &&
            expected.TryGetValue<JsonElement>(out var expectedElement))
        {
            return JsonElementEqual(actualElement, expectedElement);
        }

        return false;
    }

    private static bool JsonElementEqual(JsonElement actual, JsonElement expected)
    {
        if (actual.ValueKind != expected.ValueKind)
        {
            return false;
        }

        return actual.ValueKind switch
        {
            JsonValueKind.String => actual.GetString() == expected.GetString(),
            JsonValueKind.Number => CompareNumbers(actual, expected),
            JsonValueKind.True or JsonValueKind.False => actual.GetBoolean() == expected.GetBoolean(),
            JsonValueKind.Null => true,

            _ => false,
        };
    }

    private static bool CompareNumbers(JsonElement actual, JsonElement expected)
    {
        // Try parsing as decimal for most accurate number comparison.
        if (actual.TryGetDecimal(out decimal actualDecimal) && 
            expected.TryGetDecimal(out decimal expectedDecimal))
        {
            return actualDecimal == expectedDecimal;
        }

        // Fallback to string comparison if decimal parsing fails.
        return actual.GetRawText() == expected.GetRawText();
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

            if (!IsJsonSubtree(actualNode, expectedNode))
            {
                throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, errorMessage).ToString());
            }
        }
        catch (JsonException)
        {
            throw new ShouldAssertException(new ExpectedActualShouldlyMessage(expected, actual, $"{errorMessage} (invalid JSON provided)").ToString());
        }
    }

    private static bool IsJsonSubtree(JsonNode? actual, JsonNode? expected)
    {
        if (actual == null || expected == null)
        {
            return actual == expected;
        }

        return (actual, expected) switch
        {
            (JsonObject actualObj, JsonObject expectedObj) => IsJsonObjectSubtree(actualObj, expectedObj),
            (JsonArray actualArr, JsonArray expectedArr) => IsJsonArraySubtree(actualArr, expectedArr),
            (JsonValue actualVal, JsonValue expectedVal) => AreJsonValuesEqual(actualVal, expectedVal),

            _ => false,
        };
    }

    private static bool IsJsonObjectSubtree(JsonObject actual, JsonObject expected)
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

    private static bool IsJsonArraySubtree(JsonArray actual, JsonArray expected)
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
}
