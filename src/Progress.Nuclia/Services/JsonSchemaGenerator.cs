using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Schema;

namespace Progress.Nuclia.Services;

/// <summary>
/// Generates JSON schema from .NET types using System.Text.Json.Schema.JsonSchemaExporter
/// </summary>
internal static class JsonSchemaGenerator
{
    /// <summary>
    /// Generates a JSON schema dictionary from the specified type
    /// </summary>
    /// <typeparam name="T">The type to generate schema for</typeparam>
    /// <param name="jsonOptions">JSON serializer options to use for schema generation</param>
    /// <returns>Dictionary representing the JSON schema</returns>
    public static Dictionary<string, object> GenerateSchema<T>(JsonSerializerOptions? jsonOptions = null)
    {
        // Use the built-in JsonSchemaExporter from .NET 9
        var options = jsonOptions ?? new JsonSerializerOptions();
        
        // Ensure TypeInfoResolver is set (required for GetJsonSchemaAsNode)
        if (options.TypeInfoResolver == null)
        {
            options.TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver();
        }
        
        var schemaNode = options.GetJsonSchemaAsNode(typeof(T));
        
        // Convert JsonNode to Dictionary<string, object> for manipulation
        var jsonString = schemaNode.ToJsonString();
        var schemaDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString, options) 
            ?? new Dictionary<string, object>();
        
        // Ensure required schema properties are present
        if (!schemaDictionary.ContainsKey("$schema"))
        {
            schemaDictionary["$schema"] = "https://json-schema.org/draft/2020-12/schema";
        }
        
        // Set title from type name if not already set
        if (!schemaDictionary.ContainsKey("title"))
        {
            schemaDictionary["title"] = typeof(T).Name;
        }
        
        // Extract description from type's DescriptionAttribute or XML summary if available
        if (!schemaDictionary.ContainsKey("description"))
        {
            var descriptionAttr = typeof(T).GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
            
            if (descriptionAttr != null)
            {
                schemaDictionary["description"] = descriptionAttr.Description;
            }
        }
        
        // Ensure additionalProperties is set to false if not already specified
        if (!schemaDictionary.ContainsKey("additionalProperties"))
        {
            schemaDictionary["additionalProperties"] = false;
        }
        
