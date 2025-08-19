namespace Shouldly;

using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;

/// <summary>
/// Engine for comparing JSON structures with detailed difference tracking.
/// </summary>
internal static class JsonComparisonEngine
{
    /// <summary>
    /// Compares two JSON nodes for semantic equality.
    /// Both structures must be exactly equivalent, though property order in objects doesn't matter.
    /// </summary>
    /// <param name="actual">The actual JSON node.</param>
    /// <param name="expected">The expected JSON node.</param>
    /// <param name="basePath">The base path for error reporting.</param>
    /// <returns>A <see cref="JsonComparisonResult"/> indicating success or failure with detailed difference information.</returns>
    public static JsonComparisonResult CompareSemanticEquality(JsonNode? actual, JsonNode? expected, string basePath = "")
    {
        return CompareNodes(actual, expected, basePath, ComparisonMode.SemanticEquality);
    }

    /// <summary>
    /// Compares two JSON nodes for subtree matching.
    /// The actual JSON must be a subset of the expected JSON.
    /// </summary>
    /// <param name="actual">The actual JSON node (subset).</param>
    /// <param name="expected">The expected JSON node (superset).</param>
    /// <param name="basePath">The base path for error reporting.</param>
    /// <returns>A JsonComparisonResult indicating success or failure with detailed difference information.</returns>
    public static JsonComparisonResult CompareSubtree(JsonNode? actual, JsonNode? expected, string basePath = "")
    {
        return CompareNodes(actual, expected, basePath, ComparisonMode.SubtreeMatching);
    }

    /// <summary>
    /// Compares two JSON nodes based on the specified comparison mode.
    /// </summary>
    /// <param name="actual">The actual JSON node.</param>
    /// <param name="expected">The expected JSON node.</param>
    /// <param name="path">The current path for error reporting.</param>
    /// <param name="mode">The comparison mode to use.</param>
    /// <param name="depth">The current nesting depth to prevent stack overflow.</param>
    /// <returns>A <see cref="JsonComparisonResult"/> indicating success or failure with detailed difference information.</returns>
    private static JsonComparisonResult CompareNodes(JsonNode? actual, JsonNode? expected, string path, ComparisonMode mode, int depth = 0)
    {
        // Prevent excessive nesting that could cause stack overflow.
        const int MaxDepth = 1000;

        if (depth > MaxDepth)
        {
            var difference = JsonDifference.ValueMismatch(path, "structure too deep", "structure too deep");

            return JsonComparisonResult.Failure(difference);
        }

        // Handle null cases.
        if (actual == null && expected == null)
        {
            return JsonComparisonResult.Success();
        }

        if (actual == null || expected == null)
        {
            var difference = JsonDifference.ValueMismatch(path, expected, actual);

            return JsonComparisonResult.Failure(difference);
        }

        // Get the JSON value kinds for type comparison.
        var actualKind = actual.GetValueKind();
        var expectedKind = expected.GetValueKind();

        // Special case: null values should be treated as value mismatches, not type mismatches.
        if (actualKind == JsonValueKind.Null || expectedKind == JsonValueKind.Null)
        {
            if (actualKind != expectedKind)
            {
                var difference = JsonDifference.ValueMismatch(path, 
                    expectedKind == JsonValueKind.Null ? null : GetValueForDisplay((JsonValue)expected), 
                    actualKind == JsonValueKind.Null ? null : GetValueForDisplay((JsonValue)actual));

                return JsonComparisonResult.Failure(difference);
            }
        }

        // If types do not match, it is a type mismatch.
        // Exception: True vs False are both boolean types, so they should be value mismatches.
        else if (actualKind != expectedKind)
        {
            // Both True and False are boolean types.
            if ((actualKind == JsonValueKind.True || actualKind == JsonValueKind.False) &&
                (expectedKind == JsonValueKind.True || expectedKind == JsonValueKind.False))
            {
                // This is a boolean value mismatch, not a type mismatch.
                // Let it fall through to CompareValues.
            }
            else
            {
                var difference = JsonDifference.TypeMismatch(path, GetTypeName(expectedKind), GetTypeName(actualKind));

                return JsonComparisonResult.Failure(difference);
            }
        }

        // Compare based on the JSON type.
        return actualKind switch
        {
            JsonValueKind.Object => CompareObjects((JsonObject)actual, (JsonObject)expected, path, mode, depth),
            JsonValueKind.Array => CompareArrays((JsonArray)actual, (JsonArray)expected, path, mode, depth),

            _ => CompareValues((JsonValue)actual, (JsonValue)expected, path),
        };
    }

