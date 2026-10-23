namespace Shouldly;

using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;

/// <summary>
/// The reason why a pointer evaluation could not produce a usable value.
/// </summary>
internal enum JsonPointerFailure
{
    /// <summary>No failure.</summary>
    None,

    /// <summary>The JSON string was null or empty.</summary>
    EmptyJson,

    /// <summary>The JSON string parsed to null.</summary>
    ParsedToNull,

    /// <summary>The JSON string is not valid JSON.</summary>
    InvalidJson,

    /// <summary>The JSON pointer is not valid.</summary>
    InvalidPointer,

    /// <summary>No value exists at the pointer.</summary>
    NotFound,

    /// <summary>The value exists but cannot be converted to the requested type.</summary>
    TypeConversion,
}

/// <summary>
/// The outcome of evaluating a JSON pointer, without throwing.
/// </summary>
/// <typeparam name="T">The type the value was deserialized to.</typeparam>
/// <param name="Value">The deserialized value, only meaningful when <see cref="Success"/> is true.</param>
/// <param name="Failure">Why the evaluation failed, if it did.</param>
/// <param name="Message">Human-readable failure detail for assertion messages.</param>
internal readonly record struct JsonPointerResult<T>(T? Value, JsonPointerFailure Failure, string? Message)
{
    /// <summary>
    /// Gets whether the pointer was evaluated and a value was produced.
    /// </summary>
    public bool Success => Failure == JsonPointerFailure.None;
}

internal static class JsonHelper
{
    /// <summary>
    /// Evaluates a JSON pointer and returns the result instead of throwing.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value at the pointer to.</typeparam>
    /// <param name="json">The JSON string to evaluate against.</param>
    /// <param name="pointer">The JSON Pointer (RFC 6901) path, optionally in URI fragment form.</param>
    /// <param name="allowNull">When true, a JSON <c>null</c> value is a successful result.</param>
    /// <returns>A <see cref="JsonPointerResult{T}"/> describing either the value or the failure.</returns>
    public static JsonPointerResult<T> TryGetValueAtPointer<T>(string? json, string pointer, bool allowNull = false)
    {
        var nodeResult = TryGetNodeAtPointer(json, pointer);
        if (!nodeResult.Success)
        {
            return new JsonPointerResult<T>(default, nodeResult.Failure, nodeResult.Message);
        }

        var result = (JsonNode?)nodeResult.Value!;

        if (result is null)
        {
            return allowNull
                ? new JsonPointerResult<T>(default, JsonPointerFailure.None, null)
                : new JsonPointerResult<T>(default, JsonPointerFailure.NotFound, $"No value found at JSON pointer '{pointer}'");
        }

        try
        {
            var value = result.Deserialize<T>();

            if (value == null)
            {
                return new JsonPointerResult<T>(default, JsonPointerFailure.TypeConversion,
                    $"Value at JSON pointer '{pointer}' could not be deserialized to type {typeof(T).Name}");
            }

            return new JsonPointerResult<T>(value, JsonPointerFailure.None, null);
        }
        catch (JsonException ex)
        {
            return new JsonPointerResult<T>(default, JsonPointerFailure.TypeConversion,
                $"Value at JSON pointer '{pointer}' is not a valid {typeof(T).Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Parses the JSON string and evaluates a pointer, returning the raw node instead of throwing.
    /// </summary>
    /// <param name="json">The JSON string to evaluate against.</param>
    /// <param name="pointer">The JSON Pointer (RFC 6901) path, optionally in URI fragment form.</param>
    /// <returns>A result whose <c>Value</c> is the node at the pointer (may itself be JSON null).</returns>
    public static JsonPointerResult<JsonNode?> TryGetNodeAtPointer(string? json, string pointer)
    {
        if (string.IsNullOrEmpty(json))
        {
            return new JsonPointerResult<JsonNode?>(default, JsonPointerFailure.EmptyJson, "JSON string cannot be null or empty");
        }

        JsonNode? jsonNode;
        try
        {
            jsonNode = JsonNode.Parse(json);
        }
        catch (JsonException ex)
        {
            return new JsonPointerResult<JsonNode?>(default, JsonPointerFailure.InvalidJson, $"Invalid JSON: {ex.Message}");
        }

        if (jsonNode == null)
        {
            return new JsonPointerResult<JsonNode?>(default, JsonPointerFailure.ParsedToNull, "JSON string parsed to null");
        }

        JsonPointer jsonPointer;
        try
        {
            jsonPointer = JsonPointer.Parse(pointer);
        }
        catch (PointerParseException ex)
        {
            return new JsonPointerResult<JsonNode?>(default, JsonPointerFailure.InvalidPointer, $"Invalid JSON pointer '{pointer}': {ex.Message}");
        }

        if (!jsonPointer.TryEvaluate(jsonNode, out var result))
        {
            return new JsonPointerResult<JsonNode?>(default, JsonPointerFailure.NotFound, $"No value found at JSON pointer '{pointer}'");
        }

        return new JsonPointerResult<JsonNode?>(result, JsonPointerFailure.None, null);
    }

    /// <summary>
    /// Gets a human-readable name for a JSON value kind, matching the comparison engine's wording.
    /// </summary>
    /// <param name="kind">The JSON value kind.</param>
    /// <returns>A human-readable type name.</returns>
    public static string KindName(JsonValueKind kind)
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

    /// <summary>
    /// Gets the value at a pointer, throwing a <see cref="ShouldAssertException"/> when it cannot be resolved.
    /// </summary>
    /// <typeparam name="T">The type to deserialize the value at the pointer to.</typeparam>
    /// <param name="json">The JSON string to evaluate against.</param>
    /// <param name="pointer">The JSON Pointer (RFC 6901) path, optionally in URI fragment form.</param>
    /// <param name="allowNull">When true, a JSON <c>null</c> value is a successful result.</param>
    /// <returns>The value at the pointer.</returns>
    public static T? GetValueAtPointer<T>(string? json, string pointer, bool allowNull = false)
    {
        var result = TryGetValueAtPointer<T>(json, pointer, allowNull);

        if (!result.Success)
        {
            throw new ShouldAssertException(result.Message ?? "No value found at JSON pointer");
        }

        return result.Value;
    }
}
