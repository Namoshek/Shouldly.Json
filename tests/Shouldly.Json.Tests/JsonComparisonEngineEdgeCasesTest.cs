namespace Shouldly;

using System.Linq;
using System.Text.Json.Nodes;
using System.Text;

public class JsonComparisonEngineEdgeCasesTest
{
    [Fact]
    public void CompareSemanticEquality_WithDeeplyNestedStructure_ShouldHandleCorrectly()
    {
        var depth = 50;
        var actualJson = CreateDeeplyNestedJson(depth, "value1");
        var expectedJson = CreateDeeplyNestedJson(depth, "value2");
        
        var actual = JsonNode.Parse(actualJson);
        var expected = JsonNode.Parse(expectedJson);
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        
        // The path should reflect the deep nesting
        var expectedPath = string.Join("", Enumerable.Repeat("/level", depth)) + "/value";
        result.FirstDifference.Path.ShouldBe(expectedPath);
    }

    [Fact]
    public void CompareSemanticEquality_WithLargeArrays_ShouldHandleCorrectly()
    {
        var size = 1000;
        var actualArray = Enumerable.Range(0, size).ToArray();
        var expectedArray = Enumerable.Range(0, size).ToArray();
        expectedArray[500] = 9999; // Change one element
        
        var actual = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(actualArray));
        var expected = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(expectedArray));
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/500");
    }

    [Fact]
    public void CompareSemanticEquality_WithEmptyObjects_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("{}");
        var expected = JsonNode.Parse("{}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithEmptyArrays_ShouldReturnSuccess()
    {
        var actual = JsonNode.Parse("[]");
        var expected = JsonNode.Parse("[]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithEmptyObjectVsEmptyArray_ShouldReturnTypeMismatch()
    {
        var actual = JsonNode.Parse("{}");
        var expected = JsonNode.Parse("[]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.TypeMismatch);
        result.FirstDifference.ExpectedType.ShouldBe("array");
        result.FirstDifference.ActualType.ShouldBe("object");
    }

    [Fact]
    public void CompareSemanticEquality_WithVeryLongStrings_ShouldHandleCorrectly()
    {
        var longString1 = new string('a', 10000);
        var longString2 = new string('a', 9999) + 'b';
        
        var actual = JsonNode.Parse($"{{\"text\": \"{longString1}\"}}");
        var expected = JsonNode.Parse($"{{\"text\": \"{longString2}\"}}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/text");
    }

    [Fact]
    public void CompareSemanticEquality_WithSpecialCharactersInStrings_ShouldHandleCorrectly()
    {
        var specialChars = "\\\"\\\\\\n\\r\\t";
        var actual = JsonNode.Parse($"{{\"special\": \"{specialChars}\"}}");
        var expected = JsonNode.Parse($"{{\"special\": \"{specialChars}\"}}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithUnicodeCharacters_ShouldHandleCorrectly()
    {
        var unicodeText = "Hello 世界 🌍 Ñoël";
        var actual = JsonNode.Parse($"{{\"unicode\": \"{unicodeText}\"}}");
        var expected = JsonNode.Parse($"{{\"unicode\": \"{unicodeText}\"}}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithExtremeNumbers_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse("{\"max\": 1.7976931348623157E+308, \"min\": -1.7976931348623157E+308}");
        var expected = JsonNode.Parse("{\"max\": 1.7976931348623157E+308, \"min\": -1.7976931348623157E+308}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithMixedNullAndValues_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse("{\"a\": null, \"b\": \"value\", \"c\": null}");
        var expected = JsonNode.Parse("{\"a\": null, \"b\": null, \"c\": null}");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Type.ShouldBe(JsonDifferenceType.ValueMismatch);
        result.FirstDifference.Path.ShouldBe("/b");
    }

    [Fact]
    public void CompareSemanticEquality_WithComplexMixedTypes_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse(@"{
            ""string"": ""text"",
            ""number"": 42,
            ""boolean"": true,
            ""null"": null,
            ""array"": [1, ""two"", false, null],
            ""object"": {""nested"": ""value""}
        }");
        
        var expected = JsonNode.Parse(@"{
            ""object"": {""nested"": ""value""},
            ""array"": [1, ""two"", false, null],
            ""null"": null,
            ""boolean"": true,
            ""number"": 42,
            ""string"": ""text""
        }");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSubtree_WithComplexNestedSubset_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse(@"{
            ""user"": {
                ""profile"": {
                    ""name"": ""John""
                }
            }
        }");
        
        var expected = JsonNode.Parse(@"{
            ""user"": {
                ""profile"": {
                    ""name"": ""John"",
                    ""age"": 30,
                    ""settings"": {
                        ""theme"": ""dark""
                    }
                },
                ""permissions"": [""read"", ""write""]
            },
            ""system"": {
                ""version"": ""1.0""
            }
        }");
        
        var result = JsonComparisonEngine.CompareSubtree(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithPropertyNameEdgeCases_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse(@"{
            """": ""empty key"",
            ""with spaces"": ""value1"",
            ""with-dashes"": ""value2"",
            ""with_underscores"": ""value3"",
            ""with.dots"": ""value4"",
            ""with/slashes"": ""value5"",
            ""with~tildes"": ""value6""
        }");
        
        var expected = JsonNode.Parse(@"{
            ""with~tildes"": ""value6"",
            ""with/slashes"": ""value5"",
            ""with.dots"": ""value4"",
            ""with_underscores"": ""value3"",
            ""with-dashes"": ""value2"",
            ""with spaces"": ""value1"",
            """": ""empty key""
        }");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    [Fact]
    public void CompareSemanticEquality_WithArrayOfComplexObjects_ShouldHandleCorrectly()
    {
        var actual = JsonNode.Parse(@"[
            {
                ""id"": 1,
                ""data"": {
                    ""values"": [1, 2, 3],
                    ""metadata"": {""type"": ""test""}
                }
            },
            {
                ""id"": 2,
                ""data"": {
                    ""values"": [4, 5, 6],
                    ""metadata"": {""type"": ""prod""}
                }
            }
        ]");
        
        var expected = JsonNode.Parse(@"[
            {
                ""data"": {
                    ""metadata"": {""type"": ""test""},
                    ""values"": [1, 2, 3]
                },
                ""id"": 1
            },
            {
                ""data"": {
                    ""metadata"": {""type"": ""prod""},
                    ""values"": [4, 5, 6]
                },
                ""id"": 2
            }
        ]");
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);

        result.IsEqual.ShouldBeTrue();
    }

    private static string CreateDeeplyNestedJson(int depth, string finalValue)
    {
        var sb = new StringBuilder();
        
        for (int i = 0; i < depth; i++)
        {
            sb.Append("{\"level\":");
        }
        
        sb.Append($"{{\"value\":\"{finalValue}\"}}");
        
        for (int i = 0; i < depth; i++)
        {
            sb.Append("}");
        }
        
        return sb.ToString();
    }
}
