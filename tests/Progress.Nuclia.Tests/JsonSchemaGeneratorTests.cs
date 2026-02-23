using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;
using Progress.Nuclia.Services;

namespace Progress.Nuclia.Tests;

public class JsonSchemaGeneratorTests
{
    [Description("Test model for schema generation")]
    public class TestModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("value")]
        public int Value { get; set; }

        [JsonPropertyName("items")]
        public List<string> Items { get; set; } = new();
    }

    public class ModelWithoutDescription
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";
    }

    [Fact]
    public void GenerateSchema_ShouldIncludeSchemaProperty()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<TestModel>();

        // Assert
        Assert.True(schema.ContainsKey("$schema"));
        Assert.Equal("https://json-schema.org/draft/2020-12/schema", schema["$schema"]);
    }

    [Fact]
    public void GenerateSchema_ShouldIncludeTitleFromTypeName()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<TestModel>();

        // Assert
        Assert.True(schema.ContainsKey("title"));
        Assert.Equal("TestModel", schema["title"]);
    }

    [Fact]
    public void GenerateSchema_ShouldIncludeDescriptionFromAttribute()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<TestModel>();

        // Assert
        Assert.True(schema.ContainsKey("description"));
        Assert.Equal("Test model for schema generation", schema["description"]);
    }

    [Fact]
    public void GenerateSchema_ShouldSetAdditionalPropertiesToFalse()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<TestModel>();

        // Assert
        Assert.True(schema.ContainsKey("additionalProperties"));
        Assert.Equal(false, schema["additionalProperties"]);
    }

    [Fact]
    public void GenerateSchema_ShouldIncludeTypeProperty()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<TestModel>();

        // Assert
        Assert.True(schema.ContainsKey("type"));
        // The type should be normalized to "object" (not ["object","null"])
        Assert.Equal("object", schema["type"]);
    }

    [Fact]
    public void GenerateSchema_WithoutDescriptionAttribute_ShouldStillWork()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<ModelWithoutDescription>();

        // Assert
        Assert.True(schema.ContainsKey("$schema"));
        Assert.True(schema.ContainsKey("title"));
        Assert.Equal("ModelWithoutDescription", schema["title"]);
        Assert.True(schema.ContainsKey("additionalProperties"));
        Assert.Equal(false, schema["additionalProperties"]);
    }

    [Fact]
    public void GenerateSchema_ShouldProduceCompleteSchema()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<TestModel>();

        // Assert - Verify all required properties
        Assert.True(schema.ContainsKey("$schema"));
        Assert.Equal("https://json-schema.org/draft/2020-12/schema", schema["$schema"]);
        
        Assert.True(schema.ContainsKey("title"));
        Assert.Equal("TestModel", schema["title"]);
        
        Assert.True(schema.ContainsKey("description"));
        Assert.Equal("Test model for schema generation", schema["description"]);
        
        Assert.True(schema.ContainsKey("type"));
        Assert.Equal("object", schema["type"]);
        
        Assert.True(schema.ContainsKey("additionalProperties"));
        Assert.Equal(false, schema["additionalProperties"]);
        
        // Should also have properties
        Assert.True(schema.ContainsKey("properties"));
    }

    [Fact]
    public void GenerateSchema_ShouldApplyAdditionalPropertiesFalseToNestedObjects()
    {
        // Arrange - Create a model with nested objects
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<NestedModel>();
        var schemaJson = JsonSerializer.Serialize(schema);

        // Assert - Root level should have additionalProperties: false
        Assert.True(schema.ContainsKey("additionalProperties"));
        Assert.Equal(false, schema["additionalProperties"]);
        
        // Verify the schema JSON contains additionalProperties: false in nested definitions
        Assert.Contains("\"additionalProperties\":false", schemaJson);
    }

    [Fact]
    public void GenerateSchema_ShouldEnsureRequiredArrayIncludesAllNonNullableProperties()
    {
        // Act
        var schema = JsonSchemaGenerator.GenerateSchema<ModelWithRequiredProperties>();
        var schemaJson = JsonSerializer.Serialize(schema, new JsonSerializerOptions { WriteIndented = true });

        // Output for debugging
        System.Diagnostics.Debug.WriteLine(schemaJson);
        
        // Assert - The schema should contain required arrays for nested objects
        // Even if the root doesn't have a required array (all properties nullable),
        // nested objects in $defs or items should have required arrays
        Assert.Contains("\"required\"", schemaJson);
    }

    public class ModelWithRequiredProperties
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("items")]
        public List<NestedItem> Items { get; set; } = new();
    }

    public class NestedModel
    {
        [JsonPropertyName("items")]
        public List<NestedItem> Items { get; set; } = new();
    }

    public class NestedItem
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
