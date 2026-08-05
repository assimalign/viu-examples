using System.Collections.Generic;
using System.Threading.Tasks;

using Assimalign.Viu;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples.Showcase.Components;
using Assimalign.Viu.Examples.Showcase.Components.Views;
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
                component: ComponentTree.Template<AppShell>(),
                children:
                [
                    CreateRoute(
                        "",
                        "overview",
                        "Overview",
                        ComponentTree.Template<OverviewView>()),
                    CreateRoute(
                        "reactivity",
                        "reactivity",
                        "Reactivity",
                        ComponentTree.Template<ReactivityView>()),
                    new RouteRecord(
                        "components/:focus?",
                        name: "components",
                        meta: CreateMetadata("Components"),
                        component: ComponentTree.Template<ComponentsView>(),
                        argumentsResolver: RouteComponentArguments.FromParameters()),
                    CreateRoute(
                        "forms",
                        "forms",
                        "Forms",
                        ComponentTree.Template<FormsView>()),
                    new RouteRecord(
                        "motion",
                        name: "motion",
                        meta: CreateMetadata("Motion"),
                        component: ComponentTree.Template<MotionView>(),
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
                        ComponentTree.Template<PlatformView>()),
                    CreateRoute(
                        "utilities",
                        "utilities",
                        "Viu Utilities",
                        ComponentTree.Template<UtilitiesView>()),
                    new RouteRecord(
                        ":pathMatch(.*)*",
                        name: "not-found",
                        meta: CreateMetadata("Page not found"),
                        component: ComponentTree.Template<NotFoundView>(),
                        argumentsResolver: RouteComponentArguments.FromParameters()),
                ]),
        ];
    }

    private static RouteRecord CreateRoute(
        string path,
        string name,
        string title,
        IComponent component)
        => new(
            path,
            name: name,
            meta: CreateMetadata(title),
            component: component);

    private static IReadOnlyDictionary<string, object?> CreateMetadata(
        string title)
        => new Dictionary<string, object?>
        {
            ["title"] = title,
        };
}
