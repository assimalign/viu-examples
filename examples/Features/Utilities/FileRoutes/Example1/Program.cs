using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.FileRouting;
using Assimalign.Viu.Router;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var routes = FileRoutes.Create([new FileRouteDescriptor("/people/:id", "person", "PersonPage", forwardParameters: true)]);
    var matcher = new RouteMatcher(routes);
    Console.WriteLine(matcher.Resolve("/people/42").Parameters.GetString("id"));
});
