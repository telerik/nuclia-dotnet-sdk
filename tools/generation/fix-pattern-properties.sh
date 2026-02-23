#!/bin/bash

# Post-processing script to fix patternProperties in generated models
# Replaces Object? types with proper Dictionary<string, T> types

echo "Post-processing: Fixing patternProperties types..."

TARGET_DIR="../../Progress.Nuclia/Model/Generated"

# Function to replace pattern properties in a file
fix_file() {
    local file=$1
    local field_name=$2
    local type_name=$3
    
    if [ ! -f "$file" ]; then
        return
    fi
    
    echo "  Processing $file - fixing $field_name field..."
    
    # Capitalize first letter for property name
    local prop_name="$(echo ${field_name:0:1} | tr '[:lower:]' '[:upper:]')${field_name:1}"
    
    # Fix the constructor parameter type
    sed -i "s/Option<Object?> ${field_name} = default/Option<Dictionary<string, ${type_name}>?> ${field_name} = default/g" "$file"
    
    # Fix the Option field type
    sed -i "s/internal Option<Object?> ${prop_name}Option/internal Option<Dictionary<string, ${type_name}>?> ${prop_name}Option/g" "$file"
    
    # Fix the public property type
    sed -i "s/public Object? ${prop_name} { get/public Dictionary<string, ${type_name}>? ${prop_name} { get/g" "$file"
    
    # Fix the local variable in Read method
    sed -i "s/Option<Object?> ${field_name} = default;/Option<Dictionary<string, ${type_name}>?> ${field_name} = default;/g" "$file"
    
    # Fix the deserialization in Read method
    sed -i "s/${field_name} = new Option<Object?>(JsonSerializer.Deserialize<Object>(ref utf8JsonReader, jsonSerializerOptions)!)/${field_name} = new Option<Dictionary<string, ${type_name}>?>(JsonSerializer.Deserialize<Dictionary<string, ${type_name}>>(ref utf8JsonReader, jsonSerializerOptions)!)/g" "$file"
}

# Function to fix resource payload files
fix_resource_payload() {
    local file_name=$1
    local file_path="$TARGET_DIR/$file_name"
    
    if [ -f "$file_path" ]; then
        echo "Fixing $file_name..."
        fix_file "$file_path" "files" "FileField"
        fix_file "$file_path" "links" "LinkField"
        fix_file "$file_path" "texts" "TextField"
        fix_file "$file_path" "conversations" "InputConversationField"
    fi
}

# Fix CreateResourcePayload
fix_resource_payload "CreateResourcePayload.cs"

# Fix UpdateResourcePayload
fix_resource_payload "UpdateResourcePayload.cs"

echo "Post-processing complete!"
