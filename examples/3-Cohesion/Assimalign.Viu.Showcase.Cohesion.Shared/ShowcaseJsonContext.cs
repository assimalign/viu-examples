using System.Text.Json.Serialization;

namespace Assimalign.Viu.Showcase.Cohesion.Shared;
/// <summary>
/// Source-generated JSON metadata shared by the server endpoints and the client's HTTP catalog.
/// WebAssembly and NativeAOT forbid reflection-based serialization, so every wire type is listed.
/// </summary>
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(TaskSummary))]
[JsonSerializable(typeof(TaskSummary[]))]
[JsonSerializable(typeof(NewTaskRequest))]
public sealed partial class ShowcaseJsonContext : JsonSerializerContext;
