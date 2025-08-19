namespace Shouldly;

/// <summary>
/// Represents the result of a JSON comparison operation.
/// </summary>
public class JsonComparisonResult
{
    /// <summary>
    /// Gets a value indicating whether the JSON structures are equal.
    /// </summary>
    public bool IsEqual { get; init; }

    /// <summary>
    /// Gets the first difference found during comparison, if any.
    /// </summary>
    public JsonDifference? FirstDifference { get; init; }

    /// <summary>
    /// Gets the formatted error message describing the difference, if any.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Creates a successful comparison result indicating the JSON structures are equal.
    /// </summary>
    /// <returns>A JsonComparisonResult indicating success.</returns>
    public static JsonComparisonResult Success()
    {
        return new JsonComparisonResult
        {
            IsEqual = true,
        };
    }

    /// <summary>
    /// Creates a failed comparison result with the specified difference.
    /// </summary>
    /// <param name="difference">The difference that caused the comparison to fail.</param>
    /// <returns>A JsonComparisonResult indicating failure with detailed difference information.</returns>
    public static JsonComparisonResult Failure(JsonDifference difference)
    {
        return new JsonComparisonResult
        {
            IsEqual = false,
            FirstDifference = difference,
            ErrorMessage = difference.Description,
        };
    }

    /// <summary>
    /// Creates a failed comparison result with a custom error message.
    /// </summary>
    /// <param name="errorMessage">The custom error message.</param>
    /// <returns>A JsonComparisonResult indicating failure with a custom message.</returns>
    public static JsonComparisonResult Failure(string errorMessage)
    {
        return new JsonComparisonResult
        {
            IsEqual = false,
            ErrorMessage = errorMessage,
        };
    }

    /// <summary>
    /// Gets the error message, optionally prefixed with a custom message.
    /// </summary>
    /// <param name="customMessage">An optional custom message to prefix the error message.</param>
    /// <returns>The formatted error message.</returns>
    public string GetErrorMessage(string? customMessage = null)
    {
        if (IsEqual)
        {
            return string.Empty;
        }

        var detailedMessage = ErrorMessage ?? "JSON comparison failed";

        return JsonErrorMessageFormatter.CombineMessages(customMessage, detailedMessage);
    }
}
