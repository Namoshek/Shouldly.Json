namespace Shouldly;

/// <summary>
/// Represents a specific difference found during JSON comparison.
/// </summary>
internal class JsonDifference
{
    /// <summary>
    /// Gets the JSON Pointer path where the difference was found.
    /// </summary>
    public string Path { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of difference that was found.
    /// </summary>
    public JsonDifferenceType Type { get; init; }

    /// <summary>
    /// Gets the expected value at the difference location.
    /// </summary>
    public object? ExpectedValue { get; init; }

    /// <summary>
    /// Gets the actual value at the difference location.
    /// </summary>
    public object? ActualValue { get; init; }

    /// <summary>
    /// Gets the expected JSON type name.
    /// </summary>
    public string? ExpectedType { get; init; }

    /// <summary>
    /// Gets the actual JSON type name.
    /// </summary>
    public string? ActualType { get; init; }

    /// <summary>
    /// Gets a human-readable description of the difference.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Creates a new <see cref="JsonDifference"/> for a value mismatch.
    /// </summary>
    /// <param name="path">The JSON Pointer path where the difference occurred.</param>
    /// <param name="expectedValue">The expected value.</param>
    /// <param name="actualValue">The actual value.</param>
    /// <returns>A <see cref="JsonDifference"/> representing the value mismatch.</returns>
    public static JsonDifference ValueMismatch(string path, object? expectedValue, object? actualValue)
    {
        var difference = new JsonDifference
        {
            Path = path,
            Type = JsonDifferenceType.ValueMismatch,
            ExpectedValue = expectedValue,
            ActualValue = actualValue,
        };
        
        difference.Description = JsonErrorMessageFormatter.FormatDifference(difference);

        return difference;
    }

    /// <summary>
    /// Creates a new <see cref="JsonDifference"/> for a type mismatch.
    /// </summary>
    /// <param name="path">The JSON Pointer path where the difference occurred.</param>
    /// <param name="expectedType">The expected JSON type.</param>
    /// <param name="actualType">The actual JSON type.</param>
    /// <returns>A <see cref="JsonDifference"/> representing the type mismatch.</returns>
    public static JsonDifference TypeMismatch(string path, string expectedType, string actualType)
    {
        var difference = new JsonDifference
        {
            Path = path,
            Type = JsonDifferenceType.TypeMismatch,
            ExpectedType = expectedType,
            ActualType = actualType,
        };
        
        difference.Description = JsonErrorMessageFormatter.FormatDifference(difference);

        return difference;
    }

    /// <summary>
    /// Creates a new <see cref="JsonDifference"/> for a missing property.
    /// </summary>
    /// <param name="path">The JSON Pointer path where the property should exist.</param>
    /// <param name="propertyName">The name of the missing property.</param>
    /// <returns>A <see cref="JsonDifference"/> representing the missing property.</returns>
    public static JsonDifference MissingProperty(string path, string propertyName)
    {
        var difference = new JsonDifference
        {
            Path = path,
            Type = JsonDifferenceType.MissingProperty,
            ExpectedValue = propertyName,
        };
        
        difference.Description = JsonErrorMessageFormatter.FormatDifference(difference);

        return difference;
    }

    /// <summary>
    /// Creates a new <see cref="JsonDifference"/> for an extra property.
    /// </summary>
    /// <param name="path">The JSON Pointer path where the extra property exists.</param>
    /// <param name="propertyName">The name of the extra property.</param>
    /// <returns>A <see cref="JsonDifference"/> representing the extra property.</returns>
    public static JsonDifference ExtraProperty(string path, string propertyName)
    {
        var difference = new JsonDifference
        {
            Path = path,
            Type = JsonDifferenceType.ExtraProperty,
            ActualValue = propertyName,
        };
        
        difference.Description = JsonErrorMessageFormatter.FormatDifference(difference);

        return difference;
    }

    /// <summary>
    /// Creates a new <see cref="JsonDifference"/> for an array length mismatch.
    /// </summary>
    /// <param name="path">The JSON Pointer path where the array exists.</param>
    /// <param name="expectedLength">The expected array length.</param>
    /// <param name="actualLength">The actual array length.</param>
    /// <returns>A <see cref="JsonDifference"/> representing the array length mismatch.</returns>
    public static JsonDifference ArrayLengthMismatch(string path, int expectedLength, int actualLength)
    {
        var difference = new JsonDifference
        {
            Path = path,
            Type = JsonDifferenceType.ArrayLengthMismatch,
            ExpectedValue = expectedLength,
            ActualValue = actualLength,
        };
        
        difference.Description = JsonErrorMessageFormatter.FormatDifference(difference);

        return difference;
    }

    /// <summary>
    /// Creates a new <see cref="JsonDifference"/> for an array element mismatch.
    /// </summary>
    /// <param name="path">The JSON Pointer path where the array element exists.</param>
    /// <param name="expectedValue">The expected element value.</param>
    /// <param name="actualValue">The actual element value.</param>
    /// <returns>A <see cref="JsonDifference"/> representing the array element mismatch.</returns>
    public static JsonDifference ArrayElementMismatch(string path, object? expectedValue, object? actualValue)
    {
        var difference = new JsonDifference
        {
            Path = path,
            Type = JsonDifferenceType.ArrayElementMismatch,
            ExpectedValue = expectedValue,
            ActualValue = actualValue,
        };
        
        difference.Description = JsonErrorMessageFormatter.FormatDifference(difference);

        return difference;
    }
}
