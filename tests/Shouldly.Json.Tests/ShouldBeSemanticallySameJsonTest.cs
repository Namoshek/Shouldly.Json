namespace Shouldly;

public class ShouldBeSemanticallySameJsonTest
{
    [Fact]
    public void SimpleObjects_WithDifferentPropertyOrder_ShouldBeEqual()
    {
        var json1 = @"{""name"": ""John"", ""age"": 30}";
        var json2 = @"{""age"": 30, ""name"": ""John""}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void Arrays_WithSameOrder_ShouldBeEqual()
    {
        var json1 = @"[1, 2, 3, ""test""]";
        var json2 = @"[1, 2, 3, ""test""]";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void Arrays_WithDifferentOrder_ShouldNotBeEqual()
    {
        var json1 = @"[1, 2, 3]";
        var json2 = @"[1, 3, 2]";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeFalse();
    }

    [Fact]
    public void NestedObjects_WithDifferentPropertyOrder_ShouldBeEqual()
    {
        var json1 = @"{""person"": {""name"": ""John"", ""age"": 30}, ""active"": true}";
        var json2 = @"{""active"": true, ""person"": {""age"": 30, ""name"": ""John""}}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void ObjectsWithArrays_ShouldRespectArrayOrder()
    {
        var json1 = @"{""numbers"": [1, 2, 3], ""name"": ""test""}";
        var json2 = @"{""name"": ""test"", ""numbers"": [1, 2, 3]}";
        var json3 = @"{""name"": ""test"", ""numbers"": [1, 3, 2]}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Fact]
    public void ComplexNestedStructure_ShouldBeEqual()
    {
        var json1 = @"{
            ""array"": [
                {""id"": 1, ""value"": ""first""},
                {""value"": ""second"", ""id"": 2}
            ],
            ""nested"": {
                ""deep"": {
                    ""x"": 1,
                    ""y"": 2
                }
            }
        }";

        var json2 = @"{
            ""nested"": {
                ""deep"": {
                    ""y"": 2,
                    ""x"": 1
                }
            },
            ""array"": [
                {""value"": ""first"", ""id"": 1},
                {""id"": 2, ""value"": ""second""}
            ]
        }";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void DifferentTypes_ShouldNotBeEqual()
    {
        var json1 = @"{""value"": 123}";
        var json2 = @"{""value"": ""123""}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeFalse();
    }

    [Fact]
    public void NullValues_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""value"": null}";
        var json2 = @"{""value"": null}";
        var json3 = @"{""value"": ""null""}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldReturnFalse(string? invalidJson)
    {
        var validJson = @"{""test"": ""value""}";

        invalidJson.ShouldBeSemanticallySameJson(validJson).ShouldBeFalse();
        validJson.ShouldBeSemanticallySameJson(invalidJson).ShouldBeFalse();
    }

    [Fact]
    public void EmptyObjects_ShouldBeEqual()
    {
        var json1 = "{}";
        var json2 = "{}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void EmptyArrays_ShouldBeEqual()
    {
        var json1 = "[]";
        var json2 = "[]";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void DifferentSizedArrays_ShouldNotBeEqual()
    {
        var json1 = "[1, 2, 3]";
        var json2 = "[1, 2]";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeFalse();
    }

    [Fact]
    public void NumberFormat_ShouldNotMapperIfIntegerIsSameValueAsFloat_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""value"": 123.0}";
        var json2 = @"{""value"": 123.00}";
        var json3 = @"{""value"": 123}";
        
        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeTrue();
    }

    [Fact]
    public void NumberFormats_TrailingZerosShouldNotMatterAfterComma_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""value"": 123.230}";
        var json2 = @"{""value"": 123.23}";
        var json3 = @"{""value"": 123.2}";
        
        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Fact]
    public void BooleanValues_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""value"": true}";
        var json2 = @"{""value"": true}";
        var json3 = @"{""value"": false}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Theory]
    [InlineData(null, null, true)]  // Both null should be equal
    [InlineData(null, "{}", false)] // Null and empty object should not be equal
    [InlineData("{}", null, false)] // Empty object and null should not be equal
    [InlineData(null, "", false)]   // Null and empty string should not be equal
    [InlineData("", null, false)]   // Empty string and null should not be equal
    public void NullInputs_ShouldBeHandledGracefully(string? json1, string? json2, bool expectedResult)
    {
        json1.ShouldBeSemanticallySameJson(json2).ShouldBe(expectedResult);
    }

    [Fact]
    public void DecimalPrecision_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""value"": 1.500}";
        var json2 = @"{""value"": 1.5}";
        var json3 = @"{""value"": 1.501}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Fact]
    public void ComplexArraysWithObjects_ShouldBeHandledCorrectly()
    {
        var json1 = @"[{""id"": 1, ""values"": [1,2,3]}, {""id"": 2, ""values"": [4,5,6]}]";
        var json2 = @"[{""id"": 1, ""values"": [1,2,3]}, {""id"": 2, ""values"": [4,5,6]}]";
        var json3 = @"[{""id"": 1, ""values"": [1,2,3]}, {""id"": 2, ""values"": [4,6,5]}]";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Fact]
    public void WhitespaceAndFormatting_ShouldNotMatter()
    {
        var json1 = @"{""key"":""value""}";
        var json2 = @"{
            ""key"": ""value""
        }";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
    }

    [Fact]
    public void EmptyObjectVsNull_ShouldNotBeEqual()
    {
        var json1 = @"{""key"": {}}";
        var json2 = @"{""key"": null}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeFalse();
    }

    [Fact]
    public void EmptyArrayVsNull_ShouldNotBeEqual()
    {
        var json1 = @"{""key"": []}";
        var json2 = @"{""key"": null}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeFalse();
    }

    [Fact]
    public void DifferentNumericTypes_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""value"": 42}";
        var json2 = @"{""value"": 42.0}";
        var json3 = @"{""value"": ""42""}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }

    [Fact]
    public void NestedArraysWithMixedTypes_ShouldBeHandledCorrectly()
    {
        var json1 = @"{""array"": [1, ""text"", true, null, [1,2,3], {""key"": ""value""}]}";
        var json2 = @"{""array"": [1, ""text"", true, null, [1,2,3], {""key"": ""value""}]}";
        var json3 = @"{""array"": [1, ""text"", true, null, [1,3,2], {""key"": ""value""}]}";

        json1.ShouldBeSemanticallySameJson(json2).ShouldBeTrue();
        json1.ShouldBeSemanticallySameJson(json3).ShouldBeFalse();
    }
}
