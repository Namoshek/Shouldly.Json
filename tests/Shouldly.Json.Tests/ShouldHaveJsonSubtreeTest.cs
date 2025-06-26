namespace Shouldly;

using System;

public class ShouldHaveJsonSubtreeTest
{
    [Fact]
    public void SimpleObject_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{""name"": ""John"", ""createdAt"": ""2025-06-26T10:48:32.3127234+02:00""}";
        var fullSet = @"{""name"": ""John"", ""age"": 30, ""createdAt"": ""2025-06-26T10:48:32.3127234+02:00""}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void SimpleObject_WhenNotSubset_ShouldThrow()
    {
        var subset = @"{""name"": ""John"", ""age"": 31, ""createdAt"": ""2025-06-26T10:48:32.3127234+02:00""}";
        var fullSet = @"{""name"": ""John"", ""age"": 30, ""createdAt"": ""2025-06-26T10:48:32.3127234+02:00""}";

        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(subset));
    }

    [Fact]
    public void SimpleObject_WhenDateFormatDifferentButDateIsSame_ShouldNotThrow()
    {
        var subset = @"{""createdAt"": ""2025-06-26T10:48:32.3127230+02:00""}";
        var fullSet = @"{""createdAt"": ""2025-06-26T10:48:32.312723+02:00""}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }
    
    [Fact]
    public void NestedObject_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{""person"": {""name"": ""John""}}";
        var fullSet = @"{""person"": {""name"": ""John"", ""age"": 30}, ""active"": true}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void ObjectArray_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{""persons"": [{""name"": ""John""}]}";
        var fullSet = @"{""persons"": [{""name"": ""John"", ""age"": 30}], ""active"": true}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void Arrays_WhenEqual_ShouldNotThrow()
    {
        var subset = @"{""numbers"": [1, 2, 3]}";
        var fullSet = @"{""numbers"": [1, 2, 3], ""active"": true}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void Arrays_WhenDifferentLength_ShouldThrow()
    {
        var subset = @"{""numbers"": [1, 2]}";
        var fullSet = @"{""numbers"": [1, 2, 3]}";

        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(subset));
    }

    [Fact]
    public void Arrays_WhenDifferentOrder_ShouldThrow()
    {
        var subset = @"{""numbers"": [1, 3, 2]}";
        var fullSet = @"{""numbers"": [1, 2, 3]}";

        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(subset));
    }

    [Fact]
    public void ComplexNestedStructure_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{
            ""person"": {
                ""name"": ""John"",
                ""contacts"": [
                    {""type"": ""email"", ""value"": ""john@example.com""}
                ]
            }
        }";

        var fullSet = @"{
            ""person"": {
                ""name"": ""John"",
                ""age"": 30,
                ""contacts"": [
                    {""type"": ""email"", ""value"": ""john@example.com""},
                    {""type"": ""phone"", ""value"": ""1234567890""}
                ]
            },
            ""active"": true
        }";

        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(subset));
    }

    [Theory]
    [InlineData(null, null)]
    public void NullInputs_WhenEqual_ShouldNotThrow(string? json1, string? json2)
    {
        json1.ShouldHaveJsonSubtree(json2);
    }

    [Theory]
    [InlineData(null, "{}")]
    [InlineData("{}", null)]
    public void NullInputs_WhenNotEqual_ShouldThrow(string? json1, string? json2)
    {
        Should.Throw<ShouldAssertException>(() => json1.ShouldHaveJsonSubtree(json2));
    }

    [Fact]
    public void EmptyObject_ShouldBeSubtreeOfAnyObject()
    {
        var subset = @"{}";
        var fullSet = @"{""name"": ""John"", ""age"": 30}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void DifferentValueTypes_ShouldThrow()
    {
        var subset = @"{""value"": ""42""}";
        var fullSet = @"{""value"": 42}";

        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(subset));
    }

    [Fact]
    public void ArrayWithObjects_WhenExactMatch_ShouldNotThrow()
    {
        var subset = @"{""items"": [{""id"": 1}, {""id"": 2}]}";
        var fullSet = @"{""items"": [{""id"": 1}, {""id"": 2}], ""total"": 2}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void NestedArrays_MustMatchExactly()
    {
        var subset = @"{""matrix"": [[1,2], [3,4]]}";
        var fullSet = @"{""matrix"": [[1,2], [3,4]], ""size"": 2}";
        var nonMatch = @"{""matrix"": [[1,2], [3,4,5]]}";

        fullSet.ShouldHaveJsonSubtree(subset);
        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(nonMatch));
    }

    [Fact]
    public void ObjectWithNullValue_ShouldMatchExactNull()
    {
        var subset = @"{""value"": null}";
        var fullSet = @"{""value"": null, ""other"": 42}";
        var nonMatch = @"{""value"": {}}";

        fullSet.ShouldHaveJsonSubtree(subset);
        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(nonMatch));
    }

    [Fact]
    public void DecimalPrecision_ShouldMatchExactly()
    {
        var subset = @"{""value"": 1.500}";
        var fullSet = @"{""value"": 1.500, ""other"": 42}";
        var nonMatch = @"{""value"": 1.5001}";

        fullSet.ShouldHaveJsonSubtree(subset);
        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(nonMatch));
    }

    [Fact]
    public void MixedTypeArray_MustMatchExactly()
    {
        var subset = @"{""mixed"": [1, ""text"", true, null]}";
        var fullSet = @"{""mixed"": [1, ""text"", true, null], ""other"": 42}";
        var nonMatch = @"{""mixed"": [1, ""text"", false, null]}";

        fullSet.ShouldHaveJsonSubtree(subset);
        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(nonMatch));
    }

    [Fact]
    public void DeepNestedStructure_ShouldMatchPartially()
    {
        var subset = @"{
            ""level1"": {
                ""level2"": {
                    ""level3"": {
                        ""value"": ""deep""
                    }
                }
            }
        }";

        var fullSet = @"{
            ""level1"": {
                ""level2"": {
                    ""level3"": {
                        ""value"": ""deep"",
                        ""extra"": true
                    },
                    ""sibling"": 42
                },
                ""other"": ""value""
            },
            ""top"": ""level""
        }";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void ArrayOfObjects_OrderMatters()
    {
        var subset = @"{""items"": [{""id"": 2}, {""id"": 1}]}";
        var fullSet = @"{""items"": [{""id"": 1}, {""id"": 2}]}";

        Should.Throw<ShouldAssertException>(() => fullSet.ShouldHaveJsonSubtree(subset));
    }

    [Fact]
    public void NumericTypes_ShouldWork()
    {
        var subset = @"{""byte"": 255, ""short"": 32767}";
        var fullSet = @"{""byte"": 255, ""short"": 32767, ""int"": 2147483647, ""active"": true}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void CharacterValues_ShouldWork()
    {
        var subset = @"{""char"": ""A"", ""specialChar"": ""\n""}";
        var fullSet = @"{""char"": ""A"", ""specialChar"": ""\n"", ""unicodeChar"": ""€"", ""active"": true}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void GuidValues_ShouldWork()
    {
        var guid = Guid.NewGuid();
        var subset = $@"{{""id"": ""{guid}""}}";
        var fullSet = $@"{{""id"": ""{guid}"", ""name"": ""test"", ""active"": true}}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void TimeSpanValues_ShouldWork()
    {
        var timeSpan = TimeSpan.FromHours(2);
        var subset = $@"{{""duration"": ""{timeSpan}""}}";
        var fullSet = $@"{{""duration"": ""{timeSpan}"", ""name"": ""task"", ""completed"": false}}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void NullableValues_ShouldWork()
    {
        var subset = @"{""nullInt"": null, ""hasValue"": 42}";
        var fullSet = @"{""nullInt"": null, ""hasValue"": 42, ""name"": ""test"", ""active"": true}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }

    [Fact]
    public void MixedDataTypes_ShouldWork()
    {
        var guid = Guid.NewGuid();
        var timeSpan = TimeSpan.FromMinutes(30);
        var subset = $@"{{
            ""id"": ""{guid}"",
            ""byte"": 128,
            ""char"": ""X"",
            ""duration"": ""{timeSpan}""
        }}";
        var fullSet = $@"{{
            ""id"": ""{guid}"",
            ""byte"": 128,
            ""char"": ""X"",
            ""duration"": ""{timeSpan}"",
            ""name"": ""complex"",
            ""active"": true,
            ""score"": 95.5
        }}";

        fullSet.ShouldHaveJsonSubtree(subset);
    }
}
