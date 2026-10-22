using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Reactivity;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var selected = new ReactiveSet<string>();
    var effect = Reactive.Effect(() => Console.WriteLine(selected.Count));
    selected.Add("first");
    selected.Remove("first");
    effect.Stop();
});
