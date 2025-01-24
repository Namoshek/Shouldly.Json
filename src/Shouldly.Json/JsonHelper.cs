namespace Shouldly;

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
            (JsonValue actualVal, JsonValue expectedVal) => actualVal.IsEquivalentTo(expectedVal),

            _ => false,
        };
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

    internal static T? GetValueAtPointer<T>(string json, string pointer)
    {
        var jsonNode = JsonNode.Parse(json);
        if (jsonNode == null)
        {
            return default;
        }

        var jsonPointer = JsonPointer.Parse(pointer);
        if (!jsonPointer.TryEvaluate(jsonNode, out var result))
        {
            throw new ShouldAssertException("Pointer does not evaluate to a JSON node.");
        }
        
        return result.Deserialize<T>();
    }
}
