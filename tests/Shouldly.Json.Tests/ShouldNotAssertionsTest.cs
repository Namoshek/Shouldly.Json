namespace Shouldly;

using System;
using System.Collections.Generic;

public class ShouldNotAssertionsTest
{
    private const string ObjectJson = @"{""name"": ""John""}";
    private const string ArrayJson = @"[1, 2, 3]";
    private const string Schema = @"{""type"": ""object"", ""required"": [""name""], ""properties"": {""name"": {""type"": ""string""}}}";

    // ShouldNotBeValidJson

    [Theory]
    [InlineData("{invalid json}")]
    [InlineData("{")]
    [InlineData(@"{""trailing"": ""comma"",}")]
    public void ShouldNotBeValidJson_WithInvalidJson_ShouldNotThrow(string json)
    {
        json.ShouldNotBeValidJson();
    }

    [Theory]
    [InlineData(@"{""a"": 1}")]
    [InlineData("[1, 2]")]
    [InlineData("42")]
    [InlineData(@"{""value"": null}")]
    public void ShouldNotBeValidJson_WithValidJson_ShouldThrow(string json)
    {
        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotBeValidJson());

        ex.Message.ShouldContain("String should not be valid JSON");
    }

    [Fact]
    public void ShouldNotBeValidJson_WithNull_ShouldNotThrow()
    {
        string? json = null;

        json.ShouldNotBeValidJson();
    }

    [Fact]
    public void ShouldNotBeValidJson_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => @"{""a"": 1}".ShouldNotBeValidJson(customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldNotBeJsonObject / ShouldNotBeJsonArray

    [Fact]
    public void ShouldNotBeJsonObject_WithArray_ShouldNotThrow()
    {
        ArrayJson.ShouldNotBeJsonObject();
    }

    [Fact]
    public void ShouldNotBeJsonObject_WithObject_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ObjectJson.ShouldNotBeJsonObject());

        ex.Message.ShouldContain("should not have an object as root element");
    }

    [Fact]
    public void ShouldNotBeJsonObject_WithNull_ShouldNotThrow()
    {
        string? json = null;

        json.ShouldNotBeJsonObject();
    }

    [Fact]
    public void ShouldNotBeJsonObject_WithInvalidJson_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "{invalid json}".ShouldNotBeJsonObject());

        ex.Message.ShouldContain("Invalid JSON");
    }

    [Fact]
    public void ShouldNotBeJsonArray_WithObject_ShouldNotThrow()
    {
        ObjectJson.ShouldNotBeJsonArray();
    }

    [Fact]
    public void ShouldNotBeJsonArray_WithArray_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ArrayJson.ShouldNotBeJsonArray());

        ex.Message.ShouldContain("should not have an array as root element");
    }

    [Fact]
    public void ShouldNotBeJsonArray_WithNull_ShouldNotThrow()
    {
        string? json = null;

        json.ShouldNotBeJsonArray();
    }

    // ShouldNotHaveJsonSubtree

    [Fact]
    public void ShouldNotHaveJsonSubtree_WhenExpectedIsNotSubtree_ShouldNotThrow()
    {
        var actual = @"{""a"": 1}";
        var expected = @"{""a"": 1, ""b"": 2}";

        actual.ShouldNotHaveJsonSubtree(expected);
    }

    [Fact]
    public void ShouldNotHaveJsonSubtree_WhenExpectedIsSubtree_ShouldThrow()
    {
        var actual = @"{""a"": 1, ""b"": 2}";
        var expected = @"{""a"": 1}";

        var ex = Should.Throw<ShouldAssertException>(() => actual.ShouldNotHaveJsonSubtree(expected));

        ex.Message.ShouldContain("should not have expected JSON as subtree");
    }

    [Fact]
    public void ShouldNotHaveJsonSubtree_WithValueMismatch_ShouldNotThrow()
    {
        var actual = @"{""a"": 99}";
        var expected = @"{""a"": 1}";

        actual.ShouldNotHaveJsonSubtree(expected);
    }

    [Theory]
    [InlineData(null, null)]
    public void ShouldNotHaveJsonSubtree_WithBothNull_ShouldThrow(string? actual, string? expected)
    {
        Should.Throw<ShouldAssertException>(() => actual.ShouldNotHaveJsonSubtree(expected));
    }

    [Theory]
    [InlineData(null, @"{""a"": 1}")]
    [InlineData(@"{""a"": 1}", null)]
    public void ShouldNotHaveJsonSubtree_WithOneNull_ShouldNotThrow(string? actual, string? expected)
    {
        actual.ShouldNotHaveJsonSubtree(expected);
    }

    [Fact]
    public void ShouldNotHaveJsonSubtree_WithInvalidJson_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "{invalid}".ShouldNotHaveJsonSubtree(@"{""a"": 1}"));

        ex.Message.ShouldContain("invalid JSON provided");
    }

    [Fact]
    public void ShouldNotHaveJsonSubtree_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => @"{""a"": 1}".ShouldNotHaveJsonSubtree(@"{""a"": 1}", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldNotMatchJsonSchema

    [Fact]
    public void ShouldNotMatchJsonSchema_WhenSchemaViolated_ShouldNotThrow()
    {
        @"{""name"": 42}".ShouldNotMatchJsonSchema(Schema);
    }

    [Fact]
    public void ShouldNotMatchJsonSchema_WhenSchemaMatches_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ObjectJson.ShouldNotMatchJsonSchema(Schema));

        ex.Message.ShouldContain("should not match the provided schema");
    }

    [Fact]
    public void ShouldNotMatchJsonSchema_WithNull_ShouldNotThrow()
    {
        string? json = null;

        json.ShouldNotMatchJsonSchema(Schema);
    }

    [Fact]
    public void ShouldNotMatchJsonSchema_WithInvalidSchema_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => ObjectJson.ShouldNotMatchJsonSchema("{invalid schema"));

        ex.Message.ShouldContain("Invalid JSON Schema");
    }

    [Fact]
    public void ShouldNotMatchJsonSchema_WithInvalidJson_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "{invalid json}".ShouldNotMatchJsonSchema(Schema));

        ex.Message.ShouldContain("Invalid JSON");
    }

    [Fact]
    public void ShouldNotMatchJsonSchema_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => ObjectJson.ShouldNotMatchJsonSchema(Schema, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    // ShouldNotHaveJsonValue

    [Fact]
    public void ShouldNotHaveJsonValue_WithDifferentValue_ShouldNotThrow()
    {
        var json = @"{""value"": ""actual""}";

        json.ShouldNotHaveJsonValue("/value", "expected");
    }

    [Fact]
    public void ShouldNotHaveJsonValue_WithMatchingValue_ShouldThrow()
    {
        var json = @"{""value"": ""same""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValue("/value", "same"));

        ex.Message.ShouldContain("should not have value 'same'");
    }

    [Fact]
    public void ShouldNotHaveJsonValue_MissingPointer_ShouldNotThrow()
    {
        var json = @"{""value"": 1}";

        json.ShouldNotHaveJsonValue("/missing", 1);
    }

    [Fact]
    public void ShouldNotHaveJsonValue_WithNullValue_AndNullUnexpected_ShouldThrow()
    {
        var json = @"{""value"": null}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValue<string?>("/value", null));

        ex.Message.ShouldContain("should not have value");
    }

    [Fact]
    public void ShouldNotHaveJsonValue_WithComparer_ShouldUseComparer()
    {
        var json = @"{""value"": ""TEST""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValue("/value", "test", StringComparer.OrdinalIgnoreCase));
    }

    [Fact]
    public void ShouldNotHaveJsonValue_InvalidPointer_ShouldThrow()
    {
        var json = @"{""value"": 1}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValue("invalid", 1));

        ex.Message.ShouldContain("Invalid JSON pointer");
    }

    [Fact]
    public void ShouldNotHaveJsonValue_TypeMismatch_ShouldThrow()
    {
        var json = @"{""value"": ""text""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValue("/value", 42));

        ex.Message.ShouldContain("is not a valid Int32");
    }

    // ShouldNotHaveJsonValueBetween

    [Fact]
    public void ShouldNotHaveJsonValueBetween_OutsideRange_ShouldNotThrow()
    {
        var json = @"{""value"": 100}";

        json.ShouldNotHaveJsonValueBetween("/value", 1, 10);
    }

    [Fact]
    public void ShouldNotHaveJsonValueBetween_WithinRange_ShouldThrow()
    {
        var json = @"{""value"": 5}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueBetween("/value", 1, 10));

        ex.Message.ShouldContain("should not be between 1 and 10");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    public void ShouldNotHaveJsonValueBoundariesAreInclusive(int value)
    {
        var json = $@"{{""value"": {value}}}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueBetween("/value", 1, 10));
    }

    // ShouldNotHaveJsonDateBetween

    [Fact]
    public void ShouldNotHaveJsonDateBetween_OutsideRange_ShouldNotThrow()
    {
        var json = @"{""date"": ""2025-01-01T00:00:00Z""}";

        json.ShouldNotHaveJsonDateBetween("/date", new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero), new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero));
    }

    [Fact]
    public void ShouldNotHaveJsonDateBetween_WithinRange_ShouldThrow()
    {
        var json = @"{""date"": ""2025-07-01T00:00:00Z""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonDateBetween(
            "/date",
            new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero)));

        ex.Message.ShouldContain("should not be between");
        ex.Message.ShouldContain("(inclusive)");
        ex.Message.ShouldContain("(exclusive)");
    }

    [Fact]
    public void ShouldNotHaveJsonDateBetween_ExclusiveEndIsAllowed()
    {
        var json = @"{""date"": ""2025-12-01T00:00:00Z""}";

        json.ShouldNotHaveJsonDateBetween(
            "/date",
            new DateTimeOffset(2025, 6, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2025, 12, 1, 0, 0, 0, TimeSpan.Zero));
    }

    // ShouldHaveJsonValueOneOf / ShouldNotHaveJsonValueOneOf

    [Fact]
    public void ShouldHaveJsonValueOneOf_WithMatchingValue_ShouldNotThrow()
    {
        var json = @"{""value"": ""b""}";

        json.ShouldHaveJsonValueOneOf("/value", new[] { "a", "b", "c" });
    }

    [Fact]
    public void ShouldHaveJsonValueOneOf_WithNoMatch_ShouldThrow()
    {
        var json = @"{""value"": ""z""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueOneOf("/value", new[] { "a", "b" }));

        ex.Message.ShouldContain("should be one of a, b");
    }

    [Fact]
    public void ShouldHaveJsonValueOneOf_WithComparer_ShouldUseComparer()
    {
        var json = @"{""value"": ""B""}";

        json.ShouldHaveJsonValueOneOf("/value", new[] { "a", "b" }, StringComparer.OrdinalIgnoreCase);
    }

    [Fact]
    public void ShouldHaveJsonValueOneOf_NumericValues_ShouldWork()
    {
        var json = @"{""value"": 2}";

        json.ShouldHaveJsonValueOneOf("/value", new[] { 1, 2, 3 });
    }

    [Fact]
    public void ShouldHaveJsonValueOneOf_MissingPointer_ShouldThrow()
    {
        var json = @"{""value"": 1}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueOneOf("/missing", new[] { 1 }));

        ex.Message.ShouldContain("No value found at JSON pointer '/missing'");
    }

    [Fact]
    public void ShouldHaveJsonValueOneOf_NullValue_ShouldWork()
    {
        var json = @"{""value"": null}";

        json.ShouldHaveJsonValueOneOf<string?>(
            "/value",
            new List<string?> { null, "a" });
    }

    [Fact]
    public void ShouldNotHaveJsonValueOneOf_WithNoMatch_ShouldNotThrow()
    {
        var json = @"{""value"": ""z""}";

        json.ShouldNotHaveJsonValueOneOf("/value", new[] { "a", "b" });
    }

    [Fact]
    public void ShouldNotHaveJsonValueOneOf_WithMatch_ShouldThrow()
    {
        var json = @"{""value"": ""b""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueOneOf("/value", new[] { "a", "b" }));

        ex.Message.ShouldContain("should be none of a, b");
    }

    [Fact]
    public void ShouldNotHaveJsonValueOneOf_MissingPointer_ShouldNotThrow()
    {
        var json = @"{""value"": 1}";

        json.ShouldNotHaveJsonValueOneOf("/missing", new[] { 1 });
    }
}
