using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu;

namespace Assimalign.Viu.Examples;
// Explicit terminal ownership keeps the console and custom-element demos inside Cohesion's lifetime.
internal sealed class ExampleHostFactory(Func<IApplicationContext, ValueTask<IDisposable>> mount) : IViuApplicationHostFactory
{
    public IApplication CreateApplication(ApplicationOptions options) => new ExampleRuntime(options, mount);
    private sealed class ExampleRuntime : IApplication
    {
        private readonly Func<IApplicationContext, ValueTask<IDisposable>> mount;
        private readonly ApplicationLifetime lifetime;
        private readonly List<ApplicationMiddleware> middleware = [];
        private readonly TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private CancellationTokenRegistration cancellation;
        private Task? execution;
        internal ExampleRuntime(ApplicationOptions options, Func<IApplicationContext, ValueTask<IDisposable>> mount)
        {
            Context = new global::Assimalign.Viu.ApplicationContext(options);
            lifetime = new ApplicationLifetime((global::Assimalign.Viu.ApplicationContext)Context);
            this.mount = mount;
        }

        public IApplicationContext Context
        {
            get;
        }

        public IApplication Use(ApplicationMiddleware value)
        {
            if (execution is not null)
            {
                throw new InvalidOperationException("The example host has started.");
            }

            middleware.Add(value);
            return this;
        }

        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            lifetime.StartExecution();
            cancellation = cancellationToken.Register(lifetime.RequestStopping);
            execution = ExecuteAsync();
            return new ValueTask(started.Task);
        }

        public async ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            lifetime.RequestStopping();
            if (execution is not null)
            {
                await execution.WaitAsync(cancellationToken);
            }
        }

        private async Task ExecuteAsync()
        {
            ApplicationDelegate pipeline = MountAsync;
            for (int index = middleware.Count - 1; index >= 0; index--)
            {
                ApplicationMiddleware current = middleware[index];
                ApplicationDelegate next = pipeline;
                pipeline = context => current(context, next);
            }

            try
            {
                await pipeline(Context);
            }
            catch (Exception exception)
            {
                lifetime.Fail(exception);
                started.TrySetException(exception);
                throw;
            }
            finally
            {
                lifetime.CompleteStopping();
            }
        }

        private async ValueTask MountAsync(IApplicationContext context)
        {
            IDisposable? terminal = null;
            try
            {
                lifetime.Stopping.ThrowIfCancellationRequested();
                terminal = await mount(context);
                lifetime.SignalRunning();
                started.TrySetResult();
                try
                {
                    await Task.Delay(Timeout.Infinite, lifetime.Stopping);
                }
                catch (OperationCanceledException) when (lifetime.Stopping.IsCancellationRequested)
                {
                }
            }
            finally
            {
                terminal?.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await StopAsync();
            }
            finally
            {
                cancellation.Dispose();
                lifetime.Dispose();
            }
        }
    }
}
