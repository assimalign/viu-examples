using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Http;
using Assimalign.Cohesion.Viu.Server;
using Assimalign.Cohesion.Web;
using Assimalign.Cohesion.Web.Hosting;
using Assimalign.Cohesion.Web.Routing;
using Assimalign.Cohesion.Web.Serialization;
using Assimalign.Viu.Components;
using Assimalign.Viu.Showcase.Cohesion.Server.Rendering;
using Assimalign.Viu.Showcase.Cohesion.Server.Services;
using Assimalign.Viu.Showcase.Cohesion.Shared;
using Assimalign.Viu.Showcase.Cohesion.Shared.Pages;

// One catalog instance backs both the HTTP API and the server-rendered components.
InMemoryTaskCatalog catalog = new();

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// One Web container supplies both the HTTP pipeline and server-rendered components.
// Generated catalogs include Shared and its referenced component library.
builder.Services
    .AddSingleton<ITaskCatalog>(catalog)
    .AddRouting()
    .AddJsonSerialization(ShowcaseJsonContext.Default)
    .AddViuServerRendering(rendering =>
    {
        rendering.RootComponent = ComponentNode.For<ServerReport>();
        rendering.DocumentShell = _ => ReportDocumentShell.Instance;
    })
    .AddViuClient();

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
application.UseViuServerRenderer(context => context.Request.Path.Value == "/report");

// Everything else is the browser client.
application.UseViuApplication();

await application.RunAsync();
