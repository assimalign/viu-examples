using System;
using System.Threading;
using System.Threading.Tasks;

using Assimalign.Viu;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public sealed class ShowcasePlugin : IApplicationPlugin
{
    private readonly ShowcaseRuntimeStatus _runtimeStatus;

    public ShowcasePlugin(ShowcaseRuntimeStatus runtimeStatus)
    {
        ArgumentNullException.ThrowIfNull(runtimeStatus);
        _runtimeStatus = runtimeStatus;
    }

    public async ValueTask InstallAsync(
        IApplication application,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(application);
        await Task.Yield();
        cancellationToken.ThrowIfCancellationRequested();
        _runtimeStatus.RecordPluginInstallation();
    }
}