    /// <summary>
    /// Compares two JSON objects based on the specified comparison mode.
    /// </summary>
    /// <param name="actual">The actual JSON object.</param>
    /// <param name="expected">The expected JSON object.</param>
    /// <param name="path">The current path for error reporting.</param>
    /// <param name="mode">The comparison mode to use.</param>
    /// <param name="depth">The current nesting depth.</param>
    /// <returns>A <see cref="JsonComparisonResult"/> indicating success or failure with detailed difference information.</returns>
    private static JsonComparisonResult CompareObjects(JsonObject actual, JsonObject expected, string path, ComparisonMode mode, int depth = 0)
    {
        // In semantic equality mode, check for missing properties in actual.
        if (mode == ComparisonMode.SemanticEquality)
        {
            // Check that all expected properties exist in actual.
            foreach (var expectedProperty in expected)
            {
                if (!actual.ContainsKey(expectedProperty.Key))
                {
                    var propertyPath = JsonPointer.Parse(path).Combine(JsonPointer.Create(expectedProperty.Key)).ToString();
                    var difference = JsonDifference.MissingProperty(propertyPath, expectedProperty.Key);

                    return JsonComparisonResult.Failure(difference);
                }
            }

            // Check that actual doesn't have extra properties.
            foreach (var actualProperty in actual)
            {
                if (!expected.ContainsKey(actualProperty.Key))
                {
                    var propertyPath = JsonPointer.Parse(path).Combine(JsonPointer.Create(actualProperty.Key)).ToString();
                    var difference = JsonDifference.ExtraProperty(propertyPath, actualProperty.Key);

                    return JsonComparisonResult.Failure(difference);
                }
            }
        }

        // SubtreeMatching mode
        else
        {
            // In subtree mode, only check that all actual properties exist in expected (expected can have extra properties).
            foreach (var actualProperty in actual)
            {
                if (!expected.ContainsKey(actualProperty.Key))
                {
                    var propertyPath = JsonPointer.Parse(path).Combine(JsonPointer.Create(actualProperty.Key)).ToString();
                    var difference = JsonDifference.MissingProperty(propertyPath, actualProperty.Key);

                    return JsonComparisonResult.Failure(difference);
                }
            }
        }

        // Compare values of matching properties.
        foreach (var actualProperty in actual)
        {
            if (expected.TryGetPropertyValue(actualProperty.Key, out var expectedValue))
            {
                var propertyPath = JsonPointer.Parse(path).Combine(JsonPointer.Create(actualProperty.Key)).ToString();
                var propertyResult = CompareNodes(actualProperty.Value, expectedValue, propertyPath, mode, depth + 1);
                
                if (!propertyResult.IsEqual)
                {
                    return propertyResult;
                }
            }
        }

        return JsonComparisonResult.Success();
    }

