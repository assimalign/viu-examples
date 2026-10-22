using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.State;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var storage = new MemoryStateStorage();
    var definition = StateStores.Define("counter", () => new CounterStore(), new CounterSerializer(), new StateStorePersistenceDescriptor("example-counter"));
    using (var first = StateStores.CreateRegistry())
    {
        first.Use(new StateStorePersistencePlugin(new StateStorePersistenceOptions(storage)));
        definition.Use(first).Patch(state => state.Count = 7);
    }

    using var second = StateStores.CreateRegistry();
    second.Use(new StateStorePersistencePlugin(new StateStorePersistenceOptions(storage)));
    Console.WriteLine(definition.Use(second).State.Count); // 7
});
