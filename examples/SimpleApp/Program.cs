using System;
using System.Threading;
using System.Threading.Tasks;

using Assimalign.Viu.Browser;
using Assimalign.Viu.Components;

using SimpleApp.Components;

IComponentFactory components = new ComponentFactory(
[
    new ComponentRegistration(
        typeof(Counter),
        static () => new Counter(),
        "Counter"),
]);

BrowserApplicationBuilder builder =
    BrowserApplication.CreateBuilder(ComponentTree.Template<Counter>());
builder.UseComponentFactory(components);
builder.UseServiceProvider(EmptyServiceProvider.Instance);

await using BrowserApplication application = builder.Build();
await application.MountAsync("#app");
await Task.Delay(Timeout.Infinite);

file sealed class EmptyServiceProvider : IServiceProvider
{
    internal static EmptyServiceProvider Instance { get; } = new();

    private EmptyServiceProvider()
    {
    }

    public object? GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        return null;
    }
}
