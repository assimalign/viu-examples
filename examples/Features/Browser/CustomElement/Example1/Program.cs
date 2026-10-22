using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.Browser;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
builder.Services.AddComponents();
builder.UseRootComponent<App>();
builder.ConfigureHost(new ExampleHostFactory(async context =>
{
    await BrowserRuntime.InitializeAsync();
    var elements = new BrowserCustomElements(context.Components, context.Services);
    try
    {
        elements.Define(ComponentNode.For<App>().Component, "viu-counter");
        return elements;
    }
    catch
    {
        elements.Dispose();
        throw;
    }
}));
await using ViuApplication application = builder.Build();
await application.RunAsync();
