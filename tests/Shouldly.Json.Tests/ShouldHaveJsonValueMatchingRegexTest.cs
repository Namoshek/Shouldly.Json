namespace Shouldly;

using System;

public class ShouldHaveJsonValueMatchingRegexTest
{
    [Fact]
    public void SimpleRegexMatch_ShouldWork()
    {
        var json = @"{""name"": ""John"", ""email"": ""john@example.com""}";

        json.ShouldHaveJsonValueMatchingRegex("/name", @"^[A-Z][a-z]+$");
        json.ShouldHaveJsonValueMatchingRegex("/email", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    [Fact]
    public void NestedPropertyRegexMatch_ShouldWork()
    {
        var json = @"{
            ""person"": {
                ""name"": ""John"",
                ""contact"": {
                    ""phone"": ""555-1234""
                }
            }
        }";

        json.ShouldHaveJsonValueMatchingRegex("/person/name", @"^[A-Z][a-z]+$");
        json.ShouldHaveJsonValueMatchingRegex("/person/contact/phone", @"^\d{3}-\d{4}$");
    }

    [Fact]
    public void ArrayElementRegexMatch_ShouldWork()
    {
        var json = @"{""emails"": [""john@example.com"", ""jane@test.org""]}";

        json.ShouldHaveJsonValueMatchingRegex("/emails/0", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        json.ShouldHaveJsonValueMatchingRegex("/emails/1", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    [Fact]
    public void ComplexPathRegexMatch_ShouldWork()
    {
        var json = @"{
            ""users"": [
                {
                    ""name"": ""John"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""john@example.com""},
                        {""type"": ""phone"", ""value"": ""555-1234""}
                    ]
                }
            ]
        }";

        json.ShouldHaveJsonValueMatchingRegex("/users/0/name", @"^[A-Z][a-z]+$");
        json.ShouldHaveJsonValueMatchingRegex("/users/0/contacts/0/value", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        json.ShouldHaveJsonValueMatchingRegex("/users/0/contacts/1/value", @"^\d{3}-\d{4}$");
    }

    [Fact]
    public void VariousRegexPatterns_ShouldWork()
    {
        var json = @"{
            ""digits"": ""12345"",
            ""letters"": ""abcdef"",
            ""mixed"": ""abc123"",
            ""uuid"": ""550e8400-e29b-41d4-a716-446655440000"",
            ""url"": ""https://example.com/path"",
            ""ipAddress"": ""192.168.1.1""
        }";

        json.ShouldHaveJsonValueMatchingRegex("/digits", @"^\d+$");
        json.ShouldHaveJsonValueMatchingRegex("/letters", @"^[a-z]+$");
        json.ShouldHaveJsonValueMatchingRegex("/mixed", @"^[a-z]+\d+$");
        json.ShouldHaveJsonValueMatchingRegex("/uuid", @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
        json.ShouldHaveJsonValueMatchingRegex("/url", @"^https?://[^\s/$.?#].[^\s]*$");
        json.ShouldHaveJsonValueMatchingRegex("/ipAddress", @"^(\d{1,3}\.){3}\d{1,3}$");
    }

    [Fact]
    public void CaseSensitiveRegex_ShouldWork()
    {
        var json = @"{""value"": ""Test""}";

        json.ShouldHaveJsonValueMatchingRegex("/value", @"^Test$");
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/value", @"^test$"));
    }

    [Fact]
    public void RegexFlags_ShouldWork()
    {
        var json = @"{""value"": ""Test""}";
        
        // Test case insensitive matching using (?i) flag
        json.ShouldHaveJsonValueMatchingRegex("/value", @"(?i)^test$");
    }

    [Fact]
    public void PartialMatch_ShouldWork()
    {
        var json = @"{""sentence"": ""The quick brown fox jumps over the lazy dog""}";

        json.ShouldHaveJsonValueMatchingRegex("/sentence", @"quick.*fox");
        json.ShouldHaveJsonValueMatchingRegex("/sentence", @"brown");
        json.ShouldHaveJsonValueMatchingRegex("/sentence", @"^The");
        json.ShouldHaveJsonValueMatchingRegex("/sentence", @"dog$");
    }

    [Fact]
    public void EmptyStringRegex_ShouldWork()
    {
        var json = @"{""empty"": """", ""notEmpty"": ""value""}";

        json.ShouldHaveJsonValueMatchingRegex("/empty", @"^$");
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/notEmpty", @"^$"));
    }

    [Fact]
    public void SpecialCharacters_ShouldWork()
    {
        var json = @"{
            ""symbols"": ""!@#$%^&*()"",
            ""whitespace"": ""hello world"",
            ""newlines"": ""line1\nline2"",
            ""tabs"": ""col1\tcol2""
        }";

        json.ShouldHaveJsonValueMatchingRegex("/symbols", @"^[!@#$%^&*()]+$");
        json.ShouldHaveJsonValueMatchingRegex("/whitespace", @"hello\s+world");
        json.ShouldHaveJsonValueMatchingRegex("/newlines", @"line1[\s\S]*line2");
        json.ShouldHaveJsonValueMatchingRegex("/tabs", @"col1\s+col2");
    }

    [Theory]
    [InlineData("/invalid/path")]
    [InlineData("invalid")]
    [InlineData("~invalid")]
    public void InvalidPointer_ShouldThrow(string pointer)
    {
        var json = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex(pointer, @"^[A-Z][a-z]+$"));
    }

    [Fact]
    public void InvalidArrayIndex_ShouldThrow()
    {
        var json = @"{""array"": [""value1"", ""value2""]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/array/5", @"^value"));
    }

    [Fact]
    public void NonStringValue_ShouldThrow()
    {
        var json = @"{""number"": 42, ""boolean"": true, ""object"": {}, ""array"": []}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/number", @"^\d+$"));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/boolean", @"^true$"));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/object", @"^.*$"));
        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/array", @"^.*$"));
    }

    [Fact]
    public void NullValue_ShouldThrow()
    {
        var json = @"{""value"": null}";

        Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/value", @"^.*$"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldHaveJsonValueMatchingRegex("/path", @"^.*$"));
    }

    [Theory]
    [InlineData(@"[")]
    [InlineData(@"*")]
    [InlineData(@"(?P<name>")]
    [InlineData(@"(?<name")]
    public void InvalidRegex_ShouldThrow(string invalidRegex)
    {
        var json = @"{""value"": ""test""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/value", invalidRegex));
        ex.Message.ShouldContain("Invalid regex pattern");
    }

    [Fact]
    public void RegexDoesNotMatch_ShouldThrow()
    {
        var json = @"{""value"": ""hello""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldHaveJsonValueMatchingRegex("/value", @"^\d+$"));
        ex.Message.ShouldContain("should match regex pattern");
        ex.Message.ShouldContain(@"^\d+$");
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""value"": ""hello""}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldHaveJsonValueMatchingRegex("/value", @"^\d+$", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void EscapedCharacters_ShouldWork()
    {
        var json = @"{""~tilde"": ""value"", ""slash/prop"": ""test""}";

        json.ShouldHaveJsonValueMatchingRegex("/~0tilde", @"^value$");
        json.ShouldHaveJsonValueMatchingRegex("/slash~1prop", @"^test$");
    }

    [Fact]
    public void ComplexRegexPatterns_ShouldWork()
    {
        var json = @"{
            ""creditCard"": ""4532-1234-5678-9012"",
            ""socialSecurity"": ""123-45-6789"",
            ""postalCode"": ""12345-6789"",
            ""hexColor"": ""#FF5733"",
            ""macAddress"": ""00:1B:44:11:3A:B7""
        }";

        json.ShouldHaveJsonValueMatchingRegex("/creditCard", @"^\d{4}-\d{4}-\d{4}-\d{4}$");
        json.ShouldHaveJsonValueMatchingRegex("/socialSecurity", @"^\d{3}-\d{2}-\d{4}$");
        json.ShouldHaveJsonValueMatchingRegex("/postalCode", @"^\d{5}-\d{4}$");
        json.ShouldHaveJsonValueMatchingRegex("/hexColor", @"^#[0-9A-F]{6}$");
        json.ShouldHaveJsonValueMatchingRegex("/macAddress", @"^([0-9A-F]{2}:){5}[0-9A-F]{2}$");
    }

    [Fact]
    public void UnicodeCharacters_ShouldWork()
    {
        var json = @"{
            ""emoji"": ""😊👍"",
            ""chinese"": ""你好"",
            ""arabic"": ""مرحبا"",
            ""cyrillic"": ""Привет""
        }";

        json.ShouldHaveJsonValueMatchingRegex("/emoji", @"^[\uD800-\uDBFF][\uDC00-\uDFFF][\uD800-\uDBFF][\uDC00-\uDFFF]$|^..$");
        json.ShouldHaveJsonValueMatchingRegex("/chinese", @"^[\u4e00-\u9fff]+$");
        json.ShouldHaveJsonValueMatchingRegex("/arabic", @"^[\u0600-\u06ff]+$");
        json.ShouldHaveJsonValueMatchingRegex("/cyrillic", @"^[\u0400-\u04ff]+$");
    }

    [Fact]
    public void QuantifierPatterns_ShouldWork()
    {
        var json = @"{
            ""oneOrMore"": ""aaaa"",
            ""zeroOrMore"": """",
            ""optional"": ""test"",
            ""range"": ""abc""
        }";

        json.ShouldHaveJsonValueMatchingRegex("/oneOrMore", @"^a+$");
        json.ShouldHaveJsonValueMatchingRegex("/zeroOrMore", @"^a*$");
        json.ShouldHaveJsonValueMatchingRegex("/optional", @"^test?$");
        json.ShouldHaveJsonValueMatchingRegex("/range", @"^[a-z]{3}$");
    }

    [Fact]
    public void GroupingAndCapture_ShouldWork()
    {
        var json = @"{
            ""grouped"": ""abc123"",
            ""alternation"": ""cat"",
            ""alternation2"": ""dog""
        }";

        json.ShouldHaveJsonValueMatchingRegex("/grouped", @"^([a-z]+)(\d+)$");
        json.ShouldHaveJsonValueMatchingRegex("/alternation", @"^(cat|dog)$");
        json.ShouldHaveJsonValueMatchingRegex("/alternation2", @"^(cat|dog)$");
    }

    [Fact]
    public void MultilineStrings_ShouldWork()
    {
        var json = @"{""multiline"": ""line1\nline2\nline3""}";

        json.ShouldHaveJsonValueMatchingRegex("/multiline", @"line1[\s\S]*line2[\s\S]*line3$");
        json.ShouldHaveJsonValueMatchingRegex("/multiline", @"^line1");
        json.ShouldHaveJsonValueMatchingRegex("/multiline", @"line3$");
    }

    [Fact]
    public void LookaheadLookbehind_ShouldWork()
    {
        var json = @"{
            ""password"": ""Password123!"",
            ""beforeAfter"": ""abc123def""
        }";

        // Positive lookahead: contains at least one digit
        json.ShouldHaveJsonValueMatchingRegex("/password", @"^(?=.*\d).*$");
        
        // Positive lookbehind: digit preceded by letters  
        json.ShouldHaveJsonValueMatchingRegex("/beforeAfter", @"(?<=[a-z])\d");
    }

    [Theory]
    [InlineData(@"/path/to/value")]
    [InlineData(@"#/path/to/value")]
    public void DifferentPointerFormats_ShouldWork(string pointer)
    {
        var json = @"{""path"": {""to"": {""value"": ""test""}}}";
        
        json.ShouldHaveJsonValueMatchingRegex(pointer, @"^test$");
    }

    [Fact]
    public void LargeString_ShouldWork()
    {
        var largeString = new string('a', 10000);
        var json = $@"{{""large"": ""{largeString}""}}";

        json.ShouldHaveJsonValueMatchingRegex("/large", @"^a+$");
        json.ShouldHaveJsonValueMatchingRegex("/large", @"^a{10000}$");
    }

    [Fact]
    public void ErrorMessages_ShouldBeDescriptive()
    {
        var json = @"{""value"": ""hello""}";

        var ex = Should.Throw<ShouldAssertException>(() => 
            json.ShouldHaveJsonValueMatchingRegex("/value", @"^\d+$"));
        
        ex.Message.ShouldContain("JSON value at pointer '/value' should match regex pattern");
        ex.Message.ShouldContain(@"^\d+$");
        ex.Message.ShouldContain("hello");
    }
}