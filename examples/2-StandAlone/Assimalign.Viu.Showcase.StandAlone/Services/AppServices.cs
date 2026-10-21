using System;

using Assimalign.Viu.State;

using ViuRouter = Assimalign.Viu.Router.Router;

namespace Assimalign.Viu.Showcase.StandAlone.Services;

/// <summary>
/// The app's service provider. Viu only needs an <see cref="IServiceProvider"/>; without a DI
/// container the composition root answers each service type explicitly, which keeps activation
/// trimming- and AOT-safe. Components read these through <c>Context.Services</c>.
/// (3-Cohesion shows the same seam backed by the Cohesion DI container.)
/// </summary>
public sealed class AppServices : IServiceProvider
{
    private readonly ITaskSeedSource _seedSource = new SampleTaskSeedSource();

    public ViuRouter? Router { get; set; }

    public IStateStoreRegistry? State { get; set; }

    public object? GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        if (serviceType == typeof(ViuRouter))
        {
            return Router;
        }

        if (serviceType == typeof(IStateStoreRegistry))
        {
            return State;
        }

        return serviceType == typeof(ITaskSeedSource) ? _seedSource : null;
    }
}
