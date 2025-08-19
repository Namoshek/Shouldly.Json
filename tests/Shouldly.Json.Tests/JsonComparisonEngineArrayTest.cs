namespace Shouldly;

using System.Text.Json.Nodes;

public class JsonComparisonEngineArrayTest
{
    [Fact]
    public void CompareSemanticEquality_WithEmptyArrays_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[]");
        var expected = JsonNode.Parse("[]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithSameArrays_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[1, 2, 3]");
        var expected = JsonNode.Parse("[1, 2, 3]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentArrayLengths_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[1, 2]");
        var expected = JsonNode.Parse("[1, 2, 3]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ArrayLengthMismatch);
        result.FirstDifference.Path.ShouldBe("");
        result.FirstDifference.ExpectedValue.ShouldBe(3);
        result.FirstDifference.ActualValue.ShouldBe(2);
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentArrayOrder_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[1, 3, 2]");
        var expected = JsonNode.Parse("[1, 2, 3]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/1");
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentElementAtIndex_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[1, 2, 4]");
        var expected = JsonNode.Parse("[1, 2, 3]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/2");
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayOfObjects_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[{\"name\": \"John\"}, {\"name\": \"Jane\"}]");
        var expected = JsonNode.Parse("[{\"name\": \"John\"}, {\"name\": \"Jane\"}]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayOfObjectsDifferentOrder_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[{\"name\": \"Jane\"}, {\"name\": \"John\"}]");
        var expected = JsonNode.Parse("[{\"name\": \"John\"}, {\"name\": \"Jane\"}]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/0/name");
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayOfObjectsPropertyDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[{\"name\": \"John\", \"age\": 30}]");
        var expected = JsonNode.Parse("[{\"name\": \"John\", \"age\": 31}]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/0/age");
    }

    [Fact]
    public void CompareSemanticEquality_WithNestedArrays_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[[1, 2], [3, 4]]");
        var expected = JsonNode.Parse("[[1, 2], [3, 4]]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithNestedArrayDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[[1, 2], [3, 5]]");
        var expected = JsonNode.Parse("[[1, 2], [3, 4]]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/1/1");
    }

    [Fact]
    public void CompareSemanticEquality_WithMixedTypeArray_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[1, \"text\", true, null, {\"key\": \"value\"}]");
        var expected = JsonNode.Parse("[1, \"text\", true, null, {\"key\": \"value\"}]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithMixedTypeArrayDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[1, \"text\", false, null]");
        var expected = JsonNode.Parse("[1, \"text\", true, null]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/2");
    }

    [Fact]
    public void CompareSubtree_WithSameArrays_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[1, 2, 3]");
        var expected = JsonNode.Parse("[1, 2, 3]");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSubtree_WithDifferentArrayLengths_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[1, 2]");
        var expected = JsonNode.Parse("[1, 2, 3]");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ArrayLengthMismatch);
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayInObject_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{\"numbers\": [1, 2, 3]}");
        var expected = JsonNode.Parse("{\"numbers\": [1, 2, 3]}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayInObjectDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"numbers\": [1, 2, 4]}");
        var expected = JsonNode.Parse("{\"numbers\": [1, 2, 3]}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/numbers/2");
    }

    [Fact]
    public void CompareSemanticEquality_WithComplexNestedStructure_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse(@"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""123-456-7890""}
                    ]
                }
            ]
        }");
        
        var expected = JsonNode.Parse(@"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""123-456-7890""}
                    ]
                }
            ]
        }");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithComplexNestedStructureDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse(@"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""123-456-7890""}
                    ]
                }
            ]
        }");
        
        var expected = JsonNode.Parse(@"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""987-654-3210""}
                    ]
                }
            ]
        }");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/users/0/contacts/1/value");
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayOfObjectsWithSubsetProperties_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("[{\"name\": \"John\"}]");
        var expected = JsonNode.Parse("[{\"name\": \"John\", \"age\": 30}]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.MissingProperty);
        result.FirstDifference.Path.ShouldBe("/0/age");
    }

    [Fact]
    public void CompareSubtree_WithArrayOfObjectsWithSubsetProperties_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[{\"name\": \"John\"}]");
        var expected = JsonNode.Parse("[{\"name\": \"John\", \"age\": 30}]");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithCustomPath_ShouldUseCorrectPath()
    {
        var actual = JsonNode.Parse("[1, 2]");
        var expected = JsonNode.Parse("[1, 3]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected, "/custom/path");

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Path.ShouldBe("/custom/path/1");
    }
}
