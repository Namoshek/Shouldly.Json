namespace Shouldly;

public class ShouldHaveJsonArrayUniqueElementsTest
{
    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_WithUniqueElements_ShouldNotThrow()
    {
        "[1, 2, 3]".ShouldHaveJsonArrayUniqueElements();
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_WithDuplicates_ShouldThrow()
    {
        var json = @"[1, 2, 1, 3, 2]";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayUniqueElements());

        ex.Message.ShouldContain("duplicate elements at indices: 2, 4");
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_TreatsEquivalentNumbersAsDuplicates()
    {
        var json = @"[42, 42.0]";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayUniqueElements());

        ex.Message.ShouldContain("duplicate elements at indices: 1");
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_WithDuplicateObjects_ShouldThrow()
    {
        var json = @"{""items"": [{""a"": 1}, {""a"": 1}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayUniqueElements("/items"));

        ex.Message.ShouldContain("duplicate elements at indices: 1");
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_WithNestedArraysOrderSensitive()
    {
        "[[1, 2], [2, 1]]".ShouldHaveJsonArrayUniqueElements();
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_WithMultipleNulls_ShouldThrow()
    {
        var json = @"[null, null]";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayUniqueElements());

        ex.Message.ShouldContain("duplicate elements at indices: 1");
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_EmptyArray_ShouldNotThrow()
    {
        "[]".ShouldHaveJsonArrayUniqueElements();
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""value"": 42}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayUniqueElements("/value"));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void ShouldHaveJsonArrayUniqueElements_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => "[1, 1]".ShouldHaveJsonArrayUniqueElements("", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void ShouldNotHaveJsonArrayUniqueElements_WithDuplicates_ShouldNotThrow()
    {
        "[1, 1]".ShouldNotHaveJsonArrayUniqueElements();
    }

    [Fact]
    public void ShouldNotHaveJsonArrayUniqueElements_WithUniqueElements_ShouldThrow()
    {
        var json = @"{""items"": [1, 2, 3]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonArrayUniqueElements("/items"));

        ex.Message.ShouldContain("should have duplicate elements");
    }

    [Fact]
    public void ShouldNotHaveJsonArrayUniqueElements_EmptyArray_ShouldThrow()
    {
        Should.Throw<ShouldAssertException>(() => "[]".ShouldNotHaveJsonArrayUniqueElements());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void UniqueElements_InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonArrayUniqueElements());
    }
}
