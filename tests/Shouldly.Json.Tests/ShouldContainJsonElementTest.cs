namespace Shouldly;

public class ShouldContainJsonElementTest
{
    private const string NumbersJson = @"[1, 2, 3]";
    private const string UsersJson = @"{""users"": [{""name"": ""Ann"", ""age"": 30}, {""name"": ""Bob"", ""age"": 40}]}";

    [Fact]
    public void ShouldContainJsonElement_WithPresentPrimitive_ShouldNotThrow()
    {
        NumbersJson.ShouldContainJsonElement("", @"2");
    }

    [Fact]
    public void ShouldContainJsonElement_WithNumericEquivalent_ShouldNotThrow()
    {
        NumbersJson.ShouldContainJsonElement("", "2.0");
    }

    [Fact]
    public void ShouldContainJsonElement_WithPresentObject_ShouldIgnorePropertyOrder()
    {
        UsersJson.ShouldContainJsonElement("/users", @"{""age"": 30, ""name"": ""Ann""}");
    }

    [Fact]
    public void ShouldContainJsonElement_WithAbsentElement_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldContainJsonElement("", @"9"));

        ex.Message.ShouldContain("should contain the element '9'");
    }

    [Fact]
    public void ShouldContainJsonElement_ArrayOrderWithinElement_IsSignificant()
    {
        var json = @"{""pairs"": [[1, 2]]}";

        json.ShouldContainJsonElement("/pairs", @"[1, 2]");

        Should.Throw<ShouldAssertException>(() => json.ShouldContainJsonElement("/pairs", @"[2, 1]"));
    }

    [Fact]
    public void ShouldContainJsonElement_NullElement_ShouldMatchNullElement()
    {
        var json = @"[1, null, 3]";

        json.ShouldContainJsonElement("", "null");
    }

    [Fact]
    public void ShouldContainJsonElement_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""value"": 42}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldContainJsonElement("/value", @"42"));

        ex.Message.ShouldContain("Value at JSON pointer '/value' is not an array");
    }

    [Fact]
    public void ShouldContainJsonElement_MissingPointer_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => UsersJson.ShouldContainJsonElement("/nope", @"{}"));

        ex.Message.ShouldContain("No value found at JSON pointer '/nope'");
    }

    [Fact]
    public void ShouldContainJsonElement_InvalidExpectedElement_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldContainJsonElement("", "{invalid}"));

        ex.Message.ShouldContain("Invalid JSON");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("{invalid json}")]
    public void ShouldContainJsonElement_InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldContainJsonElement("", @"1"));
    }

    [Fact]
    public void ShouldContainJsonElement_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldContainJsonElement("", @"9", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void ShouldNotContainJsonElement_WithAbsentElement_ShouldNotThrow()
    {
        NumbersJson.ShouldNotContainJsonElement("", @"9");
    }

    [Fact]
    public void ShouldNotContainJsonElement_WithPresentElement_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => NumbersJson.ShouldNotContainJsonElement("", @"2"));

        ex.Message.ShouldContain("should not contain the element '2'");
        ex.Message.ShouldContain("matching indices: 1");
    }

    [Fact]
    public void ShouldNotContainJsonElement_ReportsEveryMatchingIndex()
    {
        var json = @"[1, 2, 1]";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotContainJsonElement("", @"1"));

        ex.Message.ShouldContain("matching indices: 0, 2");
    }
}
