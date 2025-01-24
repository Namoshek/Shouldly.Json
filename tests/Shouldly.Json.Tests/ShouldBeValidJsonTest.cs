namespace Shouldly;

public class ShouldBeValidJsonTest
{
    [Theory]
    [InlineData(@"{}")]
    [InlineData(@"[]")]
    [InlineData(@"""string""")]
    [InlineData(@"42")]
    [InlineData(@"42.5")]
    [InlineData(@"true")]
    [InlineData(@"false")]
    [InlineData(@"null")]
    public void SimpleValidValues_ShouldNotThrow(string json)
    {
        json.ShouldBeValidJson();
    }

    [Fact]
    public void ComplexObject_ShouldNotThrow()
    {
        var json = @"{
            ""string"": ""value"",
            ""number"": 42,
            ""decimal"": 42.5,
            ""boolean"": true,
            ""null"": null,
            ""array"": [1, 2, 3],
            ""object"": {
                ""nested"": ""value""
            }
        }";

        json.ShouldBeValidJson();
    }

    [Fact]
    public void ComplexArray_ShouldNotThrow()
    {
        var json = @"[
            ""string"",
            42,
            42.5,
            true,
            null,
            [1, 2, 3],
            {""key"": ""value""}
        ]";

        json.ShouldBeValidJson();
    }

    [Fact]
    public void DeepNestedStructure_ShouldNotThrow()
    {
        var json = @"{
            ""level1"": {
                ""level2"": {
                    ""level3"": {
                        ""array"": [
                            {""key"": ""value""},
                            [1, 2, 3],
                            null,
                            true,
                            42.5
                        ]
                    }
                }
            }
        }";

        json.ShouldBeValidJson();
    }

    [Theory]
    [InlineData(null, "String should be valid JSON")]
    [InlineData("", "String should be valid JSON")]
    [InlineData(" ", "String should be valid JSON")]
    [InlineData("{", "String should be valid JSON")]
    [InlineData("[", "String should be valid JSON")]
    [InlineData("{'invalid': 'quotes'}", "String should be valid JSON")]
    [InlineData(@"{""missing"": }", "String should be valid JSON")]
    [InlineData(@"{""trailing"": ""comma"",}", "String should be valid JSON")]
    [InlineData(@"[1,]", "String should be valid JSON")]
    public void InvalidJson_ShouldThrow(string? json, string expectedError)
    {
        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldBeValidJson());
        
        ex.Message.ShouldContain(expectedError);
    }

    [Fact]
    public void CustomMessage_ShouldBeIncludedInError()
    {
        var customMessage = "Custom validation message";
        var ex = Should.Throw<ShouldAssertException>(() => "foo".ShouldBeValidJson(customMessage));
        
        ex.Message.ShouldContain(customMessage);
    }

    [Theory]
    [InlineData(@"[1, 2, 3, ]")]
    [InlineData(@"{""key"": ""value"", }")]
    [InlineData(@"{key: ""value""}")]
    [InlineData(@"""unterminated string")]
    [InlineData(@"[1, 2, 3")]
    [InlineData(@"{""key"": value}")]
    public void CommonJsonSyntaxErrors_ShouldThrow(string invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldBeValidJson());
    }
}
