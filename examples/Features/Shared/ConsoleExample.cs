using System;
using System.Threading.Tasks;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu;
using Assimalign.Viu.Components;
using Assimalign.Viu.Testing;

namespace Assimalign.Viu.Examples;
// Finite console demos render an empty root in memory while exercising the real host lifetime.
internal static class ConsoleExample
{
    internal static Task RunAsync(ViuApplicationBuilder builder, Action demonstrate) => RunAsync(builder, () =>
    {
        demonstrate();
        return Task.CompletedTask;
    });
    internal static async Task RunAsync(ViuApplicationBuilder builder, Func<Task> demonstrate)
    {
        builder.ConfigureApplication(options => options.RootComponent = new CommentNode("Console example"));
        builder.ConfigureHost(new ExampleHostFactory(context => ValueTask.FromResult<IDisposable>(new ConsoleMount(context))));
        await using ViuApplication application = builder.Build();
        await application.StartAsync();
        try
        {
            await demonstrate();
        }
        finally
        {
            await application.StopAsync();
        }
    }

    private sealed class ConsoleMount : IDisposable
    {
        private readonly TestRenderer renderer = new();
        private readonly TestElement container;
        private readonly IApplicationContext context;
        internal ConsoleMount(IApplicationContext context)
        {
            this.context = context;
            container = renderer.CreateContainer();
            try
            {
                renderer.Render(context.RootComponent, container, context);
            }
            catch
            {
                renderer.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                renderer.Render(null, container, context);
            }
            finally
            {
                renderer.Dispose();
            }
        }
    }
}
