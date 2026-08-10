using System;
using System.Threading.Tasks;

using Assimalign.Viu;
using Assimalign.Viu.Browser;
using Assimalign.Viu.Browser.Router;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples.Showcase.Routing;
using Assimalign.Viu.Examples.Showcase.Runtime;
using Assimalign.Viu.Router;
using Assimalign.Viu.State;

using ViuRouter = Assimalign.Viu.Router.Router;

ShowcaseRuntimeStatus runtimeStatus = new();
ComponentFactory components = ShowcaseComponentCatalog.CreateFactory();
using IRouterHistory history = BrowserRouterHistory.CreateWebHash();
using ViuRouter router = new(history, ShowcaseRoutes.Create(runtimeStatus));
ShowcaseServiceProvider services = new(router, runtimeStatus);
using IStateStoreRegistry state = StateStores.CreateRegistry(
    services,
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
await using BrowserApplication application = new BrowserApplicationBuilder()
    .ConfigureApplication(
        options =>
        {
            options.RootComponent = new ComponentNode(
                RouterView.Registration.Reference);
            options.Components = components;
            options.Services = services;
            options.State = state;
            options.WarnHandler = message =>
                runtimeStatus.RecordWarning(message);
            options.ErrorHandler = (exception, _, source) =>
                runtimeStatus.RecordError(source, exception);
        })
    .ConfigureBrowser(
        options => options.MountTargetSelector = "#app")
    .Build();

var showcaseMiddleware = new ShowcaseMiddleware(runtimeStatus);
await application
    .Use(showcaseMiddleware.InvokeAsync)
    .UseRouter(router)
    .RunAsync();
