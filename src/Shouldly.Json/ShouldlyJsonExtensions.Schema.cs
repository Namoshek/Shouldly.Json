namespace Shouldly;

using System;
using System.Linq;
using System.Text.Json;

public static partial class ShouldlyJsonExtensions
{
    /// <summary>
    /// Asserts that the JSON string is valid according to the provided JSON schema.
    /// </summary>
    /// <param name="actual">The JSON string to validate.</param>
    /// <param name="schema">The JSON schema to validate against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON is invalid or does not conform to the schema.</exception>
    public static void ShouldMatchJsonSchema(this string? actual, string schema, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should match the provided schema";

        if (actual is null)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON string is null").ToString());
        }

        Json.Schema.JsonSchema? jsonSchema = null;
        try
        {
            jsonSchema = Json.Schema.JsonSchema.FromText(schema);
            if (jsonSchema is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "Invalid JSON Schema").ToString());
            }
        }
        catch (Exception ex) when (ex is JsonException or ArgumentException)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON Schema: {ex.Message}").ToString());
        }

        try
        {
            using var jsonDocument = JsonDocument.Parse(actual);

            var evaluationResults = jsonSchema.Evaluate(jsonDocument.RootElement, new Json.Schema.EvaluationOptions
            {
                OutputFormat = Json.Schema.OutputFormat.List,
                RequireFormatValidation = true,
            });

            if (evaluationResults is null)
            {
                throw new ShouldAssertException(new ActualShouldlyMessage(actual, "JSON Schema evaluation failed").ToString());
            }

            if (!evaluationResults.IsValid)
            {
                var errors = evaluationResults.Details?.SelectMany(d => d.Errors?.Select(kvp => $"{kvp.Key}: {kvp.Value}") ?? []) ?? [];

                throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"{errorMessage}:\n{string.Join("\n", errors)}").ToString());
            }
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (Json.Schema.JsonSchemaException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON Schema: {ex.Message}").ToString());
        }
    }

    /// <summary>
    /// Asserts that the JSON string does not conform to the provided JSON schema.
    /// </summary>
    /// <param name="actual">The JSON string to validate.</param>
    /// <param name="schema">The JSON schema to validate against.</param>
    /// <param name="customMessage">An optional custom message to include in the exception if the assertion fails.</param>
    /// <exception cref="ShouldAssertException">Thrown if the JSON conforms to the schema or if the schema or JSON is invalid.</exception>
    public static void ShouldNotMatchJsonSchema(this string? actual, string schema, string? customMessage = null)
    {
        var errorMessage = customMessage ?? "JSON should not match the provided schema";

        if (actual is null)
        {
            return;
        }

        Json.Schema.JsonSchema jsonSchema;
        try
        {
            jsonSchema = Json.Schema.JsonSchema.FromText(schema);
        }
        catch (Exception ex) when (ex is JsonException or ArgumentException)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON Schema: {ex.Message}").ToString());
        }

        Json.Schema.EvaluationResults evaluationResults;
        try
        {
            using var jsonDocument = JsonDocument.Parse(actual);

            evaluationResults = jsonSchema.Evaluate(jsonDocument.RootElement, new Json.Schema.EvaluationOptions
            {
                OutputFormat = Json.Schema.OutputFormat.List,
                RequireFormatValidation = true,
            });
        }
        catch (JsonException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON: {ex.Message}").ToString());
        }
        catch (Json.Schema.JsonSchemaException ex)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, $"Invalid JSON Schema: {ex.Message}").ToString());
        }

        if (evaluationResults.IsValid)
        {
            throw new ShouldAssertException(new ActualShouldlyMessage(actual, errorMessage).ToString());
        }
    }
}