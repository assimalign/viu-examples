using System;

using Assimalign.Viu;
using Assimalign.Viu.Browser;
using Assimalign.Viu.Browser.Router;
using Assimalign.Viu.Components;
using Assimalign.Viu.Router;
using Assimalign.Viu.Showcase.StandAlone;
using Assimalign.Viu.Showcase.StandAlone.Components;
using Assimalign.Viu.Showcase.StandAlone.Routing;
using Assimalign.Viu.Showcase.StandAlone.Services;
using Assimalign.Viu.State;

using LibraryComponents = Assimalign.Viu.Showcase.Components.GeneratedViuComponents;
using ViuRouter = Assimalign.Viu.Router.Router;

// 1. Services: a plain IServiceProvider the components read through Context.Services.
AppServices services = new();

// 2. Routing: hash history keeps deep links refresh-safe on the static dev server.
using IRouterHistory history = BrowserRouterHistory.CreateWebHash();
using ViuRouter router = new(history, AppRoutes.Create());
services.Router = router;

// 3. Application state: one registry, so every TaskStore.Definition.Use(...) shares one store.
using IStateStoreRegistry state = StateStores.CreateRegistry(services, new ApplicationWatchScheduler());
services.State = state;

// 4. Components: compiled .viu files from this app and the library, the router's components
//    (templates reference them by tag name), and one code-first component.
ComponentFactory components = new();
GeneratedViuComponents.Register(components);
LibraryComponents.Register(components);
components.Register(new ComponentRegistration(
    ComponentReference.ForName("RouterView"), RouterView.Registration.Contract, RouterView.Registration.Activator));
components.Register(new ComponentRegistration(
    ComponentReference.ForName("RouterLink"), RouterLink.Registration.Contract, RouterLink.Registration.Activator));
components.Register(CodeFirstComponents.Footnote);

await using BrowserApplication application = new BrowserApplicationBuilder()
    .ConfigureApplication(options =>
    {
        options.RootComponent = new ComponentNode(ComponentReference.ForName("App"));
        options.Components = components;
        options.Services = services;
        options.State = state;
        options.WarnHandler = Console.Error.WriteLine;
    })
    .ConfigureBrowser(options => options.MountTargetSelector = "#app")
    .Build();

// UseRouter settles the initial navigation before the root mounts.
await application
    .UseRouter(router)
    .RunAsync();
