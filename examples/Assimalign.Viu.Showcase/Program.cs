using System;
using System.Threading;
using System.Threading.Tasks;

using Assimalign.Viu;
using Assimalign.Viu.Browser;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples.Showcase.Components;
using Assimalign.Viu.Examples.Showcase.Routing;
using Assimalign.Viu.Examples.Showcase.Runtime;
using Assimalign.Viu.Examples.Showcase.State;
using Assimalign.Viu.Reactivity;
using Assimalign.Viu.Router;
using Assimalign.Viu.Router.Browser;
using Assimalign.Viu.State;

using ViuRouter = Assimalign.Viu.Router.Router;

IComponentFactory components = ShowcaseComponentCatalog.CreateFactory();
ShowcaseRuntimeStatus runtimeStatus = new();
await RouterHistory.InitializeAsync();
IRouterHistory history = RouterHistory.CreateWebHash();
ViuRouter router = new(history, ShowcaseRoutes.Create(runtimeStatus));
ShowcaseServiceProvider services = new(router, runtimeStatus);

using StateStoreRegistry state = StateStores.CreateRegistry(
    components,
    services,
    new ReactiveEffectScopeFactory(),
    new ApplicationWatchScheduler());

router.BeforeEach(
    (to, from, _) =>
    {
        runtimeStatus.RecordNavigationGuard(from.Path, to.Path);
        return Task.FromResult(NavigationGuardResult.Allow);
    });
router.AfterEach(
    (to, _, failure) =>
        runtimeStatus.RecordNavigation(
            to.Path,
            failure is null ? "completed" : failure.Type.ToString()));
await router.ReadyAsync();

BrowserApplicationBuilder builder = BrowserApplication.CreateBuilder(
    ComponentTree.Template<RouterView>(),
    useCommandBuffer: true);
builder.UseComponentFactory(components);
builder.UseServiceProvider(services);
builder.UseStateRegistry(state);
builder.Use(new ShowcasePlugin(runtimeStatus));
builder.ConfigureApplication(
    context =>
    {
        context.Performance = true;
        context.WarnHandler = message =>
            runtimeStatus.RecordWarning(message);
        context.ErrorHandler = (exception, _, source) =>
            runtimeStatus.RecordError(source, exception);
    });

try
{
    RouterLinkDomBridge.Install();
    await using BrowserApplication application = builder.Build();
    await application.MountAsync("#app");
    await Task.Delay(Timeout.Infinite);
}
finally
{
    RouterLinkDomBridge.Uninstall();
    router.Dispose();
    history.Destroy();
}
