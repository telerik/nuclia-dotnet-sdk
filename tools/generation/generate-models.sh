#!/bin/bash

# Exit on error
set -e

echo "Starting model generation..."

echo "Generating models from OpenAPI spec..."
npx @openapitools/openapi-generator-cli generate -c openapi-generator-config.json

if [ ! -d "./models" ]; then
    echo "Error: Models directory not created"
    exit 1
fi

TARGET_DIR="../../../src/Progress.Nuclia/Model/Generated"

# Create target directory if it doesn't exist
mkdir -p "$TARGET_DIR"

# Read excluded models from config
EXCLUDE_CONFIG="exclude-models.json"
if [ -f "$EXCLUDE_CONFIG" ]; then
    echo "Loading exclusion list from $EXCLUDE_CONFIG..."
    EXCLUDED_MODELS=$(cat "$EXCLUDE_CONFIG" | grep -oP '"\K[^"]+(?=")' | grep '\.cs$')
else
    echo "Warning: $EXCLUDE_CONFIG not found, copying all models"
    EXCLUDED_MODELS=""
fi

# Clear target directory
echo "Clearing target directory $TARGET_DIR..."
rm -rf "$TARGET_DIR"/*

# Copy generated models to the project, excluding specified files
echo "Copying generated models to $TARGET_DIR..."
SOURCE_DIR="./models/src/Progress.Nuclia/Model"

for file in "$SOURCE_DIR"/*; do
    filename=$(basename "$file")
    
    # Check if file should be excluded
    skip=false
    for excluded in $EXCLUDED_MODELS; do
        if [ "$filename" = "$excluded" ]; then
            echo "Skipping excluded file: $filename"
            skip=true
            break
        fi
    done
    
    # Copy if not excluded
    if [ "$skip" = false ]; then
        cp "$file" "$TARGET_DIR/"
    fi
done

# Run post-processing to fix patternProperties
echo "Running post-processing..."
if [ -f "./fix-pattern-properties.sh" ]; then
    bash ./fix-pattern-properties.sh
else
    echo "Warning: Post-processing script not found"
fi

echo "Cleaning up temporary files..."
rm -rf ./models

echo "Model generation complete!"
