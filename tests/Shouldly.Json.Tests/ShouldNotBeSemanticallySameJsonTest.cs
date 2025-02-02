namespace Shouldly;

public class ShouldNotBeSemanticallySameJsonTest
{
    [Theory]
    [InlineData(@"{""name"": ""John""}", @"{""name"": ""Jane""}")]
    [InlineData(@"{""age"": 30}", @"{""age"": 31}")]
    [InlineData(@"{""active"": true}", @"{""active"": false}")]
    [InlineData(@"[1, 2, 3]", @"[1, 3, 2]")]
    [InlineData(@"{""array"": [1, 2]}", @"{""array"": [2, 1]}")]
    [InlineData(@"{""nested"": {""value"": 1}}", @"{""nested"": {""value"": 2}}")]
    public void ShouldNotBeSemanticallySameJson_WithDifferentValues_ShouldNotThrow(string actual, string expected)
    {
        actual.ShouldNotBeSemanticallySameJson(expected);
    }

    [Theory]
    [InlineData(@"{""name"": ""John"", ""age"": 30}", @"{""age"": 30, ""name"": ""John""}")]
    [InlineData( @"{""nested"": {""a"": 1, ""b"": 2}}", @"{""nested"": {""b"": 2, ""a"": 1}}")]
    [InlineData(@"[1, 2, 3]", @"[1, 2, 3]")]
    [InlineData(@"""string""", @"""string""")]
    [InlineData(@"42", @"42")]
    [InlineData(@"true", @"true")]
    [InlineData(@"null", @"null")]
    public void ShouldNotBeSemanticallySameJson_WithSameValues_ShouldThrow(string actual, string expected)
    {
        Should.Throw<ShouldAssertException>(() => actual.ShouldNotBeSemanticallySameJson(expected));
    }

    [Theory]
    [InlineData(@"{""value"": 42.0}", @"{""value"": 42}")]
    [InlineData(@"{""value"": 42.00}", @"{""value"": 42.0}")]
    public void ShouldNotBeSemanticallySameJson_WithEquivalentNumbers_ShouldThrow(string actual, string expected)
    {
        Should.Throw<ShouldAssertException>(() => actual.ShouldNotBeSemanticallySameJson(expected));
    }

    [Theory]
    [InlineData(null, @"{""name"": ""John""}")]
    [InlineData(@"{""name"": ""John""}", null)]
    public void ShouldNotBeSemanticallySameJson_WithOneNull_ShouldNotThrow(string? actual, string? expected)
    {
        actual.ShouldNotBeSemanticallySameJson(expected);
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_WithBothNull_ShouldThrow()
    {
        string? actual = null;
        string? expected = null;

        Should.Throw<ShouldAssertException>(() => actual.ShouldNotBeSemanticallySameJson(expected));
    }

    [Theory]
    [InlineData(@"{""name"": missing_quotes}", @"{""name"": ""John""}")]
    [InlineData(@"{""name"": ""John""}", @"{""name"": missing_quotes}")]
    public void ShouldNotBeSemanticallySameJson_WithInvalidJson_ShouldThrow(string actual, string expected)
    {
        Should.Throw<ShouldAssertException>(() => actual.ShouldNotBeSemanticallySameJson(expected))
            .Message.ShouldContain("Invalid JSON");
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_WithCustomMessage_ShouldIncludeMessage()
    {
        var actual = @"{""name"": ""John""}";
        var expected = @"{""name"": ""John""}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() => actual.ShouldNotBeSemanticallySameJson(expected, customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Theory]
    [InlineData(@"{ ""name"": ""John"" }", @"{""name"":""John""}")]
    [InlineData(@"{""name"":""John""}", @"{\n  ""name"": ""John""\n}")]
    public void ShouldNotBeSemanticallySameJson_WithDifferentFormatting_ShouldThrow(string actual, string expected)
    {
        Should.Throw<ShouldAssertException>(() => actual.ShouldNotBeSemanticallySameJson(expected));
    }
}
