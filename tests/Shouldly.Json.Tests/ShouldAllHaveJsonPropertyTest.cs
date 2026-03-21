namespace Shouldly;

public class ShouldAllHaveJsonPropertyTest
{
    [Fact]
    public void AllElementsHaveProperty_ShouldNotThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""Jane""}]}";

        json.ShouldAllHaveJsonProperty("/users", "/name");
    }

    [Fact]
    public void AllElementsHaveNestedProperty_ShouldNotThrow()
    {
        var json = @"{""users"": [{""address"": {""city"": ""New York""}}, {""address"": {""city"": ""Boston""}}]}";

        json.ShouldAllHaveJsonProperty("/users", "/address/city");
    }

    [Fact]
    public void SomeElementsMissingProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""age"": 30}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllHaveJsonProperty("/users", "/name"));

        ex.Message.ShouldContain("failed at indices: 1");
    }

    [Fact]
    public void AllElementsMissingProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""age"": 20}, {""age"": 30}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllHaveJsonProperty("/users", "/name"));

        ex.Message.ShouldContain("failed at indices: 0, 1");
    }

    [Fact]
    public void EmptyArray_ShouldNotThrow()
    {
        var json = @"{""users"": []}";

        json.ShouldAllHaveJsonProperty("/users", "/name");
    }

    [Fact]
    public void PropertyWithNullValue_ShouldNotThrow()
    {
        var json = @"{""users"": [{""name"": null}, {""name"": ""Jane""}]}";

        json.ShouldAllHaveJsonProperty("/users", "/name");
    }

    [Fact]
    public void RootLevelArray_ShouldWork()
    {
        var json = @"[{""name"": ""John""}, {""name"": ""Jane""}]";

        json.ShouldAllHaveJsonProperty("", "/name");
    }

    [Fact]
    public void InvalidArrayPointer_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldAllHaveJsonProperty("/missing", "/name"));
    }

    [Fact]
    public void ArrayPointerToNonArray_ShouldThrow()
    {
        var json = @"{""user"": {""name"": ""John""}}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllHaveJsonProperty("/user", "/name"));

        ex.Message.ShouldContain("is not an array");
    }

    [Fact]
    public void InvalidJsonPointer_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldAllHaveJsonProperty("invalid pointer", "/name"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldAllHaveJsonProperty("/users", "/name"));
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""users"": [{""age"": 30}]}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllHaveJsonProperty("/users", "/name", customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}

public class ShouldAllNotHaveJsonPropertyTest
{
    [Fact]
    public void NoElementsHaveProperty_ShouldNotThrow()
    {
        var json = @"{""users"": [{""name"": ""John""}, {""name"": ""Jane""}]}";

        json.ShouldAllNotHaveJsonProperty("/users", "/secret");
    }

    [Fact]
    public void SomeElementsHaveProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""name"": ""John"", ""secret"": ""s1""}, {""name"": ""Jane""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonProperty("/users", "/secret"));

        ex.Message.ShouldContain("failed at indices: 0");
    }

    [Fact]
    public void AllElementsHaveProperty_ShouldThrow()
    {
        var json = @"{""users"": [{""secret"": ""s1""}, {""secret"": ""s2""}]}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldAllNotHaveJsonProperty("/users", "/secret"));

        ex.Message.ShouldContain("failed at indices: 0, 1");
    }

    [Fact]
    public void EmptyArray_ShouldNotThrow()
    {
        var json = @"{""users"": []}";

        json.ShouldAllNotHaveJsonProperty("/users", "/secret");
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""users"": [{""secret"": ""s1""}]}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldAllNotHaveJsonProperty("/users", "/secret", customMessage));

        ex.Message.ShouldContain(customMessage);
    }
}
