using System.Threading.Tasks;

using Assimalign.Viu;
using Assimalign.Viu.Examples.Showcase.Components.Demos;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public static class ShowcaseAsynchronousComponents
{
    public static AsynchronousComponentDefinition Insight { get; } =
        AsynchronousComponents.DefineAsynchronousComponent<InsightIdentity>(
            async cancellationToken =>
            {
                await Task.Delay(900, cancellationToken);
                return AsynchronousComponentTarget.From<LoadedInsight>();
            },
            name: "AsyncInsight");

    private sealed class InsightIdentity;
}
