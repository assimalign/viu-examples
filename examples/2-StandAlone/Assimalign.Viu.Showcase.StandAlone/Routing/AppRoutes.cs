using System.Collections.Generic;

using Assimalign.Viu.Components;
using Assimalign.Viu.Router;

namespace Assimalign.Viu.Showcase.StandAlone.Routing;

/// <summary>The route table. Each record maps a path to a registered component by name.</summary>
public static class AppRoutes
{
    public static IReadOnlyList<RouteRecord> Create() =>
    [
        Page("/", "home", "HomePage", "Home"),
        Page("/tasks", "tasks", "TasksPage", "Tasks"),
        // ":id" becomes a route parameter; FromParameters passes it to the page's Id parameter.
        new RouteRecord(
            "/tasks/:id",
            name: "task",
            meta: Title("Task"),
            component: new ComponentNode(ComponentReference.ForName("TaskDetailPage")),
            argumentsResolver: RouteComponentArguments.FromParameters()),
        Page("/:pathMatch(.*)*", "not-found", "NotFoundPage", "Not found"),
    ];

    private static RouteRecord Page(string path, string name, string component, string title) =>
        new(
            path,
            name: name,
            meta: Title(title),
            component: new ComponentNode(ComponentReference.ForName(component)));

    private static Dictionary<string, object?> Title(string title) => new() { ["title"] = title };
}
