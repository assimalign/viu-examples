using System.Collections.Generic;

using Assimalign.Viu.Router;
using Assimalign.Viu.Showcase.StandAlone.Pages;

namespace Assimalign.Viu.Showcase.StandAlone.Routing;

/// <summary>The route table, using registered names and typed component definitions.</summary>
public static class AppRoutes
{
    public static IReadOnlyList<RouteRecord> Create()
    {
        List<RouteRecord> routes = [];
        routes.Add("/", "HomePage", "Home");
        routes.Add("/tasks", "TasksPage", "Tasks");
        // ":id" becomes the page's Id parameter.
        routes.Add<TaskDetailPage>("/tasks/:id", "Task",
            argumentsResolver: RouteComponentArguments.FromParameters());
        routes.Add("/:pathMatch(.*)*", "NotFoundPage", "Not found");
        return routes;
    }
}
