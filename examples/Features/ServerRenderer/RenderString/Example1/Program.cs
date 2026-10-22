using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples;
using Assimalign.Viu.ServerRenderer;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, async () =>
{
    var root = new ElementNode(new QualifiedName("p"), children: [new TextNode("<safe> & text")]);
    Console.WriteLine(await ServerRender.RenderToStringAsync(root));
});
