using System;
using System.Net.Http;
using System.Runtime.InteropServices.JavaScript;

using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Cohesion.Viu.Hosting.Browser;
using Assimalign.Cohesion.Viu.Routing;
using Assimalign.Viu;
using Assimalign.Viu.Router;
using Assimalign.Viu.Showcase.Cohesion.Client;
using Assimalign.Viu.Showcase.Cohesion.Client.Services;
using Assimalign.Viu.Showcase.Cohesion.Shared;
using Assimalign.Viu.State;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();

// The HttpClient is created here because AddClientConfiguration borrows it before the container
// exists; registering the same instance lets services and components share it.
using HttpClient http = new() { BaseAddress = ResolveBaseAddress() };

// ---------------------------------------------------------------------------------------------
// Dependency injection. Registrations are instances or factories only: WebAssembly is trimmed and
// AOT-compiled, so nothing is activated by reflection.
// ---------------------------------------------------------------------------------------------
builder.Services.AddSingleton(http);
builder.Services.AddSingleton<ITaskCatalog>(services => new HttpTaskCatalog(services.GetRequiredService<HttpClient>()));
builder.Services.AddSingleton(_ => new GreetingService(builder.Environment.Name ?? "unknown"));

// Application state: an unset ApplicationOptions.State is resolved from the container.
builder.Services.AddSingleton<IStateStoreRegistry>(services =>
    StateStores.CreateRegistry(services, new ApplicationWatchScheduler()));

// Components: this app's .viu files and the component library's.
builder.Services.AddComponents();

// Routing: registers the router, browser (path) history, RouterView, and RouterLink. Deep links
// work because the server answers unknown paths with index.html.
builder.Services.AddRouting(routing =>
{
    routing.Routes.Add("/", "HomePage", "Home");
    routing.Routes.Add("/tasks", "TasksPage", "Tasks");
    routing.Routes.Add("/:pathMatch(.*)*", "NotFoundPage", "Not found");
});

// ---------------------------------------------------------------------------------------------
// Configuration and browser host.
// ---------------------------------------------------------------------------------------------
// Loads the server's public /_cohesion/client.json into IConfiguration before components start.
builder.AddClientConfiguration(http);

builder.UseRootComponent<App>();
builder.ConfigureBrowser(options => options.MountTargetSelector = "#app");

await using ViuApplication application = builder.Build();
await application.UseRouter().RunAsync();

static Uri ResolveBaseAddress()
{
    using JSObject document = JSHost.GlobalThis.GetPropertyAsJSObject("document")
        ?? throw new InvalidOperationException("The browser document is unavailable.");
    return new Uri(document.GetPropertyAsString("baseURI")!, UriKind.Absolute);
}
