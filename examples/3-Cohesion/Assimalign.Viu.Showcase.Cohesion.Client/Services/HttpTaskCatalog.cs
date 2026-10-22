using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Assimalign.Viu.Showcase.Cohesion.Shared;

namespace Assimalign.Viu.Showcase.Cohesion.Client.Services;
/// <summary>
/// The browser's <see cref = "ITaskCatalog"/>: calls the server's JSON API with the HttpClient
/// registered in DI. JSON uses the source-generated <see cref = "ShowcaseJsonContext"/>.
/// </summary>
public sealed class HttpTaskCatalog : ITaskCatalog
{
    private readonly HttpClient _http;
    public HttpTaskCatalog(HttpClient http)
    {
        ArgumentNullException.ThrowIfNull(http);
        _http = http;
    }

    public async Task<IReadOnlyList<TaskSummary>> GetTasksAsync(CancellationToken cancellationToken = default) => await _http.GetFromJsonAsync("api/tasks", ShowcaseJsonContext.Default.TaskSummaryArray, cancellationToken) ?? [];
    public async Task<TaskSummary> AddAsync(string title, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _http.PostAsJsonAsync("api/tasks", new NewTaskRequest(title), ShowcaseJsonContext.Default.NewTaskRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync(ShowcaseJsonContext.Default.TaskSummary, cancellationToken) ?? throw new InvalidOperationException("The server returned an empty task.");
    }
}
