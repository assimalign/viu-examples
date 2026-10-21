using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Cohesion.Viu.Server;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.Serialization;
using Assimalign.Viu.Components;
using Assimalign.Viu.ServerRenderer;
using Assimalign.Viu.Showcase.Cohesion.Server.Rendering;
using Assimalign.Viu.Showcase.Cohesion.Server.Services;
using Assimalign.Viu.Showcase.Cohesion.Shared;

using LibraryComponents = Assimalign.Viu.Showcase.Components.GeneratedViuComponents;
using LibraryServerRenders = Assimalign.Viu.Showcase.Components.GeneratedViuServerRenders;
using SharedComponents = Assimalign.Viu.Showcase.Cohesion.Shared.GeneratedViuComponents;
using SharedServerRenders = Assimalign.Viu.Showcase.Cohesion.Shared.GeneratedViuServerRenders;

// One catalog instance backs both the HTTP API and the server-rendered components.
InMemoryTaskCatalog catalog = new();

// ---------------------------------------------------------------------------------------------
// 1. The server-side Viu application: the composition every server render borrows.
//    Same builder as the browser client, but ConfigureServerRendering selects the server runtime.
// ---------------------------------------------------------------------------------------------
ViuApplicationBuilder viuBuilder = ViuApplication.CreateBuilder();

// Services that server-rendered components resolve through Context.Services.
viuBuilder.Services.AddSingleton<ITaskCatalog>(catalog);

// Components: the shared pages plus the library they use. Each assembly has its own generated catalog.
viuBuilder.Services.AddComponents(SharedComponents.Register);
viuBuilder.Services.AddComponents(LibraryComponents.Register);

viuBuilder.ConfigureServerRendering();
viuBuilder.ConfigureApplication(options =>
    options.RootComponent = new ComponentNode(ComponentReference.ForName("ServerReport")));

await using ViuApplication viuApplication = viuBuilder.Build();

// The generated server-markup bodies, registered explicitly (no assembly scanning).
ServerRenderRegistry serverRenders = new();
SharedServerRenders.Register(serverRenders);
LibraryServerRenders.Register(serverRenders);

// ---------------------------------------------------------------------------------------------
// 2. The Cohesion web host.
// ---------------------------------------------------------------------------------------------
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Services for the HTTP pipeline (the web host has its own container).
builder.Services
    .AddSingleton<ITaskCatalog>(catalog)
    .AddRouting()
    .AddJsonSerialization(ShowcaseJsonContext.Default);

// Starts and stops the server-side Viu application with the web host.
builder.AddViuServerApplication(viuApplication);

// Serves the WebAssembly client: its static assets, /_cohesion/client.json, and an index.html
// fallback for client-side routes. UseClientOutput finds the client build via ViuClientReference.
builder.AddViuApplication(options => options.UseClientOutput("Assimalign.Viu.Showcase.Cohesion.Client"));

await using WebApplication application = builder.Build();

// JSON API. Endpoints run at the end of the pipeline, so /api requests go into a branch that
// ends there immediately and never reach the client's index.html fallback below.
application.UseRouting();
application.MapGet("/api/tasks", () => catalog.GetSnapshot());
application.MapPost("/api/tasks", (NewTaskRequest request, IHttpContext context) =>
{
    context.Response.StatusCode = HttpStatusCode.Created;
    return catalog.Add(request.Title);
});
application.Map("/api", _ => { });

// Server rendering: GET /report renders the ServerReport component to HTML inside the shell.
application.UseViuServerRenderer(
    viuApplication,
    serverRenders,
    _ => ReportDocumentShell.Instance,
    context => context.Request.Path.Value == "/report");

// Everything else is the browser client.
application.UseViuApplication();

await application.RunAsync();
