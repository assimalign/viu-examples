using Assimalign.Viu;
using Assimalign.Viu.Components;
using Assimalign.Viu.Examples.Showcase.Components;
using Assimalign.Viu.Examples.Showcase.Components.Demos;
using Assimalign.Viu.Examples.Showcase.Components.Shared;
using Assimalign.Viu.Examples.Showcase.Components.Views;
using Assimalign.Viu.Router;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public static class ShowcaseComponentCatalog
{
    public static IComponentFactory CreateFactory()
        => new ComponentFactory(
        [
            Register<AppShell>("AppShell"),
            Register<FeatureCard>("FeatureCard"),
            Register<StatusPill>("StatusPill"),
            Register<RatingControl>("RatingControl"),
            Register<MetricPanel>("MetricPanel"),
            Register<QuotePanel>("QuotePanel"),
            Register<LifecycleProbe>("LifecycleProbe"),
            Register<LoadedInsight>("LoadedInsight"),
            Register<StateStorePanel>("StateStorePanel"),
            Register<OverviewView>("OverviewView"),
            Register<ReactivityView>("ReactivityView"),
            Register<ComponentsView>("ComponentsView"),
            Register<FormsView>("FormsView"),
            Register<MotionView>("MotionView"),
            Register<PlatformView>("PlatformView"),
            Register<UtilitiesView>("UtilitiesView"),
            Register<NotFoundView>("NotFoundView"),
            Register<RouterLink>("RouterLink"),
            Register<RouterView>("RouterView"),
            KeepAlive.Registration,
            Suspense.Registration,
            ShowcaseAsynchronousComponents.Insight.Registration,
        ]);

    private static ComponentRegistration Register<TComponent>(string name)
        where TComponent : class, IComponentTemplate, new()
        => new(
            typeof(TComponent),
            static () => new TComponent(),
            name);
}
