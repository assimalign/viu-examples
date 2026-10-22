using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Examples;
using Assimalign.Viu.UtilityCss;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    var result = UtilityCandidateParser.Parse("hover:bg-[#123456]/50");
    Console.WriteLine($"Diagnostics: {result.Diagnostics.Count}");
});
