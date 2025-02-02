namespace Shouldly;

public class ShouldNotBeJsonSubtreeOfTest
{
    [Fact]
    public void SimpleObject_WhenSemanticallyEqual_ShouldThrow()
    {
        var subset = @"{""name"": ""John""}";
        var fullSet = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldNotBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void SimpleObject_WhenSubset_ShouldThrow()
    {
        var subset = @"{""name"": ""John""}";
        var fullSet = @"{""name"": ""John"", ""age"": 30}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldNotBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void SimpleObject_WhenNotSubset_ShouldNotThrow()
    {
        var subset = @"{""name"": ""John"", ""age"": 31}";
        var fullSet = @"{""name"": ""John"", ""age"": 30}";

        subset.ShouldNotBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void NestedObject_WhenSubset_ShouldThrow()
    {
        var subset = @"{""person"": {""name"": ""John""}}";
        var fullSet = @"{""person"": {""name"": ""John"", ""age"": 30}, ""active"": true}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldNotBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void Arrays_WhenEqual_ShouldThrow()
    {
        var subset = @"{""numbers"": [1, 2, 3]}";
        var fullSet = @"{""numbers"": [1, 2, 3], ""active"": true}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldNotBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void Arrays_WhenDifferent_ShouldNotThrow()
    {
        var subset = @"{""numbers"": [1, 2, 4]}";
        var fullSet = @"{""numbers"": [1, 2, 3]}";

        subset.ShouldNotBeJsonSubtreeOf(fullSet);
    }

    [Theory]
    [InlineData(null, null)]
    public void NullInputs_WhenEqual_ShouldThrow(string? json1, string? json2)
    {
        Should.Throw<ShouldAssertException>(() => json1.ShouldNotBeJsonSubtreeOf(json2));
    }

    [Theory]
    [InlineData(null, "{}")]
    [InlineData("{}", null)]
    public void NullInputs_WhenNotEqual_ShouldNotThrow(string? json1, string? json2)
    {
        json1.ShouldNotBeJsonSubtreeOf(json2);
    }

    [Fact]
    public void InvalidJson_ShouldThrow()
    {
        var invalidJson = @"{""name"": }";
        var validJson = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldNotBeJsonSubtreeOf(validJson));
        Should.Throw<ShouldAssertException>(() => validJson.ShouldNotBeJsonSubtreeOf(invalidJson));
    }

    [Fact]
    public void DifferentTypes_ShouldNotThrow()
    {
        var json1 = @"{""value"": ""42""}";
        var json2 = @"{""value"": 42}";

        json1.ShouldNotBeJsonSubtreeOf(json2);
    }

    [Fact]
    public void ArraysWithDifferentOrder_ShouldNotThrow()
    {
        var json1 = @"{""numbers"": [1, 3, 2]}";
        var json2 = @"{""numbers"": [1, 2, 3]}";

        json1.ShouldNotBeJsonSubtreeOf(json2);
    }

    [Fact]
    public void ComplexNestedStructure_WhenDifferent_ShouldNotThrow()
    {
        var json1 = @"{
            ""person"": {
                ""name"": ""John"",
                ""contacts"": [
                    {""type"": ""email"", ""value"": ""different@example.com""}
                ]
            }
        }";

        var json2 = @"{
            ""person"": {
                ""name"": ""John"",
                ""contacts"": [
                    {""type"": ""email"", ""value"": ""john@example.com""}
                ]
            }
        }";

        json1.ShouldNotBeJsonSubtreeOf(json2);
    }
}
