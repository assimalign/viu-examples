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
    counter.Patch(state => state.Count += 2);
    Console.WriteLine(counter.State.Count); // 2
    counter.Reset();
    Console.WriteLine(counter.State.Count); // 0
});
