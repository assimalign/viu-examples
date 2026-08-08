using System.Collections.Generic;
using System.Threading.Tasks;

using Assimalign.Viu;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples.Showcase.Runtime;
using Assimalign.Viu.Router;

namespace Assimalign.Viu.Examples.Showcase.Routing;

public static class ShowcaseRoutes
{
    public static IReadOnlyList<RouteRecord> Create(
        ShowcaseRuntimeStatus runtimeStatus)
    {
        return
        [
            new RouteRecord(
                "/",
                name: "showcase",
                meta: new Dictionary<string, object?>
                {
                    ["application"] = "Viu SDK Showcase",
                },
                component: Component("AppShell"),
                children:
                [
                    CreateRoute(
                        "",
                        "overview",
                        "Overview",
                        Component("OverviewView")),
                    CreateRoute(
                        "reactivity",
                        "reactivity",
                        "Reactivity",
                        Component("ReactivityView")),
                    new RouteRecord(
                        "components/:focus?",
                        name: "components",
                        meta: CreateMetadata("Components"),
                        component: Component("ComponentsView"),
                        argumentsResolver: RouteComponentArguments.FromParameters()),
                    CreateRoute(
                        "forms",
                        "forms",
                        "Forms",
                        Component("FormsView")),
                    new RouteRecord(
                        "motion",
                        name: "motion",
                        meta: CreateMetadata("Motion"),
                        component: Component("MotionView"),
                        beforeEnter: (to, _, _) =>
                        {
                            runtimeStatus.AddDiagnostic(
                                $"Per-route guard allowed {to.Path}");
                            return Task.FromResult(NavigationGuardResult.Allow);
                        }),
                    CreateRoute(
                        "platform",
                        "platform",
                        "Platform",
                        Component("PlatformView")),
                    CreateRoute(
                        "utilities",
                        "utilities",
                        "Viu Utilities",
                        Component("UtilitiesView")),
                    new RouteRecord(
                        ":pathMatch(.*)*",
                        name: "not-found",
                        meta: CreateMetadata("Page not found"),
                        component: Component("NotFoundView"),
                        argumentsResolver: RouteComponentArguments.FromParameters()),
                ]),
        ];
    }

    private static RouteRecord CreateRoute(
        string path,
        string name,
        string title,
        VirtualNode component)
        => new(
            path,
            name: name,
            meta: CreateMetadata(title),
            component: component);

    private static ComponentNode Component(string name)
        => new(ComponentReference.ForName(name));

    private static IReadOnlyDictionary<string, object?> CreateMetadata(
        string title)
        => new Dictionary<string, object?>
        {
            ["title"] = title,
        };
}
