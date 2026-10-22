using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Reactivity;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var query = Reactive.Reference("first");
    using var watch = Reactive.WatchEffect(cleanup =>
    {
        Console.WriteLine(query.Value);
        cleanup(() => Console.WriteLine("Cleanup"));
    });
    query.Value = "second";
});
