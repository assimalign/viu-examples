using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.State;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var definition = StateStores.Define("counter", () => new CounterStore(), new CounterSerializer());
    using var source = StateStores.CreateRegistry();
    definition.Use(source).State.Count = 42;
    StateStorePayload payload = ((IStateStorePayloadRegistry)source).CapturePayload();
    using var destination = StateStores.CreateRegistry();
    ((IStateStorePayloadRegistry)destination).RestorePayload(payload);
    Console.WriteLine(definition.Use(destination).State.Count); // 42
});
