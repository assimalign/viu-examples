using System.Collections.Generic;
using System.Threading.Tasks;
using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Cohesion.Viu.Hosting.Browser;
using Assimalign.Cohesion.Viu.Routing;
using Assimalign.Viu.Router;

namespace Assimalign.Viu.Examples;

internal static class RoutingBootstrap
{
    internal static async Task RunAsync(IRouterHistory history, IReadOnlyList<RouteRecord> routes)
    {
        using (history)
        {
            ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
            builder.Services.AddComponents();
            builder.Services.AddSingleton<IRouterHistory>(history);
            builder.Services.AddRouting();
            builder.Services.AddRoutes(_ => routes);
            builder.UseRootComponent<App>();
            builder.ConfigureBrowser(options => options.MountTargetSelector = "#app");
            await using ViuApplication application = builder.Build();
            await application.UseRouter().RunAsync();
        }
    }
}
