# Model Generation

This directory contains tools for generating C# model classes from the NucliaDB OpenAPI specification.

## Quick Start

To regenerate all models:

```bash
cd tools/generation
bash get-nucliadb-spec.sh    # Download latest spec (optional)
bash generate-models.sh       # Generate models
```

Or run both steps together:

```bash
bash get-nucliadb-spec.sh && bash generate-models.sh
```

## Generation Process

The model generation follows these steps:

### 1. Download OpenAPI Specification

**Script:** `get-nucliadb-spec.sh`

Downloads the latest NucliaDB OpenAPI spec from the CDN:

- **Source:** `https://cdn.rag.progress.cloud/api/nucliadb/v1/api.yaml`
- **Output:** `nucliadb.yaml` (local file)
- **Tools:** Uses `curl` or `wget` (automatic fallback)

### 2. Generate C# Models

**Script:** `generate-models.sh`

Generates C# model classes using OpenAPI Generator and processes them for the SDK.

#### Steps:

1. **Generate Models**
   - Uses `@openapitools/openapi-generator-cli`
   - Configuration: `openapi-generator-config.json`
   - Input: `nucliadb.yaml`
   - Temporary output: `./models/`

2. **Filter Excluded Models**
   - Reads exclusion list from `exclude-models.json`
   - Skips models that conflict with custom implementations
   - Current excluded models:
     - `Error.cs`
     - `SearchFilters.cs`, `CatalogFilters.cs`, `KeywordFilters.cs`, `ResourceFilters.cs`
     - `NotOperand.cs`, `NotOperand2.cs`, `NotOperand5.cs`
     - `ValidationErrorLocInner.cs`

3. **Copy to SDK**
   - Target: `Progress.Nuclia/Model/Generated/`
   - Clears existing generated models
   - Copies only non-excluded files

4. **Post-Processing**
   - Script: `fix-pattern-properties.sh`
   - Fixes OpenAPI `patternProperties` mappings
   - Converts `Object?` to proper `Dictionary<string, T>` types
   - Ensures type safety for dynamic field collections

5. **Cleanup**
   - Removes temporary `./models/` directory

## Requirements

- **Bash** (Git Bash on Windows, native on Linux/macOS)
- **Node.js** and **npm** (for OpenAPI Generator CLI)
- **curl** or **wget** (for downloading spec)

## Troubleshooting

### Download fails

If `get-nucliadb-spec.sh` fails:

1. Check internet connection
2. Verify the CDN URL is accessible
3. Ensure `curl` or `wget` is installed

### Generation fails

If `generate-models.sh` fails:

1. Ensure `nucliadb.yaml` exists (run `get-nucliadb-spec.sh` first)
2. Check that Node.js/npm are installed
3. Verify `openapi-generator-config.json` is valid
4. Look for errors in the OpenAPI spec itself

### Models missing or incorrect

1. Check if the model is in `exclude-models.json`
2. Review the OpenAPI spec for the entity definition
3. Check `fix-pattern-properties.sh` for post-processing rules

## Manual Steps

### Use a different spec version

Edit `get-nucliadb-spec.sh` and change the URL:

```bash
download_openapi_spec "https://your-custom-url/api.yaml" "nucliadb.yaml"
```

### Add custom model exclusions

Edit `exclude-models.json`:

```json
{
  "excludedModels": ["YourModel.cs"]
}
```

## Output

Generated models are placed in:

```
Progress.Nuclia/Model/Generated/
```

These files are auto-generated and should not be manually edited. Any changes will be overwritten on the next generation run.
