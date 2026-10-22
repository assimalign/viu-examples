using System;
using System.Threading.Tasks;
using Assimalign.Viu;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Cohesion.Viu.Hosting.Browser;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples;

var deferred = AsynchronousComponents.Define<Greeting>(new AsynchronousComponentOptions
{
    Loader = async cancellation =>
{
    await Task.Delay(250, cancellation);
    return new AsynchronousComponentTarget(ComponentNode.For<Greeting>().Component);
},
    Delay = 0,
    LoadingComponent = _ => new TextNode("Loading…")
}, name: "DeferredGreeting");
ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
builder.Services.AddComponents();
builder.Services.AddComponent(deferred.Registration);
builder.UseRootComponent<App>();
builder.ConfigureBrowser(options => options.MountTargetSelector = "#app");
await using ViuApplication application = builder.Build();
await application.RunAsync();
