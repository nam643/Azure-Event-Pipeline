# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project context

A C#/.NET 9 learning project: a synthetic trade-event processor built in three phases by a developer new to C#. The user writes the code themselves. Guide with concepts and hints rather than dumping code, and don't introduce later-phase concepts early.

1. **Worker Service** (done): `BackgroundService`, DI, `ILogger`, reads JSON events on a timer.
2. **Azure Blob Storage** (in progress): per-batch JSON summaries pushed with `Azure.Storage.Blobs`. A connection string is used first, then `DefaultAzureCredential`.
3. **Aspire** (planned): an AppHost wrapping the worker plus a minimal API with `/stats` that reads the same storage.

## Commands

Run from the repo root (`azureEventPipeline.sln`). There are no tests yet.

```bash
dotnet build
dotnet run --project src/EventProcessor.Worker
```

The worker will not start without `BlobStorage:ConnectionString`. It is not in `appsettings.json`, so supply it through user secrets (the project has a `UserSecretsId`) or an environment variable:

```bash
dotnet user-secrets set "BlobStorage:ConnectionString" "<conn string>" --project src/EventProcessor.Worker
# or: export BlobStorage__ConnectionString="<conn string>"
```

For local development, Azurite (`UseDevelopmentStorage=true`) works as the storage backend.

## Architecture

Single project, `src/EventProcessor.Worker`. `Program.cs` wires everything through DI.

- `Worker` (`BackgroundService`) loops until cancelled: `IEventSource.GetNextBatchAsync` → `BatchSummarizer.Summarize` → `IBatchSink.WriteAsync` → log, then delay by `IntervalSeconds`.
  - Per-batch exceptions are caught and logged so the loop keeps going.
  - `OperationCanceledException` is excluded from that catch so shutdown stays clean.
- `IEventSource` is implemented by `JsonFileEventSource`, which reads `Data/events.json`. The csproj copies that file to the output directory.
- `IBatchSink` has two implementations, `BlobBatchSink` and `LoggingBatchSink`. `Program.cs` currently registers `BlobBatchSink`; switch the registration to change sinks.
- `BlobBatchSink` creates the container lazily on the first write. Blobs are named `yyyy/MM/dd/{BatchId}.json` and serialized with web JSON defaults (camelCase).
- Models in `Models/`: `TradeEvent` → `BatchSummary` containing `SymbolSummary` entries.
- Options in `Configuration/` are bound from config sections and validated on startup (`ValidateDataAnnotations().ValidateOnStart()`):
  - `WorkerOptions`, section `EventProcessor` (`BatchSize`, `IntervalSeconds`)
  - `BlobStorageOptions`, section `BlobStorage` (`ConnectionString`, `ContainerName`)
- `BlobContainerClient` is registered as a singleton, built from `BlobStorageOptions`.
