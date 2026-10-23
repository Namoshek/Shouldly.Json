namespace Shouldly;

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Pointer;

public static partial class ShouldlyJsonExtensions
{
    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is of the expected JSON type.
    /// </summary>
    /// <remarks>
    /// <see cref="JsonValueKind.True"/> and <see cref="JsonValueKind.False"/> both assert a boolean.
    /// </remarks>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="expectedKind">The JSON type the value must have.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not of the expected type.</exception>
    public static void ShouldHaveJsonValueType(this string? actual, string jsonPointer, JsonValueKind expectedKind, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, expectedKind, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is not of the given JSON type.
    /// </summary>
    /// <remarks>
    /// <see cref="JsonValueKind.True"/> and <see cref="JsonValueKind.False"/> both refer to a boolean.
    /// </remarks>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="unexpectedKind">The JSON type the value must not have.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is of the unexpected type.</exception>
    public static void ShouldNotHaveJsonValueType(this string? actual, string jsonPointer, JsonValueKind unexpectedKind, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, unexpectedKind, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is a JSON object.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not an object.</exception>
    public static void ShouldBeJsonObjectAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Object, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is not a JSON object.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is an object.</exception>
    public static void ShouldNotBeJsonObjectAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Object, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is a JSON array.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not an array.</exception>
    public static void ShouldBeJsonArrayAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Array, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is not a JSON array.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is an array.</exception>
    public static void ShouldNotBeJsonArrayAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Array, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is a JSON string.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not a string.</exception>
    public static void ShouldBeJsonStringAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.String, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is not a JSON string.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is a string.</exception>
    public static void ShouldNotBeJsonStringAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.String, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is a JSON number.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not a number.</exception>
    public static void ShouldBeJsonNumberAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Number, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is not a JSON number.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is a number.</exception>
    public static void ShouldNotBeJsonNumberAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Number, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is a JSON boolean (<c>true</c> or <c>false</c>).
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not a boolean.</exception>
    public static void ShouldBeJsonBooleanAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.True, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that the value at the specified JSON Pointer (RFC 6901) path is not a JSON boolean (<c>true</c> or <c>false</c>).
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value).</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is a boolean.</exception>
    public static void ShouldNotBeJsonBooleanAt(this string? actual, string jsonPointer, string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.True, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that a value exists at the specified JSON Pointer (RFC 6901) path and that it is JSON <c>null</c>.
    /// </summary>
    /// <remarks>
    /// Unlike <see cref="ShouldHaveJsonProperty(string?,string,string?)"/>, this distinguishes a JSON <c>null</c>
    /// value from a property that carries a value.
    /// </remarks>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value). Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is not <c>null</c>.</exception>
    public static void ShouldHaveJsonNullValue(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Null, negated: false, customMessage);
    }

    /// <summary>
    /// Asserts that a value exists at the specified JSON Pointer (RFC 6901) path and that it is not JSON <c>null</c>.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value. Can be in standard notation (/path/to/value) or URI Fragment Identifier notation (#/path/to/value). Defaults to the root pointer.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if there is no value at the path or it is <c>null</c>.</exception>
    public static void ShouldNotHaveJsonNullValue(this string? actual, string jsonPointer = "", string? customMessage = null)
    {
        AssertJsonKind(actual, jsonPointer, JsonValueKind.Null, negated: true, customMessage);
    }

    /// <summary>
    /// Asserts that the value at a pointer has (or does not have, when negated) the given JSON kind.
    /// </summary>
    /// <param name="actual">The JSON string to check.</param>
    /// <param name="jsonPointer">The JSON Pointer path to the value.</param>
    /// <param name="kind">The JSON kind to compare against.</param>
    /// <param name="negated">When true, the assertion requires the value not to be of the given kind.</param>
    /// <param name="customMessage">An optional custom message to include in the exception.</param>
    /// <param name="shouldlyMethod">Name of the calling assertion, captured automatically.</param>
    /// <exception cref="ShouldAssertException">Thrown when the kind does not satisfy the assertion.</exception>
    private static void AssertJsonKind(
        string? actual,
        string jsonPointer,
        JsonValueKind kind,
        bool negated,
        string? customMessage,
        [CallerMemberName] string shouldlyMethod = null!)
    {
        var result = JsonHelper.TryGetNodeAtPointer(actual, jsonPointer);

        // A JSON document whose root is literally `null` parses to no node. At the root pointer
        // that parsed null *is* the JSON null value, so it must be treated as kind Null.
        var isRootNull = result.Failure == JsonPointerFailure.ParsedToNull && (jsonPointer.Length == 0 || jsonPointer == "#");

        if (!result.Success && !isRootNull)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, customMessage ?? result.Message, shouldlyMethod).ToString());
        }

        var actualKind = isRootNull ? JsonValueKind.Null : result.Value?.GetValueKind() ?? JsonValueKind.Null;
        var matches = KindMatches(actualKind, kind);

        if (matches != negated)
        {
            return;
        }

        var expectedName = JsonHelper.KindName(kind);
        var actualName = JsonHelper.KindName(actualKind);

        var message = negated
            ? customMessage ?? $"JSON value at pointer '{jsonPointer}' should not be {expectedName}"
            : customMessage ?? $"JSON value at pointer '{jsonPointer}' should be {expectedName} but was {actualName}";

        throw new ShouldAssertException(new ActualShouldlyMessage(actual, message, shouldlyMethod).ToString());
    }

    /// <summary>
    /// Determines whether an actual JSON kind satisfies an expected kind, treating <c>true</c>/<c>false</c> as one boolean kind.
    /// </summary>
    /// <param name="actualKind">The kind found in the JSON.</param>
    /// <param name="expectedKind">The kind the assertion asked for.</param>
    /// <returns>True when the kinds match.</returns>
    private static bool KindMatches(JsonValueKind actualKind, JsonValueKind expectedKind)
    {
        if (expectedKind == JsonValueKind.True || expectedKind == JsonValueKind.False)
        {
            return actualKind == JsonValueKind.True || actualKind == JsonValueKind.False;
        }

        return actualKind == expectedKind;
    }
}
