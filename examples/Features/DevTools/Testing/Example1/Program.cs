using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples;
using Assimalign.Viu.Testing;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    using var renderer = new TestRenderer();
    var container = renderer.CreateContainer();
    renderer.Render(new ElementNode(new QualifiedName("p"), children: [new TextNode("Hello")]), container);
    Console.WriteLine(container.Children.Count); // 1
    renderer.Render(null, container);
    Console.WriteLine(container.Children.Count); // 0 after unmount
});
