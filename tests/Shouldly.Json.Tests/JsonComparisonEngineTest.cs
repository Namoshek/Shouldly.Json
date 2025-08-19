namespace Shouldly;

using System.Text.Json.Nodes;

public class JsonComparisonEngineTest
{
    [Fact]
    public void CompareSemanticEquality_WithBothNull_ShouldReturnSuccess()
    {
        var result = JsonComparisonEngine.CompareSemanticEquality(null, null);

        result.IsEqual.ShouldBeTrue();
        result.FirstDifference.ShouldBeNull();
    }

    [Fact]
    public void CompareSemanticEquality_WithOneNull_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{}");
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, null);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("");
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentTypes_ShouldReturnTypeMismatch()
    {
        var actual = JsonNode.Parse("42");
        var expected = JsonNode.Parse("\"42\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.TypeMismatch);
        result.FirstDifference.ExpectedType.ShouldBe("string");
        result.FirstDifference.ActualType.ShouldBe("number");
        result.FirstDifference.Path.ShouldBe("");
    }

    [Fact]
    public void CompareSubtree_WithBothNull_ShouldReturnSuccess()
    {
        var result = JsonComparisonEngine.CompareSubtree(null, null);

        result.IsEqual.ShouldBeTrue();
        result.FirstDifference.ShouldBeNull();
    }

    [Fact]
    public void CompareSubtree_WithOneNull_ShouldReturnFailure()
    {
        var expected = JsonNode.Parse("{}");
        var result = JsonComparisonEngine.CompareSubtree(null, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
    }

    [Fact]
    public void CompareSemanticEquality_WithSameObjects_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{}");
        var expected = JsonNode.Parse("{}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithSameArrays_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[]");
        var expected = JsonNode.Parse("[]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithSameValues_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("42");
        var expected = JsonNode.Parse("42");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithCustomPath_ShouldUseCorrectPath()
    {
        var actual = JsonNode.Parse("42");
        var expected = JsonNode.Parse("\"42\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected, "/custom/path");

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Path.ShouldBe("/custom/path");
    }

    [Theory]
    [InlineData("true", "boolean")]
    [InlineData("false", "boolean")]
    [InlineData("null", "null")]
    [InlineData("42", "number")]
    [InlineData("\"text\"", "string")]
    [InlineData("{}", "object")]
    [InlineData("[]", "array")]
    public void CompareSemanticEquality_TypeMismatch_ShouldReportCorrectTypes(string jsonValue, string expectedTypeName)
    {
        var actual = JsonNode.Parse("42"); // Always number
        var expected = JsonNode.Parse(jsonValue);
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        if (jsonValue == "42") // Same type, should succeed
        {
            result.IsEqual.ShouldBeTrue();
        }
        else if (jsonValue == "null") // null vs number is a value mismatch, not type mismatch
        {
            result.IsEqual.ShouldBeFalse();
            result.FirstDifference.ShouldNotBeNull();
            result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        }
        else
        {
            result.IsEqual.ShouldBeFalse();
            result.FirstDifference.ShouldNotBeNull();
            result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.TypeMismatch);
            result.FirstDifference.ExpectedType.ShouldBe(expectedTypeName);
            result.FirstDifference.ActualType.ShouldBe("number");
        }
    }
}
