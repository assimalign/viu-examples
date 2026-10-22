using System.Threading;
using System.Threading.Tasks;
using Assimalign.Viu.Components;
using Assimalign.Viu.ServerRenderer;

namespace Assimalign.Viu.Examples;

internal sealed class PageScopes : IServerRenderRequestScopeFactory<StaticSiteRouteContext>
{
    public ValueTask<IServerRenderRequestScope> CreateAsync(ServerRenderRequest<StaticSiteRouteContext> request, CancellationToken cancellationToken = default) => ValueTask.FromResult<IServerRenderRequestScope>(new PageScope(request));
    private sealed class PageScope(ServerRenderRequest<StaticSiteRouteContext> request) : IServerRenderRequestScope
    {
        public ServerRenderApplication Application { get; } = new(request.RootComponent, new ComponentFactory());
        public SsrContext RenderContext { get; } = new();

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
