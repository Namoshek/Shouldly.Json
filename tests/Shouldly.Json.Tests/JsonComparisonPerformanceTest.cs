namespace Shouldly;

using System.Diagnostics;
using System.Linq;
using System.Text.Json.Nodes;

public class JsonComparisonPerformanceTest
{
    [Fact]
    public void CompareSemanticEquality_WithLargeIdenticalObjects_ShouldCompleteQuickly()
    {
        var largeObject = CreateLargeJsonObject(1000);
        var actual = JsonNode.Parse(largeObject);
        var expected = JsonNode.Parse(largeObject);
        
        var stopwatch = Stopwatch.StartNew();
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);
        stopwatch.Stop();
        
        result.IsEqual.ShouldBeTrue();
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(1000); // Should complete within 1 second
    }

    [Fact]
    public void CompareSemanticEquality_WithLargeArrays_ShouldCompleteQuickly()
    {
        var size = 10000;
        var largeArray = Enumerable.Range(0, size).ToArray();
        var actual = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(largeArray));
        var expected = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(largeArray));
        
        var stopwatch = Stopwatch.StartNew();
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);
        stopwatch.Stop();
        
        result.IsEqual.ShouldBeTrue();
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(1000); // Should complete within 1 second
    }

    [Fact]
    public void CompareSemanticEquality_WithLargeDifferentObjects_ShouldFailFast()
    {
        var largeObject1 = CreateLargeJsonObject(1000, "value1");
        var largeObject2 = CreateLargeJsonObject(1000, "value2");
        var actual = JsonNode.Parse(largeObject1);
        var expected = JsonNode.Parse(largeObject2);
        
        var stopwatch = Stopwatch.StartNew();
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);
        stopwatch.Stop();
        
        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(100); // Should fail fast
    }

    [Fact]
    public void CompareSemanticEquality_WithDeeplyNestedStructure_ShouldHandleDepthLimit()
    {
        var depth = 50; // Within JSON parser limits but tests our depth handling
        var actualJson = CreateDeeplyNestedJson(depth, "value1");
        var expectedJson = CreateDeeplyNestedJson(depth, "value2");
        
        var actual = JsonNode.Parse(actualJson);
        var expected = JsonNode.Parse(expectedJson);
        
        var result = JsonComparisonEngine.CompareSemanticEquality(actual, expected);
        
        result.IsEqual.ShouldBeFalse();
        result.FirstDifference.ShouldNotBeNull();
        result.FirstDifference!.Path.ShouldContain("/level");
    }

    [Fact]
    public void ShouldBeSemanticallySameJson_WithLargeObjects_ShouldProvideDetailedErrorsQuickly()
    {
        var largeObject1 = CreateLargeJsonObject(500, "value1");
        var largeObject2 = CreateLargeJsonObject(500, "value2");
        
        var stopwatch = Stopwatch.StartNew();
        var exception = Should.Throw<ShouldAssertException>(() => largeObject1.ShouldBeSemanticallySameJson(largeObject2));
        stopwatch.Stop();
        
        exception.Message.ShouldContain("JSON value mismatch");
        stopwatch.ElapsedMilliseconds.ShouldBeLessThan(500); // Should complete quickly even with error
    }

    private static string CreateLargeJsonObject(int propertyCount, string valuePrefix = "value")
    {
        var properties = Enumerable.Range(0, propertyCount)
            .Select(i => $"\"property{i}\": \"{valuePrefix}{i}\"");
        
        return "{" + string.Join(", ", properties) + "}";
    }

    private static string CreateDeeplyNestedJson(int depth, string finalValue)
    {
        var json = $"{{\"value\":\"{finalValue}\"}}";
        
        for (int i = 0; i < depth; i++)
        {
            json = $"{{\"level\":{json}}}";
        }
        
        return json;
    }
}
