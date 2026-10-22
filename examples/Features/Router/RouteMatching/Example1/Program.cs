using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Router;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var matcher = new RouteMatcher([new RouteRecord("/people/:id", name: "person"), new RouteRecord("/people/new", name: "create")]);
    Console.WriteLine(matcher.Resolve("/people/new").Name); // create
    Console.WriteLine(matcher.Resolve("/people/42").Parameters.GetString("id")); // 42
});
