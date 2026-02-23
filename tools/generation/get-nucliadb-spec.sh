#!/bin/bash

# Exit on error
set -e

# Function to download the OpenAPI spec from CDN
download_openapi_spec() {
    local spec_url="$1"
    local spec_file="$2"
    
    echo "Downloading OpenAPI spec from $spec_url..."
    if command -v curl &> /dev/null; then
        curl -fsSL -o "$spec_file" "$spec_url"
    elif command -v wget &> /dev/null; then
        wget -q -O "$spec_file" "$spec_url"
    else
        echo "Error: Neither curl nor wget is available. Please install one of them."
        exit 1
    fi
    
    if [ ! -f "$spec_file" ]; then
        echo "Error: Failed to download OpenAPI spec"
        exit 1
    fi
    
    echo "Successfully downloaded $spec_file"
}

# Download the latest OpenAPI spec from CDN
download_openapi_spec "https://cdn.rag.progress.cloud/api/nucliadb/v1/api.yaml" "nucliadb.yaml"
