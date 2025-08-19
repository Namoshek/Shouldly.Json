namespace Shouldly;

using System.Text.Json.Nodes;

public class JsonComparisonEngineObjectTest
{
    [Fact]
    public void CompareSemanticEquality_WithEmptyObjects_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{}");
        var expected = JsonNode.Parse("{}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithSameProperties_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentPropertyOrder_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        var expected = JsonNode.Parse("{\"age\": 30, \"name\": \"John\"}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithMissingProperty_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\"}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.MissingProperty);
        result.FirstDifference.Path.ShouldBe("/age");
        result.FirstDifference.ExpectedValue.ShouldBe("age");
    }

    [Fact]
    public void CompareSemanticEquality_WithExtraProperty_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        var expected = JsonNode.Parse("{\"name\": \"John\"}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ExtraProperty);
        result.FirstDifference.Path.ShouldBe("/age");
        result.FirstDifference.ActualValue.ShouldBe("age");
    }

    [Fact]
    public void CompareSemanticEquality_WithDifferentPropertyValue_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": 31}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/age");
    }

    [Fact]
    public void CompareSemanticEquality_WithNestedObjects_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"age\": 30}}");
        var expected = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"age\": 30}}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithNestedObjectDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"age\": 30}}");
        var expected = JsonNode.Parse("{\"person\": {\"name\": \"Jane\", \"age\": 30}}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/person/name");
    }

    [Fact]
    public void CompareSemanticEquality_WithNestedMissingProperty_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"person\": {\"name\": \"John\"}}");
        var expected = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"age\": 30}}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.MissingProperty);
        result.FirstDifference.Path.ShouldBe("/person/age");
    }

    [Fact]
    public void CompareSubtree_WithSubsetObject_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\"}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSubtree_WithExtraPropertyInActual_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"email\": \"john@example.com\"}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.MissingProperty);
        result.FirstDifference.Path.ShouldBe("/email");
    }

    [Fact]
    public void CompareSubtree_WithNestedSubset_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{\"person\": {\"name\": \"John\"}}");
        var expected = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"age\": 30}, \"active\": true}");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSubtree_WithNestedExtraProperty_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"email\": \"john@example.com\"}}");
        var expected = JsonNode.Parse("{\"person\": {\"name\": \"John\", \"age\": 30}}");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.MissingProperty);
        result.FirstDifference.Path.ShouldBe("/person/email");
    }

    [Fact]
    public void CompareSemanticEquality_WithComplexNestedStructure_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse(@"{
            ""user"": {
                ""profile"": {
                    ""name"": ""John"",
                    ""settings"": {
                        ""theme"": ""dark"",
                        ""notifications"": true
                    }
                },
                ""contacts"": {
                    ""email"": ""john@example.com""
                }
            }
        }");
        
        var expected = JsonNode.Parse(@"{
            ""user"": {
                ""contacts"": {
                    ""email"": ""john@example.com""
                },
                ""profile"": {
                    ""settings"": {
                        ""notifications"": true,
                        ""theme"": ""dark""
                    },
                    ""name"": ""John""
                }
            }
        }");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithDeepNestedDifference_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse(@"{
            ""user"": {
                ""profile"": {
                    ""settings"": {
                        ""theme"": ""dark""
                    }
                }
            }
        }");
        
        var expected = JsonNode.Parse(@"{
            ""user"": {
                ""profile"": {
                    ""settings"": {
                        ""theme"": ""light""
                    }
                }
            }
        }");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/user/profile/settings/theme");
    }

    [Fact]
    public void CompareSemanticEquality_WithSpecialCharactersInPropertyNames_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse("{\"prop~with/special\": \"value\"}");
        var expected = JsonNode.Parse("{\"prop~with/special\": \"different\"}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Path.ShouldBe("/prop~0with~1special");
    }

    [Fact]
    public void CompareSemanticEquality_WithNullPropertyValues_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"age\": null}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": null}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithNullVsValueProperty_ShouldReturnFailure()
    {
        var actual = JsonNode.Parse("{\"name\": \"John\", \"age\": null}");
        var expected = JsonNode.Parse("{\"name\": \"John\", \"age\": 30}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/age");
    }
}
