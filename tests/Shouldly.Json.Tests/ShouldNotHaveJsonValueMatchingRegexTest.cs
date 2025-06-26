namespace Shouldly;

using System;

public class ShouldNotHaveJsonValueMatchingRegexTest
{
    [Fact]
    public void SimpleRegexNonMatch_ShouldWork()
    {
        var json = @"{""name"": ""john"", ""email"": ""invalid-email""}";

        json.ShouldNotHaveJsonValueMatchingRegex("/name", @"^[A-Z][a-z]+$"); // Should not match uppercase start
        json.ShouldNotHaveJsonValueMatchingRegex("/email", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"); // Should not match valid email pattern
    }

    [Fact]
    public void NestedPropertyRegexNonMatch_ShouldWork()
    {
        var json = @"{
            ""person"": {
                ""name"": ""john"",
                ""contact"": {
                    ""phone"": ""invalid-phone""
                }
            }
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/person/name", @"^[A-Z][a-z]+$"); // Should not match uppercase start
        json.ShouldNotHaveJsonValueMatchingRegex("/person/contact/phone", @"^\d{3}-\d{4}$"); // Should not match phone pattern
    }

    [Fact]
    public void ArrayElementRegexNonMatch_ShouldWork()
    {
        var json = @"{""emails"": [""invalid1"", ""invalid2""]}";

        json.ShouldNotHaveJsonValueMatchingRegex("/emails/0", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        json.ShouldNotHaveJsonValueMatchingRegex("/emails/1", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
    }

    [Fact]
    public void ComplexPathRegexNonMatch_ShouldWork()
    {
        var json = @"{
            ""users"": [
                {
                    ""name"": ""john"",
                    ""contacts"": [
                        {""type"": ""email"", ""value"": ""invalid-email""},
                        {""type"": ""phone"", ""value"": ""invalid-phone""}
                    ]
                }
            ]
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/users/0/name", @"^[A-Z][a-z]+$"); // Should not match uppercase start
        json.ShouldNotHaveJsonValueMatchingRegex("/users/0/contacts/0/value", @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
        json.ShouldNotHaveJsonValueMatchingRegex("/users/0/contacts/1/value", @"^\d{3}-\d{4}$");
    }

    [Fact]
    public void VariousRegexPatterns_ShouldWork()
    {
        var json = @"{
            ""letters"": ""abc123"",
            ""numbers"": ""abc"",
            ""mixed"": ""123abc"",
            ""uuid"": ""not-a-uuid"",
            ""url"": ""not-a-url"",
            ""ipAddress"": ""not-an-ip""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/letters", @"^[a-z]+$"); // Contains numbers
        json.ShouldNotHaveJsonValueMatchingRegex("/numbers", @"^\d+$"); // Contains letters
        json.ShouldNotHaveJsonValueMatchingRegex("/mixed", @"^[a-z]+\d+$"); // Wrong order
        json.ShouldNotHaveJsonValueMatchingRegex("/uuid", @"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
        json.ShouldNotHaveJsonValueMatchingRegex("/url", @"^https?://[^\s/$.?#].[^\s]*$");
        json.ShouldNotHaveJsonValueMatchingRegex("/ipAddress", @"^(\d{1,3}\.){3}\d{1,3}$");
    }

    [Fact]
    public void CaseSensitiveRegex_ShouldWork()
    {
        var json = @"{""value"": ""test""}";

        json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^Test$"); // Should not match different case
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^test$")); // Should throw because it matches
    }

    [Fact]
    public void RegexFlags_ShouldWork()
    {
        var json = @"{""value"": ""Test""}";
        
        // Should throw because case-insensitive regex matches
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/value", @"(?i)^test$"));
    }

    [Fact]
    public void PartialMatch_ShouldWork()
    {
        var json = @"{""sentence"": ""The quick brown fox jumps over the lazy dog""}";

        json.ShouldNotHaveJsonValueMatchingRegex("/sentence", @"cat"); // Should not contain 'cat'
        json.ShouldNotHaveJsonValueMatchingRegex("/sentence", @"^fox"); // Should not start with 'fox'
        json.ShouldNotHaveJsonValueMatchingRegex("/sentence", @"The$"); // Should not end with 'The'
        
        // Should throw because these patterns do match
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/sentence", @"quick.*fox"));
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/sentence", @"brown"));
    }

    [Fact]
    public void EmptyStringRegex_ShouldWork()
    {
        var json = @"{""empty"": """", ""notEmpty"": ""value""}";

        json.ShouldNotHaveJsonValueMatchingRegex("/notEmpty", @"^$"); // Should not match empty pattern
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/empty", @"^$")); // Should throw because empty matches
    }

    [Fact]
    public void SpecialCharacters_ShouldWork()
    {
        var json = @"{
            ""letters"": ""abcdef"",
            ""nospace"": ""helloworld"",
            ""singleline"": ""line1line2"",
            ""spaces"": ""col1 col2""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/letters", @"^[!@#$%^&*()]+$"); // Should not match symbols
        json.ShouldNotHaveJsonValueMatchingRegex("/nospace", @"hello\s+world"); // Should not match with spaces
        json.ShouldNotHaveJsonValueMatchingRegex("/singleline", @"line1\nline2"); // Should not match with newline
        json.ShouldNotHaveJsonValueMatchingRegex("/spaces", @"col1\tcol2"); // Should not match with tab
    }

    [Theory]
    [InlineData("/invalid/path")]
    [InlineData("invalid")]
    [InlineData("~invalid")]
    public void InvalidPointer_ShouldThrow(string pointer)
    {
        var json = @"{""name"": ""John""}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex(pointer, @"^[A-Z][a-z]+$"));
    }

    [Fact]
    public void InvalidArrayIndex_ShouldThrow()
    {
        var json = @"{""array"": [""value1"", ""value2""]}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/array/5", @"^value"));
    }

    [Fact]
    public void NonStringValue_ShouldThrow()
    {
        var json = @"{""number"": 42, ""boolean"": true, ""object"": {}, ""array"": []}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/number", @"^\d+$"));
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/boolean", @"^true$"));  
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/object", @"^.*$"));
        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/array", @"^.*$"));
    }

    [Fact]
    public void NullValue_ShouldThrow()
    {
        var json = @"{""value"": null}";

        Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^.*$"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("{invalid json}")]
    public void InvalidJson_ShouldThrow(string? invalidJson)
    {
        Should.Throw<ShouldAssertException>(() => invalidJson.ShouldNotHaveJsonValueMatchingRegex("/path", @"^.*$"));
    }

    [Theory]
    [InlineData(@"[")]
    [InlineData(@"*")]
    [InlineData(@"(?P<name>")]
    [InlineData(@"(?<name")]
    public void InvalidRegex_ShouldThrow(string invalidRegex)
    {
        var json = @"{""value"": ""test""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/value", invalidRegex));
        ex.Message.ShouldContain("Invalid regex pattern");
    }

    [Fact]
    public void RegexMatches_ShouldThrow()
    {
        var json = @"{""value"": ""12345""}";

        var ex = Should.Throw<ShouldAssertException>(() => json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^\d+$"));
        ex.Message.ShouldContain("should not match regex pattern");
        ex.Message.ShouldContain(@"^\d+$");
    }

    [Fact]
    public void CustomMessage_ShouldBeIncluded()
    {
        var json = @"{""value"": ""12345""}";
        var customMessage = "Custom error message";

        var ex = Should.Throw<ShouldAssertException>(() =>
            json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^\d+$", customMessage));

        ex.Message.ShouldContain(customMessage);
    }

    [Fact]
    public void EscapedCharacters_ShouldWork()
    {
        var json = @"{""~tilde"": ""nomatch"", ""slash/prop"": ""different""}";

        json.ShouldNotHaveJsonValueMatchingRegex("/~0tilde", @"^value$");
        json.ShouldNotHaveJsonValueMatchingRegex("/slash~1prop", @"^test$");
    }

    [Fact]
    public void ComplexRegexPatterns_ShouldWork()
    {
        var json = @"{
            ""invalidCard"": ""4532-1234-5678"",
            ""invalidSSN"": ""123-45"",
            ""invalidZip"": ""12345"",
            ""invalidHex"": ""#GG5733"",
            ""invalidMac"": ""00:1B:44:11:3A""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/invalidCard", @"^\d{4}-\d{4}-\d{4}-\d{4}$"); // Too short
        json.ShouldNotHaveJsonValueMatchingRegex("/invalidSSN", @"^\d{3}-\d{2}-\d{4}$"); // Missing last part
        json.ShouldNotHaveJsonValueMatchingRegex("/invalidZip", @"^\d{5}-\d{4}$"); // Missing dash and extension
        json.ShouldNotHaveJsonValueMatchingRegex("/invalidHex", @"^#[0-9A-F]{6}$"); // Invalid hex characters
        json.ShouldNotHaveJsonValueMatchingRegex("/invalidMac", @"^([0-9A-F]{2}:){5}[0-9A-F]{2}$"); // Too short
    }

    [Fact]
    public void UnicodeCharacters_ShouldWork()
    {
        var json = @"{
            ""latin"": ""hello"",
            ""english"": ""Hello"",
            ""english2"": ""Hi"",
            ""mixed"": ""Hello你好""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/latin", @"^[\u4e00-\u9fff]+$"); // Not Chinese
        json.ShouldNotHaveJsonValueMatchingRegex("/english", @"^[\u0600-\u06ff]+$"); // Not Arabic
        json.ShouldNotHaveJsonValueMatchingRegex("/english2", @"^[\u0400-\u04ff]+$"); // Not Cyrillic
        json.ShouldNotHaveJsonValueMatchingRegex("/mixed", @"^[\u4e00-\u9fff]+$"); // Mixed, not pure Chinese
    }

    [Fact]
    public void QuantifierPatterns_ShouldWork()
    {
        var json = @"{
            ""empty"": """",
            ""single"": ""b"",
            ""noMatch"": ""hello"",
            ""wrongLength"": ""abcd""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/empty", @"^a+$"); // Empty doesn't match one or more
        json.ShouldNotHaveJsonValueMatchingRegex("/single", @"^a+$"); // Wrong character
        json.ShouldNotHaveJsonValueMatchingRegex("/noMatch", @"^test$"); // Completely different word
        json.ShouldNotHaveJsonValueMatchingRegex("/wrongLength", @"^[a-z]{3}$"); // Wrong length
    }

    [Fact]
    public void GroupingAndCapture_ShouldWork()
    {
        var json = @"{
            ""wrongPattern"": ""123abc"",
            ""noMatch1"": ""bird"",
            ""noMatch2"": ""fish""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/wrongPattern", @"^([a-z]+)(\d+)$"); // Wrong order
        json.ShouldNotHaveJsonValueMatchingRegex("/noMatch1", @"^(cat|dog)$"); // Different animal
        json.ShouldNotHaveJsonValueMatchingRegex("/noMatch2", @"^(cat|dog)$"); // Different animal
    }

    [Fact]
    public void MultilineStrings_ShouldWork()
    {
        var json = @"{""singleline"": ""line1 line2 line3""}";

        json.ShouldNotHaveJsonValueMatchingRegex("/singleline", @"line1.*line2.*line4$"); // Wrong ending
        json.ShouldNotHaveJsonValueMatchingRegex("/singleline", @"^line2"); // Wrong beginning
        json.ShouldNotHaveJsonValueMatchingRegex("/singleline", @"line4$"); // Wrong ending
    }

    [Fact]
    public void LookaheadLookbehind_ShouldWork()
    {
        var json = @"{
            ""noDigits"": ""Password!"",
            ""wrongOrder"": ""123abc""
        }";

        // Should not match: no digits
        json.ShouldNotHaveJsonValueMatchingRegex("/noDigits", @"^(?=.*\d).*$");
        
        // Should not match: digit not preceded by letters
        json.ShouldNotHaveJsonValueMatchingRegex("/wrongOrder", @"(?<=[a-z])\d"); // Digits come first
    }

    [Theory]
    [InlineData(@"/path/to/value")]
    [InlineData(@"#/path/to/value")]
    public void DifferentPointerFormats_ShouldWork(string pointer)
    {
        var json = @"{""path"": {""to"": {""value"": ""nomatch""}}}";
        
        json.ShouldNotHaveJsonValueMatchingRegex(pointer, @"^test$"); // Should not match "test"
    }

    [Fact]
    public void LargeString_ShouldWork()
    {
        var largeString = new string('b', 10000); // Different character
        var json = $@"{{""large"": ""{largeString}""}}";

        json.ShouldNotHaveJsonValueMatchingRegex("/large", @"^a+$"); // Should not match 'a' pattern
        json.ShouldNotHaveJsonValueMatchingRegex("/large", @"^b{9999}$"); // Wrong count
    }

    [Fact]
    public void ErrorMessages_ShouldBeDescriptive()
    {
        var json = @"{""value"": ""12345""}";

        var ex = Should.Throw<ShouldAssertException>(() => 
            json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^\d+$"));
        
        ex.Message.ShouldContain("JSON value at pointer '/value' should not match regex pattern");
        ex.Message.ShouldContain(@"^\d+$");
        ex.Message.ShouldContain("12345");
    }

    [Fact]
    public void EdgeCases_EmptyAndWhitespace_ShouldWork()
    {
        var json = @"{
            ""empty"": """",
            ""spaces"": ""   "",
            ""tabs"": ""\t\t"",
            ""newlines"": ""\n\n""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/empty", @"^\s+$"); // Empty should not match whitespace pattern
        json.ShouldNotHaveJsonValueMatchingRegex("/spaces", @"^\t+$"); // Spaces should not match tab pattern
        json.ShouldNotHaveJsonValueMatchingRegex("/tabs", @"^ +$"); // Tabs should not match space pattern
        json.ShouldNotHaveJsonValueMatchingRegex("/newlines", @"^\r+$"); // Newlines should not match carriage return pattern
    }

    [Fact]
    public void BoundaryMatching_ShouldWork()
    {
        var json = @"{
            ""word"": ""hello world"",
            ""number"": ""abc123def""
        }";

        json.ShouldNotHaveJsonValueMatchingRegex("/word", @"^hello$"); // Should not match just "hello" (has " world")
        json.ShouldNotHaveJsonValueMatchingRegex("/number", @"^\d+$"); // Should not match just digits (has letters)
    }

    [Fact]
    public void CaseInsensitiveFailures_ShouldWork()
    {
        var json = @"{""value"": ""TEST""}";

        // Should not match case sensitive pattern
        json.ShouldNotHaveJsonValueMatchingRegex("/value", @"^test$");
        
        // But should throw with case insensitive pattern since it would match
        Should.Throw<ShouldAssertException>(() => 
            json.ShouldNotHaveJsonValueMatchingRegex("/value", @"(?i)^test$"));
    }
}