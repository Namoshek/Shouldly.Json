namespace Shouldly;

using System.Collections.Generic;

/// <summary>
/// Provides enhanced formatting for JSON comparison error messages.
/// </summary>
public static class JsonErrorMessageFormatter
{
    private static readonly Dictionary<JsonDifferenceType, string> ErrorTemplates = new()
    {
        [JsonDifferenceType.ValueMismatch] = "JSON value mismatch at path '{0}': expected '{1}' but was '{2}'",
        [JsonDifferenceType.TypeMismatch] = "JSON type mismatch at path '{0}': expected {1} but was {2}",
        [JsonDifferenceType.MissingProperty] = "JSON missing property at path '{0}': expected property '{1}' not found",
        [JsonDifferenceType.ExtraProperty] = "JSON extra property at path '{0}': unexpected property '{1}' found",
        [JsonDifferenceType.ArrayLengthMismatch] = "JSON array length mismatch at path '{0}': expected {1} elements but was {2}",
        [JsonDifferenceType.ArrayElementMismatch] = "JSON array element mismatch at path '{0}': expected '{1}' but was '{2}'",
    };

    /// <summary>
    /// Formats a <see cref="JsonDifference"/> into a human-readable error message.
    /// </summary>
    /// <param name="difference">The difference to format.</param>
    /// <returns>A formatted error message.</returns>
    public static string FormatDifference(JsonDifference difference)
    {
        if (!ErrorTemplates.TryGetValue(difference.Type, out var template))
        {
            return $"JSON comparison failed at path '{difference.Path}': {difference.Description}";
        }

        return difference.Type switch
        {
            JsonDifferenceType.ValueMismatch => string.Format(template, 
                FormatPath(difference.Path), 
                FormatValue(difference.ExpectedValue), 
                FormatValue(difference.ActualValue)),
                
            JsonDifferenceType.TypeMismatch => string.Format(template, 
                FormatPath(difference.Path), 
                difference.ExpectedType, 
                difference.ActualType),
                
            JsonDifferenceType.MissingProperty => string.Format(template, 
                FormatPath(difference.Path), 
                difference.ExpectedValue),
                
            JsonDifferenceType.ExtraProperty => string.Format(template, 
                FormatPath(difference.Path), 
                difference.ActualValue),
                
            JsonDifferenceType.ArrayLengthMismatch => string.Format(template, 
                FormatPath(difference.Path), 
                difference.ExpectedValue, 
                difference.ActualValue),
                
            JsonDifferenceType.ArrayElementMismatch => string.Format(template, 
                FormatPath(difference.Path), 
                FormatValue(difference.ExpectedValue), 
                FormatValue(difference.ActualValue)),
                
            _ => difference.Description,
        };
    }

    /// <summary>
    /// Combines a custom message with a detailed difference message.
    /// </summary>
    /// <param name="customMessage">The custom message provided by the user.</param>
    /// <param name="differenceMessage">The detailed difference message.</param>
    /// <returns>A combined error message.</returns>
    public static string CombineMessages(string? customMessage, string differenceMessage)
    {
        if (string.IsNullOrEmpty(customMessage))
        {
            return differenceMessage ?? "JSON comparison failed";
        }

        if (string.IsNullOrEmpty(differenceMessage))
        {
            return customMessage;
        }

        return $"{customMessage}. {differenceMessage}";
    }

    /// <summary>
    /// Formats a JSON path for display in error messages.
    /// </summary>
    /// <param name="path">The JSON path to format.</param>
    /// <returns>A formatted path string.</returns>
    private static string FormatPath(string path)
    {
        return path ?? string.Empty;
    }

    /// <summary>
    /// Formats a value for display in error messages.
    /// </summary>
    /// <param name="value">The value to format.</param>
    /// <returns>A formatted value string.</returns>
    private static string FormatValue(object? value)
    {
        return value switch
        {
            null => "null",
            string s => s,
            bool b => b.ToString(),
            decimal d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            double d => d.ToString(System.Globalization.CultureInfo.InvariantCulture),
            float f => f.ToString(System.Globalization.CultureInfo.InvariantCulture),
            int i => i.ToString(),
            long l => l.ToString(),

            _ => value.ToString() ?? "null",
        };
    }

    /// <summary>
    /// Truncates a long message if it exceeds the maximum length.
    /// </summary>
    /// <param name="message">The message to potentially truncate.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <returns>The original message or a truncated version.</returns>
    public static string TruncateIfNeeded(string message, int maxLength = 1000)
    {
        if (string.IsNullOrEmpty(message) || message.Length <= maxLength)
        {
            return message;
        }

        return message.Substring(0, maxLength - 3) + "...";
    }

    /// <summary>
    /// Creates a contextual error message that includes information about the comparison context.
    /// </summary>
    /// <param name="difference">The difference that occurred.</param>
    /// <param name="comparisonMode">The mode of comparison that was performed.</param>
    /// <returns>A contextual error message.</returns>
    public static string CreateContextualMessage(JsonDifference difference, ComparisonMode comparisonMode)
    {
        var baseMessage = FormatDifference(difference);
        
        var contextInfo = comparisonMode switch
        {
            ComparisonMode.SemanticEquality => "during semantic equality comparison",
            ComparisonMode.SubtreeMatching => "during subtree matching comparison",

            _ => "during JSON comparison",
        };

        return $"{baseMessage} ({contextInfo})";
    }
}
