using System.Threading.Tasks;

using Assimalign.Viu;
using Assimalign.Viu.Components;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public static class ShowcaseAsynchronousComponents
{
    public static AsynchronousComponentDefinition Insight { get; } =
        AsynchronousComponents.Define<InsightIdentity>(
            async cancellationToken =>
            {
                await Task.Delay(900, cancellationToken);
                return new AsynchronousComponentTarget("LoadedInsight");
            },
            name: "AsyncInsight");

    private sealed class InsightIdentity : IComponent
    {
        public ComponentRenderer Setup(ComponentContext context) =>
            static _ => null;
    }
}
