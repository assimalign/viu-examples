using Assimalign.Cohesion.DependencyInjection;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Cohesion.Viu.Hosting.Browser;
using Assimalign.Viu.Examples;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
builder.Services.AddSingleton(new GreetingMessage("Hello from an application service"));
builder.Services.AddComponents();
builder.UseRootComponent<App>();
builder.ConfigureBrowser(options => options.MountTargetSelector = "#app");
await using ViuApplication application = builder.Build();
await application.RunAsync();
