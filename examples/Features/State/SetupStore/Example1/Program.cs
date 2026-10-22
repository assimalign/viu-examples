using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Reactivity;
using Assimalign.Viu.State;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    using var registry = StateStores.CreateRegistry();
    var definition = StateStores.Define("counter", () => Reactive.Reference(0));
    var first = definition.Use(registry);
    var second = definition.Use(registry);
    first.Value++;
    Console.WriteLine(second.Value); // 1: both consumers share the registry-owned store.
});
