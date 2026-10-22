using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Reactivity;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var count = Reactive.Reference(0);
    var scope = Reactive.EffectScope();
    scope.Run(() => Reactive.Effect(() => Console.WriteLine(count.Value)));
    count.Value = 1; // Prints 1.
    scope.Stop();
    count.Value = 2; // No effect runs after stop.
});
