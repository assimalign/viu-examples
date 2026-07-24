using System;

using ViuRouter = Assimalign.Viu.Router.Router;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public sealed class ShowcaseServiceProvider : IServiceProvider
{
    private readonly ViuRouter _router;
    private readonly ShowcaseRuntimeStatus _runtimeStatus;

    public ShowcaseServiceProvider(
        ViuRouter router,
        ShowcaseRuntimeStatus runtimeStatus)
    {
        ArgumentNullException.ThrowIfNull(router);
        ArgumentNullException.ThrowIfNull(runtimeStatus);
        _router = router;
        _runtimeStatus = runtimeStatus;
    }

    public object? GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);
        if (serviceType == typeof(ViuRouter))
        {
            return _router;
        }

        return serviceType == typeof(ShowcaseRuntimeStatus)
            ? _runtimeStatus
            : null;
    }
}
