using System.IO;
using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples;
using Assimalign.Viu.ServerRenderer;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, async () =>
{
    using var writer = new StringWriter();
    await ServerRender.RenderToStreamAsync(new ElementNode(new QualifiedName("h1"), children: [new TextNode("Hello")]), writer);
    Console.WriteLine(writer.ToString());
});
