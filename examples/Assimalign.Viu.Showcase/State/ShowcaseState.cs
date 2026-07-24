using Assimalign.Viu.State;

namespace Assimalign.Viu.Examples.Showcase.State;

public static class ShowcaseState
{
    public static StateStoreDefinition<ShowcaseStore> Definition { get; } =
        StateStores.Define(
            "viu-showcase",
            static () => new ShowcaseStore());

    public static StateStoreDefinition<StateStoreDemonstrationStore>
        StateStoreDemonstrationDefinition { get; } =
            StateStores.Define(
                "platform-state-store",
                static () => new StateStoreDemonstrationStore());
}
