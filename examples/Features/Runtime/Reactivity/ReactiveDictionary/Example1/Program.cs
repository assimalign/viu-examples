using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Reactivity;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var values = new ReactiveDictionary<string, int>
    {
        ["count"] = 0
    };
    var effect = Reactive.Effect(() => Console.WriteLine(values["count"]));
    values["count"] = 1;
    effect.Stop();
});
