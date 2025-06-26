namespace Shouldly;

using System;

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

    [Theory]
    [InlineData(@"{""byte"": 255}", @"{""byte"": 254}")]
    [InlineData(@"{""short"": 32767}", @"{""short"": 32766}")]
    [InlineData(@"{""sbyte"": -128}", @"{""sbyte"": -127}")]
    [InlineData(@"{""ushort"": 65535}", @"{""ushort"": 65534}")]
    public void ShouldNotBeSemanticallySameJson_WithDifferentNumericTypes_ShouldNotThrow(string actual, string expected)
    {
        actual.ShouldNotBeSemanticallySameJson(expected);
    }

    [Theory]
    [InlineData(@"{""char"": ""A""}", @"{""char"": ""B""}")]
    [InlineData(@"{""char"": ""A""}", @"{""char"": 65}")]  // ASCII value
    [InlineData(@"{""char"": ""\n""}", @"{""char"": ""\t""}")]
    public void ShouldNotBeSemanticallySameJson_WithDifferentCharValues_ShouldNotThrow(string actual, string expected)
    {
        actual.ShouldNotBeSemanticallySameJson(expected);
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_WithDifferentGuids_ShouldNotThrow()
    {
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();
        var json1 = $@"{{""guid"": ""{guid1}""}}";
        var json2 = $@"{{""guid"": ""{guid2}""}}";

        json1.ShouldNotBeSemanticallySameJson(json2);
    }

    [Fact] 
    public void ShouldNotBeSemanticallySameJson_WithDifferentTimeSpans_ShouldNotThrow()
    {
        var timeSpan1 = TimeSpan.FromHours(1);
        var timeSpan2 = TimeSpan.FromHours(2);
        var json1 = $@"{{""timespan"": ""{timeSpan1}""}}";
        var json2 = $@"{{""timespan"": ""{timeSpan2}""}}";

        json1.ShouldNotBeSemanticallySameJson(json2);
    }

    [Theory]
    [InlineData(@"{""value"": null}", @"{""value"": 0}")]
    [InlineData(@"{""value"": null}", @"{""value"": false}")]
    [InlineData(@"{""value"": null}", @"{""value"": """"}")]
    public void ShouldNotBeSemanticallySameJson_WithNullVsOtherTypes_ShouldNotThrow(string actual, string expected)
    {
        actual.ShouldNotBeSemanticallySameJson(expected);
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_WithStringifiedNumbers_ShouldNotThrow()
    {
        var json1 = @"{""byte"": 255, ""short"": 32767}";
        var json2 = @"{""byte"": ""255"", ""short"": ""32767""}";

        json1.ShouldNotBeSemanticallySameJson(json2);
    }

    [Fact]
    public void ShouldNotBeSemanticallySameJson_WithGuidVsString_ShouldNotThrow()
    {
        var guid = Guid.NewGuid();
        var json1 = $@"{{""id"": ""{guid}""}}";
        var json2 = @"{""id"": ""not-a-guid""}";

        json1.ShouldNotBeSemanticallySameJson(json2);
    }
}