    /// <summary>
    /// Compares two JSON arrays based on the specified comparison mode.
    /// </summary>
    /// <param name="actual">The actual JSON array.</param>
    /// <param name="expected">The expected JSON array.</param>
    /// <param name="path">The current path for error reporting.</param>
    /// <param name="mode">The comparison mode to use.</param>
    /// <param name="depth">The current nesting depth.</param>
    /// <returns>A <see cref="JsonComparisonResult"/> indicating success or failure with detailed difference information.</returns>
    private static JsonComparisonResult CompareArrays(JsonArray actual, JsonArray expected, string path, ComparisonMode mode, int depth = 0)
    {
        // Arrays must match exactly in both semantic equality and subtree matching modes (check length first).
        if (actual.Count != expected.Count)
        {
            var difference = JsonDifference.ArrayLengthMismatch(path, expected.Count, actual.Count);

            return JsonComparisonResult.Failure(difference);
        }

        // Compare elements at each index.
        for (int i = 0; i < actual.Count; i++)
        {
            var elementPath = JsonPointer.Parse(path).Combine(JsonPointer.Create(i)).ToString();
            var elementResult = CompareNodes(actual[i], expected[i], elementPath, mode, depth + 1);
            
            if (!elementResult.IsEqual)
            {
                return elementResult;
            }
        }

        return JsonComparisonResult.Success();
    }

    /// <summary>
    /// Compares two JSON values (primitives).
    /// </summary>
    /// <param name="actual">The actual JSON value.</param>
    /// <param name="expected">The expected JSON value.</param>
    /// <param name="path">The current path for error reporting.</param>
    /// <returns>A <see cref="JsonComparisonResult"/> indicating success or failure with detailed difference information.</returns>
    private static JsonComparisonResult CompareValues(JsonValue actual, JsonValue expected, string path)
    {
        var actualKind = actual.GetValueKind();
        var expectedKind = expected.GetValueKind();

        // Handle null values.
        if (actualKind == JsonValueKind.Null && expectedKind == JsonValueKind.Null)
        {
            return JsonComparisonResult.Success();
        }

        if (actualKind == JsonValueKind.Null || expectedKind == JsonValueKind.Null)
        {
            var difference = JsonDifference.ValueMismatch(path, 
                expectedKind == JsonValueKind.Null ? null : GetValueForDisplay(expected),
                actualKind == JsonValueKind.Null ? null : GetValueForDisplay(actual));

            return JsonComparisonResult.Failure(difference);
        }

        // Handle boolean values.
        if (actualKind == JsonValueKind.True || actualKind == JsonValueKind.False)
        {
            if (expectedKind != JsonValueKind.True && expectedKind != JsonValueKind.False)
            {
                var difference = JsonDifference.TypeMismatch(path, GetTypeName(expectedKind), GetTypeName(actualKind));

                return JsonComparisonResult.Failure(difference);
            }

            if (actualKind != expectedKind)
            {
                var difference = JsonDifference.ValueMismatch(path, GetValueForDisplay(expected), GetValueForDisplay(actual));

                return JsonComparisonResult.Failure(difference);
            }

            return JsonComparisonResult.Success();
        }

        // Handle boolean values for expected (when actual is not boolean).
        if (expectedKind == JsonValueKind.True || expectedKind == JsonValueKind.False)
        {
            var difference = JsonDifference.TypeMismatch(path, GetTypeName(expectedKind), GetTypeName(actualKind));

            return JsonComparisonResult.Failure(difference);
        }

        // Handle string values
        if (actualKind == JsonValueKind.String)
        {
            if (expectedKind != JsonValueKind.String)
            {
                var difference = JsonDifference.TypeMismatch(path, GetTypeName(expectedKind), GetTypeName(actualKind));
                return JsonComparisonResult.Failure(difference);
            }

            var actualString = actual.GetValue<string>();
            var expectedString = expected.GetValue<string>();

            // Try to parse as DateTimeOffset for semantic date comparison.
            if (TryParseDateTimeOffset(actualString, out var actualDate) && 
                TryParseDateTimeOffset(expectedString, out var expectedDate))
            {
                if (actualDate != expectedDate)
                {
                    var difference = JsonDifference.ValueMismatch(path, expectedString, actualString);

                    return JsonComparisonResult.Failure(difference);
                }

                return JsonComparisonResult.Success();
            }

            // Regular string comparison.
            if (actualString != expectedString)
            {
                var difference = JsonDifference.ValueMismatch(path, expectedString, actualString);

                return JsonComparisonResult.Failure(difference);
            }

            return JsonComparisonResult.Success();
        }

        // Handle numeric values.
        if (actualKind == JsonValueKind.Number)
        {
            if (expectedKind != JsonValueKind.Number)
            {
                var difference = JsonDifference.TypeMismatch(path, GetTypeName(expectedKind), GetTypeName(actualKind));

                return JsonComparisonResult.Failure(difference);
            }

            // Compare as decimal for precision, fall back to double for extreme values.
            decimal actualDecimal, expectedDecimal;
            
            if (!actual.TryGetValue<decimal>(out actualDecimal))
            {
                var actualDouble = actual.GetValue<double>();

                try
                {
                    actualDecimal = (decimal)actualDouble;
                }
                catch (OverflowException)
                {
                    // For extreme values that can't be represented as decimal, compare as double.
                    var expectedDouble = expected.GetValue<double>();
                    if (actualDouble != expectedDouble)
                    {
                        var difference = JsonDifference.ValueMismatch(path, GetValueForDisplay(expected), GetValueForDisplay(actual));

                        return JsonComparisonResult.Failure(difference);
                    }

                    return JsonComparisonResult.Success();
                }
            }

            if (!expected.TryGetValue<decimal>(out expectedDecimal))
            {
                var expectedDouble = expected.GetValue<double>();

                try
                {
                    expectedDecimal = (decimal)expectedDouble;
                }
                catch (OverflowException)
                {
                    // Already handled above.
                    return JsonComparisonResult.Success();
                }
            }

            if (actualDecimal != expectedDecimal)
            {
                var difference = JsonDifference.ValueMismatch(path, GetValueForDisplay(expected), GetValueForDisplay(actual));

                return JsonComparisonResult.Failure(difference);
            }

            return JsonComparisonResult.Success();
        }

        // If we get here, there's an unexpected value kind.
        var typeDifference = JsonDifference.TypeMismatch(path, GetTypeName(expectedKind), GetTypeName(actualKind));

        return JsonComparisonResult.Failure(typeDifference);
    }

