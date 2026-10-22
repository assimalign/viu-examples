using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Assimalign.Viu.Showcase.Cohesion.Shared;
/// <summary>
/// The task data contract. Components depend only on this interface and resolve it from
/// <c>Context.Services</c>; each host registers its own implementation in dependency injection:
/// the server registers the in-memory store, the browser client registers an HTTP client for
/// <c>/api/tasks</c>. The same component therefore works in both places.
/// </summary>
public interface ITaskCatalog
{
    Task<IReadOnlyList<TaskSummary>> GetTasksAsync(CancellationToken cancellationToken = default);
    Task<TaskSummary> AddAsync(string title, CancellationToken cancellationToken = default);
}
