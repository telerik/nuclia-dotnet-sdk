# Basics Examples

This directory contains step-by-step examples demonstrating the core functionality of the Progress Nuclia SDK. Each example builds upon the previous one, introducing new concepts and features.

## Examples Overview

| Example | Description | Key Feature |
|---------|-------------|-------------|
| [SDK_Step01_Connecting](SDK_Step01_Connecting/) | Basic connection to Nuclia API | Simple client setup and usage |
| [SDK_Step02_Connecting_With_DI](SDK_Step02_Connecting_With_DI/) | Dependency Injection integration | Modern .NET service registration |
| [SDK_Step03_Streaming_Responses](SDK_Step03_Streaming_Responses/) | Real-time streaming responses | Progressive answer rendering |
| [SDK_Step04_Citations](SDK_Step04_Citations/) | Citations and source attribution | Transparency and verifiability |

## Prerequisites

All examples require Nuclia credentials configured using .NET user secrets:

```bash
dotnet user-secrets set "ApiKey" "your-api-key-value"
dotnet user-secrets set "ZoneId" "your-zone-id-value"
dotnet user-secrets set "KnowledgeBoxId" "your-knowledge-box-id-value"
```

## Recommended Learning Path

1. **SDK_Step01_Connecting** - Start here to learn basic connectivity
2. **SDK_Step02_Connecting_With_DI** - Essential for production applications
3. **SDK_Step03_Streaming_Responses** - Improve user experience with real-time results
4. **SDK_Step04_Citations** - Add transparency and source attribution
5. **SDK_Step05_StructuredOutput** - Get type-safe, strongly-typed responses
6. **SDK_Step06_Ingest** - Learn to add external content to your knowledge box

Each example directory contains its own detailed README with specific instructions and code explanations.

## Quick Start

Navigate to any example directory and run:

```bash
cd SDK_Step01_Connecting
dotnet run
```

Each example is self-contained and can run independently.

