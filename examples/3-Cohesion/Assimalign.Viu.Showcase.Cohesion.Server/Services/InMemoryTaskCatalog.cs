using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assimalign.Viu.Showcase.Cohesion.Shared;

namespace Assimalign.Viu.Showcase.Cohesion.Server.Services;
/// <summary>
/// The server's <see cref = "ITaskCatalog"/>: a thread-safe in-memory list. One instance is shared by
/// the HTTP API and server-rendered components, so a task added from the client shows up in the
/// server-rendered report.
/// </summary>
public sealed class InMemoryTaskCatalog : ITaskCatalog
{
    private readonly Lock _gate = new();
    private readonly List<TaskSummary> _tasks = [new(1, "Wire up dependency injection", true), new(2, "Render a page on the server", true), new(3, "Call the API from the browser", false), ];
    public TaskSummary[] GetSnapshot()
    {
        lock (_gate)
        {
            return[.._tasks];
        }
    }

    public TaskSummary Add(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        lock (_gate)
        {
            TaskSummary task = new(_tasks.Count + 1, title.Trim(), false);
            _tasks.Add(task);
            return task;
        }
    }

    public Task<IReadOnlyList<TaskSummary>> GetTasksAsync(CancellationToken cancellationToken = default) => Task.FromResult<IReadOnlyList<TaskSummary>>(GetSnapshot());
    public Task<TaskSummary> AddAsync(string title, CancellationToken cancellationToken = default) => Task.FromResult(Add(title));
}
