namespace Shouldly;

public class ShouldBeJsonArrayEmptyTest
{
    [Fact]
    public void ShouldBeJsonArrayEmpty_WithEmptyRootArray_ShouldNotThrow()
    {
        "[]".ShouldBeJsonArrayEmpty();
    }

    [Fact]
    public void ShouldBeJsonArrayEmpty_WithEmptyNestedArray_ShouldNotThrow()
    {
        var json = @"{""items"": []}";

        json.ShouldBeJsonArrayEmpty("/items");
    }

    [Fact]
    public void ShouldBeJsonArrayEmpty_WithNonEmptyArray_ShouldThrow()
    {
        var json = @"{""items"": [1]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonArrayEmpty("/items"));

        ex.Message.ShouldContain("should be empty");
    }

    [Fact]
    public void ShouldBeJsonArrayEmpty_NonArrayPointer_ShouldThrow()
    {
        var json = @"{""items"": 1}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldBeJsonArrayEmpty("/items"));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void ShouldBeJsonArrayNotEmpty_WithNonEmptyArray_ShouldNotThrow()
    {
        "[1, 2]".ShouldBeJsonArrayNotEmpty();
    }

    [Fact]
    public void ShouldBeJsonArrayNotEmpty_WithEmptyArray_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "[]".ShouldBeJsonArrayNotEmpty());

        ex.Message.ShouldContain("should have at least one element");
    }

    [Fact]
    public void ShouldNotBeJsonArrayEmpty_WithNonEmptyArray_ShouldNotThrow()
    {
        "[1]".ShouldNotBeJsonArrayEmpty();
    }

    [Fact]
    public void ShouldNotBeJsonArrayEmpty_WithEmptyArray_ShouldThrow()
    {
        var json = @"{""items"": []}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotBeJsonArrayEmpty("/items"));

        ex.Message.ShouldContain("should not be empty");
    }

    [Fact]
    public void ShouldHaveJsonArrayCountBetween_WithinRange_ShouldNotThrow()
    {
        "[1, 2, 3]".ShouldHaveJsonArrayCountBetween(2, 4);
    }

    [Theory]
    [InlineData(4, 6)]
    [InlineData(0, 2)]
    public void ShouldHaveJsonArrayCountBetween_OutsideRange_ShouldThrow(int min, int max)
    {
        var ex = Should.Throw<ShouldAssertException>(() => "[1, 2, 3]".ShouldHaveJsonArrayCountBetween(min, max));

        ex.Message.ShouldContain($"should have between {min} and {max} elements");
    }

    [Fact]
    public void ShouldHaveJsonArrayCountBetween_BoundariesAreInclusive()
    {
        "[1, 2, 3]".ShouldHaveJsonArrayCountBetween(3, 3);
    }

    [Fact]
    public void ShouldHaveJsonArrayCountBetween_WithPointer_ShouldNotThrow()
    {
        var json = @"{""items"": [1, 2, 3]}";

        json.ShouldHaveJsonArrayCountBetween(1, 5, "/items");
    }

    [Fact]
    public void ShouldNotHaveJsonArrayCountBetween_OutsideRange_ShouldNotThrow()
    {
        "[1, 2, 3]".ShouldNotHaveJsonArrayCountBetween(4, 6);
    }

    [Fact]
    public void ShouldNotHaveJsonArrayCountBetween_WithinRange_ShouldThrow()
    {
        var ex = Should.Throw<ShouldAssertException>(() => "[1, 2, 3]".ShouldNotHaveJsonArrayCountBetween(2, 4));

        ex.Message.ShouldContain("should not have between 2 and 4 elements");
    }

    [Fact]
    public void ArrayCountBetween_MissingPointer_ShouldThrow()
    {
        var json = @"{""items"": [1]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonArrayCountBetween(1, 2, "/nope"));

        ex.Message.ShouldContain("No value found at JSON pointer '/nope'");
    }

    [Fact]
    public void ArrayCountBetween_CustomMessage_ShouldBeIncluded()
    {
        var customMessage = "Custom error";

        var ex = Should.Throw<ShouldAssertException>(() => "[1]".ShouldHaveJsonArrayCountBetween(2, 3, "", customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
