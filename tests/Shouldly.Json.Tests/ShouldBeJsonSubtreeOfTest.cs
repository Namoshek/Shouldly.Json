namespace Shouldly;

public class ShouldBeJsonSubtreeOfTest
{
    [Fact]
    public void SimpleObject_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{""name"": ""John""}";
        var fullSet = @"{""name"": ""John"", ""age"": 30}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void SimpleObject_WhenNotSubset_ShouldThrow()
    {
        var subset = @"{""name"": ""John"", ""age"": 31}";
        var fullSet = @"{""name"": ""John"", ""age"": 30}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void NestedObject_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{""person"": {""name"": ""John""}}";
        var fullSet = @"{""person"": {""name"": ""John"", ""age"": 30}, ""active"": true}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void ObjectArray_WhenSubset_ShouldNotThrow()
    {
        var subset = @"{""persons"": [{""name"": ""John""}]}";
        var fullSet = @"{""persons"": [{""name"": ""John"", ""age"": 30}], ""active"": true}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void Arrays_WhenEqual_ShouldNotThrow()
    {
        var subset = @"{""numbers"": [1, 2, 3]}";
        var fullSet = @"{""numbers"": [1, 2, 3], ""active"": true}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void Arrays_WhenDifferentLength_ShouldThrow()
    {
        var subset = @"{""numbers"": [1, 2]}";
        var fullSet = @"{""numbers"": [1, 2, 3]}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void Arrays_WhenDifferentOrder_ShouldThrow()
    {
        var subset = @"{""numbers"": [1, 3, 2]}";
        var fullSet = @"{""numbers"": [1, 2, 3]}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(fullSet));
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

        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(fullSet));
    }

    [Theory]
    [InlineData(null, null)]
    public void NullInputs_WhenEqual_ShouldNotThrow(string? json1, string? json2)
    {
        json1.ShouldBeJsonSubtreeOf(json2);
    }

    [Theory]
    [InlineData(null, "{}")]
    [InlineData("{}", null)]
    public void NullInputs_WhenNotEqual_ShouldThrow(string? json1, string? json2)
    {
        Should.Throw<ShouldAssertException>(() => json1.ShouldBeJsonSubtreeOf(json2));
    }

    [Fact]
    public void EmptyObject_ShouldBeSubtreeOfAnyObject()
    {
        var subset = @"{}";
        var fullSet = @"{""name"": ""John"", ""age"": 30}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void DifferentValueTypes_ShouldThrow()
    {
        var subset = @"{""value"": ""42""}";
        var fullSet = @"{""value"": 42}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(fullSet));
    }

    [Fact]
    public void ArrayWithObjects_WhenExactMatch_ShouldNotThrow()
    {
        var subset = @"{""items"": [{""id"": 1}, {""id"": 2}]}";
        var fullSet = @"{""items"": [{""id"": 1}, {""id"": 2}], ""total"": 2}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void NestedArrays_MustMatchExactly()
    {
        var subset = @"{""matrix"": [[1,2], [3,4]]}";
        var fullSet = @"{""matrix"": [[1,2], [3,4]], ""size"": 2}";
        var nonMatch = @"{""matrix"": [[1,2], [3,4,5]]}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(nonMatch));
    }

    [Fact]
    public void ObjectWithNullValue_ShouldMatchExactNull()
    {
        var subset = @"{""value"": null}";
        var fullSet = @"{""value"": null, ""other"": 42}";
        var nonMatch = @"{""value"": {}}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(nonMatch));
    }

    [Fact]
    public void DecimalPrecision_ShouldMatchExactly()
    {
        var subset = @"{""value"": 1.500}";
        var fullSet = @"{""value"": 1.500, ""other"": 42}";
        var nonMatch = @"{""value"": 1.5001}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(nonMatch));
    }

    [Fact]
    public void MixedTypeArray_MustMatchExactly()
    {
        var subset = @"{""mixed"": [1, ""text"", true, null]}";
        var fullSet = @"{""mixed"": [1, ""text"", true, null], ""other"": 42}";
        var nonMatch = @"{""mixed"": [1, ""text"", false, null]}";

        subset.ShouldBeJsonSubtreeOf(fullSet);
        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(nonMatch));
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

        subset.ShouldBeJsonSubtreeOf(fullSet);
    }

    [Fact]
    public void ArrayOfObjects_OrderMatters()
    {
        var subset = @"{""items"": [{""id"": 2}, {""id"": 1}]}";
        var fullSet = @"{""items"": [{""id"": 1}, {""id"": 2}]}";

        Should.Throw<ShouldAssertException>(() => subset.ShouldBeJsonSubtreeOf(fullSet));
    }
} 