    /// <summary>
    /// Gets a value from a JsonValue for display purposes.
    /// </summary>
    /// <param name="jsonValue">The JsonValue to extract the display value from.</param>
    /// <returns>The value suitable for display in error messages.</returns>
    private static object? GetValueForDisplay(JsonValue jsonValue)
    {
        var kind = jsonValue.GetValueKind();

        return kind switch
        {
            JsonValueKind.String => jsonValue.GetValue<string>(),
            JsonValueKind.Number => jsonValue.TryGetValue<decimal>(out var decimalValue) ? decimalValue : jsonValue.GetValue<double>(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,

            _ => jsonValue.ToString(),
        };
    }

    /// <summary>
    /// Tries to parse a string as a DateTimeOffset for semantic date comparison.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <param name="dateTime">The parsed DateTimeOffset if successful.</param>
    /// <returns>True if the string was successfully parsed as a DateTimeOffset, false otherwise.</returns>
    private static bool TryParseDateTimeOffset(string value, out DateTimeOffset dateTime)
    {
        return DateTimeOffset.TryParse(value, out dateTime);
    }

    /// <summary>
    /// Gets a human-readable type name for a JsonValueKind.
    /// </summary>
    /// <param name="kind">The JsonValueKind to get the name for.</param>
    /// <returns>A human-readable type name.</returns>
    private static string GetTypeName(JsonValueKind kind)
    {
        return kind switch
        {
            JsonValueKind.Object => "object",
            JsonValueKind.Array => "array",
            JsonValueKind.String => "string",
            JsonValueKind.Number => "number",
            JsonValueKind.True => "boolean",
            JsonValueKind.False => "boolean",
            JsonValueKind.Null => "null",

            _ => "unknown",
        };
    }
}