        // Normalize the "type" property to be "object" instead of ["object", "null"]
        // This is required by some JSON Schema consumers that don't accept null types
        if (schemaDictionary.ContainsKey("type"))
        {
            var typeValue = schemaDictionary["type"];
            
            // Handle JsonElement case
            if (typeValue is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    // Extract just "object" from the array
                    var typeArray = jsonElement.EnumerateArray()
                        .Where(e => e.GetString() == "object")
                        .FirstOrDefault();
                    
                    if (typeArray.ValueKind != JsonValueKind.Undefined)
                    {
                        schemaDictionary["type"] = "object";
                    }
                }
            }
            // Handle string array case
            else if (typeValue is string[] stringArray)
            {
                if (stringArray.Contains("object"))
                {
                    schemaDictionary["type"] = "object";
                }
            }
            // Handle object array case
            else if (typeValue is object[] objectArray)
            {
                if (objectArray.Any(o => o?.ToString() == "object"))
                {
                    schemaDictionary["type"] = "object";
                }
            }
        }
        
        // Build type registry for resolving types in $defs
        var typeRegistry = new Dictionary<string, Type>();
        BuildTypeRegistry(typeof(T), typeRegistry);
        
        // Recursively apply additionalProperties: false and add descriptions to all nested objects
        ApplyAdditionalPropertiesFalseRecursively(schemaDictionary, typeof(T), typeRegistry, options);
        
        return schemaDictionary;
    }
    
    /// <summary>
    /// Builds a registry of all types referenced in the schema
    /// </summary>
    private static void BuildTypeRegistry(Type type, Dictionary<string, Type> registry)
    {
        if (registry.ContainsKey(type.Name))
        {
            return;
        }
        
        // Register with multiple possible keys to handle different naming conventions
        registry[type.Name] = type; // Simple name: "Question"
        registry[type.FullName ?? type.Name] = type; // Full name: "Namespace.Question"
        
        // Also try without namespace for nested types
        if (type.FullName != null && type.FullName.Contains('+'))
        {
            var nestedName = type.FullName.Substring(type.FullName.LastIndexOf('+') + 1);
            if (!registry.ContainsKey(nestedName))
            {
                registry[nestedName] = type;
            }
        }
        
        // Register properties' types
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propType = prop.PropertyType;
            
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(propType) ?? propType;
            
            // Handle collection types
            if (underlyingType.IsGenericType)
            {
                foreach (var genericArg in underlyingType.GetGenericArguments())
                {
                    if (genericArg.IsClass && genericArg != typeof(string))
                    {
                        BuildTypeRegistry(genericArg, registry);
                    }
                }
            }
            else if (underlyingType.IsClass && underlyingType != typeof(string) && !underlyingType.IsPrimitive)
            {
                BuildTypeRegistry(underlyingType, registry);
            }
        }
    }
    
    /// <summary>
    /// Finds a property by name, handling different naming conventions (PascalCase, camelCase, snake_case)
    /// </summary>
    private static PropertyInfo? FindPropertyByName(Type type, string propertyName)
    {
        // Try direct match with ignore case
        var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propertyInfo != null)
        {
            return propertyInfo;
        }
        
        // Convert snake_case or kebab-case to PascalCase
        // Example: "prompt_text" -> "PromptText", "my-property" -> "MyProperty"
        var pascalCaseName = ConvertToPascalCase(propertyName);
        propertyInfo = type.GetProperty(pascalCaseName, BindingFlags.Public | BindingFlags.Instance);
        if (propertyInfo != null)
        {
            return propertyInfo;
        }
        
        // Try camelCase version
        // Example: "PromptText" -> "promptText"
        var camelCaseName = char.ToLowerInvariant(propertyName[0]) + propertyName.Substring(1);
        propertyInfo = type.GetProperty(camelCaseName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (propertyInfo != null)
        {
            return propertyInfo;
        }
        
        return null;
    }
    
    /// <summary>
    /// Converts snake_case or kebab-case to PascalCase
    /// </summary>
    private static string ConvertToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }
        
        // Split by underscores or hyphens
        var parts = input.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Capitalize first letter of each part
        var result = string.Concat(parts.Select(part => 
            char.ToUpperInvariant(part[0]) + part.Substring(1).ToLowerInvariant()));
        
        return result;
    }
    
    /// <summary>
    /// Recursively applies additionalProperties: false to all object schemas in the tree,
    /// ensures required arrays include all non-nullable properties, and adds descriptions from Data Annotations
    /// </summary>
    private static void ApplyAdditionalPropertiesFalseRecursively(Dictionary<string, object> schema, Type? currentType, Dictionary<string, Type> typeRegistry, JsonSerializerOptions options)
    {
        // If this schema defines an object type, ensure additionalProperties is false
        if (schema.ContainsKey("type"))
        {
            var typeValue = schema["type"];
            var isObject = false;
            
            if (typeValue is JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == JsonValueKind.String && jsonElement.GetString() == "object")
                {
                    isObject = true;
                }
                else if (jsonElement.ValueKind == JsonValueKind.Array)
                {
                    isObject = jsonElement.EnumerateArray().Any(e => e.GetString() == "object");
                }
            }
            else if (typeValue?.ToString() == "object")
            {
                isObject = true;
            }
            
            if (isObject)
            {
                if (!schema.ContainsKey("additionalProperties"))
                {
                    schema["additionalProperties"] = false;
                }
                
                // Ensure required array includes all properties with non-nullable types
                EnsureRequiredArrayIsComplete(schema);
            }
        }
        
        // Process nested properties and add descriptions
        if (schema.ContainsKey("properties") && schema["properties"] is JsonElement propertiesElement)
        {
            if (propertiesElement.ValueKind == JsonValueKind.Object)
            {
                var propertiesDict = JsonSerializer.Deserialize<Dictionary<string, object>>(propertiesElement.GetRawText());
                if (propertiesDict != null)
                {
                    foreach (var propertyName in propertiesDict.Keys.ToList())
                    {
                        var propertySchemaObj = propertiesDict[propertyName];
                        
                        if (propertySchemaObj is JsonElement propertySchemaElement && propertySchemaElement.ValueKind == JsonValueKind.Object)
                        {
                            var propertyDict = JsonSerializer.Deserialize<Dictionary<string, object>>(propertySchemaElement.GetRawText());
                            if (propertyDict != null)
                            {
                                // Find the corresponding .NET property (only if currentType is available)
                                PropertyInfo? propertyInfo = null;
                                if (currentType != null)
                                {
                                    // Try multiple name matching strategies to handle different naming conventions
                                    propertyInfo = FindPropertyByName(currentType, propertyName);
                                    
                                    // Add description from DescriptionAttribute if not already present
                                    if (propertyInfo != null && !propertyDict.ContainsKey("description"))
                                    {
                                        var descriptionAttr = propertyInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                                            .FirstOrDefault() as System.ComponentModel.DescriptionAttribute;
                                        
                                        if (descriptionAttr != null)
                                        {
                                            propertyDict["description"] = descriptionAttr.Description;
                                        }
                                    }
                                }
                                
                                // Recursively process nested objects
                                Type? nestedType = null;
                                if (propertyInfo != null)
                                {
                                    var propType = propertyInfo.PropertyType;
                                    nestedType = Nullable.GetUnderlyingType(propType) ?? propType;
                                    
                                    // Handle collections
                                    if (nestedType.IsGenericType && nestedType.GetGenericArguments().Length > 0)
                                    {
                                        var genericArg = nestedType.GetGenericArguments()[0];
                                        if (genericArg.IsClass && genericArg != typeof(string))
                                        {
                                            nestedType = genericArg;
                                        }
                                    }
                                }
                                
                                ApplyAdditionalPropertiesFalseRecursively(propertyDict, nestedType, typeRegistry, options);
                                propertiesDict[propertyName] = propertyDict;
                            }
                        }
                    }
                    schema["properties"] = propertiesDict;
                }
            }
        }
        
        // Process items in arrays
        if (schema.ContainsKey("items") && schema["items"] is JsonElement itemsElement)
        {
            if (itemsElement.ValueKind == JsonValueKind.Object)
            {
                var itemsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(itemsElement.GetRawText());
                if (itemsDict != null)
                {
                    // For array items, try to determine the item type
                    Type? itemType = null;
                    if (currentType != null)
                    {
                        // Handle array types (e.g., Question[])
                        if (currentType.IsArray)
                        {
                            itemType = currentType.GetElementType();
                        }
                        // Handle generic collection types (e.g., List<Question>, IEnumerable<Question>)
                        else if (currentType.IsGenericType)
                        {
                            var genericArgs = currentType.GetGenericArguments();
                            if (genericArgs.Length > 0)
                            {
                                itemType = genericArgs[0];
                            }
                        }
                    }
                    
                    ApplyAdditionalPropertiesFalseRecursively(itemsDict, itemType, typeRegistry, options);
                    schema["items"] = itemsDict;
                }
            }
        }
        
        // Process $defs (definitions)
        if (schema.ContainsKey("$defs") && schema["$defs"] is JsonElement defsElement)
        {
            if (defsElement.ValueKind == JsonValueKind.Object)
            {
                var defsDict = JsonSerializer.Deserialize<Dictionary<string, object>>(defsElement.GetRawText());
                if (defsDict != null)
                {
                    foreach (var key in defsDict.Keys.ToList())
                    {
                        if (defsDict[key] is JsonElement defElement && defElement.ValueKind == JsonValueKind.Object)
                        {
                            var defDict = JsonSerializer.Deserialize<Dictionary<string, object>>(defElement.GetRawText());
                            if (defDict != null)
                            {
                                // Try to find the type in the registry with various naming strategies
                                Type? defType = null;
                                if (!typeRegistry.TryGetValue(key, out defType))
                                {
                                    // Try to find by searching for a type with matching simple name
                                    defType = typeRegistry.Values.FirstOrDefault(t => t.Name == key);
                                }
                                
                                ApplyAdditionalPropertiesFalseRecursively(defDict, defType, typeRegistry, options);
                                defsDict[key] = defDict;
                            }
                        }
                    }
                    schema["$defs"] = defsDict;
                }
            }
        }
    }
    
    /// <summary>
    /// Ensures that the required array includes all properties that have non-nullable types
    /// </summary>
    private static void EnsureRequiredArrayIsComplete(Dictionary<string, object> schema)
    {
        if (!schema.ContainsKey("properties"))
        {
            return;
        }
        
        var propertiesElement = schema["properties"];
        if (propertiesElement is not JsonElement propertiesJson || propertiesJson.ValueKind != JsonValueKind.Object)
        {
            return;
        }
        
        // Get all property names - we'll add ALL of them to required unless explicitly nullable
        var requiredProperties = new List<string>();
        
        foreach (var property in propertiesJson.EnumerateObject())
        {
            if (property.Value.ValueKind == JsonValueKind.Object)
            {
                // Check if the property type is explicitly nullable (has "null" in type array)
                var isExplicitlyNullable = false;
                
                if (property.Value.TryGetProperty("type", out var typeElement))
                {
                    if (typeElement.ValueKind == JsonValueKind.Array)
                    {
                        // If type is an array like ["string", "null"], it's explicitly nullable
                        isExplicitlyNullable = typeElement.EnumerateArray().Any(t => t.GetString() == "null");
                    }
                }
                
                // Add to required if not explicitly nullable
                // This ensures that properties without explicit null types are treated as required
                if (!isExplicitlyNullable)
                {
                    requiredProperties.Add(property.Name);
                }
            }
        }
        
        // If there are required properties, set or update the required array
        if (requiredProperties.Count > 0)
        {
            schema["required"] = requiredProperties.ToArray();
        }
    }
}
