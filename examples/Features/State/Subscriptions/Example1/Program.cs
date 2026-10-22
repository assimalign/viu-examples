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
    using var subscription = counter.Subscribe((mutation, state) => Console.WriteLine(state.Count));
    counter.Patch(state => state.Count = 3);
});
