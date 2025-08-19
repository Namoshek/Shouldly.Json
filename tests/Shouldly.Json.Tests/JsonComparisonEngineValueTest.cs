namespace Shouldly;

using System.Text.Json.Nodes;

public class JsonComparisonEngineValueTest
{
    [Fact]
    public void CompareSemanticEquality_WithSameStrings_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("\"hello\"");
        var expected = JsonNode.Parse("\"hello\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentStrings_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("\"hello\"");
        var expected = JsonNode.Parse("\"world\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.ExpectedValue.ShouldBe("world");
        result.FirstDifference.ActualValue.ShouldBe("hello");
    }

    [Fact]
    public void CompareSemanticEquality_WithSameNumbers_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("42");
        var expected = JsonNode.Parse("42");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentNumbers_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("42");
        var expected = JsonNode.Parse("43");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
    }

    [Fact]
    public void CompareSemanticEquality_WithEquivalentNumbers_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("42.0");
        var expected = JsonNode.Parse("42");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithSameBooleans_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("true");
        var expected = JsonNode.Parse("true");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentBooleans_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("true");
        var expected = JsonNode.Parse("false");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.ExpectedValue.ShouldBe(false);
        result.FirstDifference.ActualValue.ShouldBe(true);
    }

    [Fact]
    public void CompareSemanticEquality_WithBothNull_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("null");
        var expected = JsonNode.Parse("null");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithNullVsValue_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("null");
        var expected = JsonNode.Parse("\"hello\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.ExpectedValue.ShouldNotBeNull();
        result.FirstDifference.ActualValue.ShouldBeNull();
    }

    [Fact]
    public void CompareSemanticEquality_WithStringVsNumber_ShouldReturnTypeMismatch()
    {
        var actual = JsonNode.Parse("\"42\"");
        var expected = JsonNode.Parse("42");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.TypeMismatch);
        result.FirstDifference.ExpectedType.ShouldBe("number");
        result.FirstDifference.ActualType.ShouldBe("string");
    }

    [Fact]
    public void CompareSemanticEquality_WithBooleanVsString_ShouldReturnTypeMismatch()
    {
        var actual = JsonNode.Parse("true");
        var expected = JsonNode.Parse("\"true\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.TypeMismatch);
        result.FirstDifference.ExpectedType.ShouldBe("string");
        result.FirstDifference.ActualType.ShouldBe("boolean");
    }

    [Fact]
    public void CompareSemanticEquality_WithSameDates_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("\"2023-01-01T00:00:00Z\"");
        var expected = JsonNode.Parse("\"2023-01-01T00:00:00Z\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentDates_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("\"2023-01-01T00:00:00Z\"");
        var expected = JsonNode.Parse("\"2023-01-02T00:00:00Z\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
    }

    [Fact]
    public void CompareSemanticEquality_WithEquivalentDatesInDifferentFormats_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("\"2023-01-01T00:00:00Z\"");
        var expected = JsonNode.Parse("\"2023-01-01T00:00:00+00:00\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Theory]
    [InlineData("42", "42.0")]
    [InlineData("42.0", "42")]
    [InlineData("42.00", "42.0")]
    [InlineData("0", "0.0")]
    [InlineData("-42", "-42.0")]
    public void CompareSemanticEquality_WithEquivalentNumericFormats_ShouldReturnSuccess(string actualJson, string expectedJson)
    {
        var actual = JsonNode.Parse(actualJson);
        var expected = JsonNode.Parse(expectedJson);
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Theory]
    [InlineData("42.1", "42.2")]
    [InlineData("42", "43")]
    [InlineData("-42", "42")]
    [InlineData("0", "1")]
    public void CompareSemanticEquality_WithNonEquivalentNumbers_ShouldReturnFailure(string actualJson, string expectedJson)
    {
        var actual = JsonNode.Parse(actualJson);
        var expected = JsonNode.Parse(expectedJson);
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
    }

    [Fact]
    public void CompareSemanticEquality_WithCustomPath_ShouldUseCorrectPath()
    {
        var actual = JsonNode.Parse("\"hello\"");
        var expected = JsonNode.Parse("\"world\"");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected, "/custom/path");

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Path.ShouldBe("/custom/path");
    }

    [Fact]
    public void CompareSemanticEquality_WithLargeNumbers_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse("999999999999999999");
        var expected = JsonNode.Parse("999999999999999999");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDecimalPrecision_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse("1.23456789");
        var expected = JsonNode.Parse("1.23456789");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentDecimalPrecision_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("1.23456789");
        var expected = JsonNode.Parse("1.23456788");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
    }
}
