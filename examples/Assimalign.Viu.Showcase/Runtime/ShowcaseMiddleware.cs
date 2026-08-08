using System;
using System.Threading.Tasks;

using Assimalign.Viu;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public sealed class ShowcaseMiddleware
{
    private readonly ShowcaseRuntimeStatus _runtimeStatus;

    public ShowcaseMiddleware(ShowcaseRuntimeStatus runtimeStatus)
    {
        ArgumentNullException.ThrowIfNull(runtimeStatus);
        _runtimeStatus = runtimeStatus;
    }

    public async ValueTask InvokeAsync(
        IApplicationContext context,
        ApplicationDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        await Task.Yield();
        context.Stopping.ThrowIfCancellationRequested();
        _runtimeStatus.RecordMiddlewareEntry();
        await next(context);
    }
}
