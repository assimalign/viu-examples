using System.IO;
using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples;
using Assimalign.Viu.ServerRenderer;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, async () =>
{
    var generator = new StaticSiteGenerator(() => new ElementNode(new QualifiedName("h1"), children: [new TextNode("Viu static page")]), new PageScopes());
    var shell = new HostPageDocumentShell("<!doctype html><html><body><main id=\"app\"></main></body></html>");
    var files = await generator.GenerateAsync(["/", "/guide"], shell, new FileSystemStaticSiteOutput(Path.Combine(AppContext.BaseDirectory, "site")));
    Console.WriteLine(files.Count); // 2: index.html and guide/index.html.
});
