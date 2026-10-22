using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Router;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, async () =>
{
    using var history = RouterHistory.CreateMemory();
    using var router = new Router(history, [new RouteRecord("/"), new RouteRecord("/people/:id")]);
    await router.PushAsync("/people/42");
    Console.WriteLine(router.CurrentRoute.Value.Parameters.GetString("id"));
});
