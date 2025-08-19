namespace Shouldly;

using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;

internal static class JsonHelper
{
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
