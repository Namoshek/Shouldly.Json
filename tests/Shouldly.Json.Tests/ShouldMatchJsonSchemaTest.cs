namespace Shouldly;

public class ShouldMatchJsonSchemaTest
{
    [Fact]
    public void ShouldMatchJsonSchema_WithValidJson_ShouldNotThrow()
    {
        var schema = @"{
            ""type"": ""object"",
            ""properties"": {
                ""name"": { ""type"": ""string"" },
                ""age"": { ""type"": ""integer"", ""minimum"": 0 },
                ""email"": { ""type"": ""string"", ""format"": ""email"" }
            },
            ""required"": [""name"", ""age""]
        }";

        var json = @"{
            ""name"": ""John Doe"",
            ""age"": 30,
            ""email"": ""john@example.com""
        }";

        json.ShouldMatchJsonSchema(schema);
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithArraySchema_ShouldNotThrow()
    {
        var schema = @"{
            ""type"": ""array"",
            ""items"": {
                ""type"": ""object"",
                ""properties"": {
                    ""id"": { ""type"": ""integer"" },
                    ""name"": { ""type"": ""string"" }
                },
                ""required"": [""id"", ""name""]
            },
            ""minItems"": 1
        }";

        var json = @"[
            { ""id"": 1, ""name"": ""Item 1"" },
            { ""id"": 2, ""name"": ""Item 2"" }
        ]";

        json.ShouldMatchJsonSchema(schema);
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithComplexValidation_ShouldNotThrow()
    {
        var schema = @"{
            ""type"": ""object"",
            ""properties"": {
                ""id"": { ""type"": ""integer"" },
                ""name"": { 
                    ""type"": ""string"",
                    ""minLength"": 3,
                    ""maxLength"": 50
                },
                ""tags"": {
                    ""type"": ""array"",
                    ""items"": { ""type"": ""string"" },
                    ""uniqueItems"": true,
                    ""minItems"": 1
                },
                ""metadata"": {
                    ""type"": ""object"",
                    ""additionalProperties"": { ""type"": ""string"" }
                }
            },
            ""required"": [""id"", ""name"", ""tags""]
        }";

        var json = @"{
            ""id"": 1,
            ""name"": ""Test Item"",
            ""tags"": [""tag1"", ""tag2""],
            ""metadata"": {
                ""created"": ""2024-01-01"",
                ""author"": ""John Doe""
            }
        }";

        json.ShouldMatchJsonSchema(schema);
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithInvalidType_ShouldThrow()
    {
        var schema = @"{
            ""type"": ""object"",
            ""properties"": {
                ""age"": { ""type"": ""integer"" }
            }
        }";

        var json = @"{ ""age"": ""not a number"" }";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldMatchJsonSchema(schema));

        ex.Message.ShouldContain("integer");
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithMissingRequired_ShouldThrow()
    {
        var schema = @"{
            ""type"": ""object"",
            ""properties"": {
                ""name"": { ""type"": ""string"" }
            },
            ""required"": [""name""]
        }";

        var json = @"{}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldMatchJsonSchema(schema));

        ex.Message.ShouldContain("required");
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithInvalidFormat_ShouldThrow()
    {
        var schema = @"{
            ""type"": ""object"",
            ""properties"": {
                ""email"": { ""type"": ""string"", ""format"": ""email"" }
            }
        }";

        var json = @"{ ""email"": ""not-an-email"" }";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldMatchJsonSchema(schema));

        ex.Message.ShouldContain("email");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ShouldMatchJsonSchema_WithInvalidJson_ShouldThrow(string? json)
    {
        var schema = @"{ ""type"": ""object"" }";

        Should.Throw<ShouldAssertException>(() => json.ShouldMatchJsonSchema(schema));
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithInvalidSchema_ShouldThrow()
    {
        var schema = "[]";
        var json = "{}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldMatchJsonSchema(schema));

        ex.Message.ShouldContain("Invalid JSON Schema");
    }

    [Fact]
    public void ShouldMatchJsonSchema_WithCustomMessage_ShouldIncludeMessage()
    {
        var schema = @"{ ""type"": ""string"" }";
        var json = "42";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldMatchJsonSchema(schema, customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
