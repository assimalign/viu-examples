using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.State;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    using var registry = StateStores.CreateRegistry();
    var definition = StateStores.Define("counter", () => new CounterStore());
    var counter = definition.Use(registry);
    using var subscription = counter.OnAction(context => Console.WriteLine(context.Name));
    counter.Increment();
    Console.WriteLine(counter.State.Count); // 1
});
