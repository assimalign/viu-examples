using Assimalign.Viu.State;

namespace Assimalign.Viu.Examples.Showcase.State;

public sealed class StateStoreDemonstrationStore
    : StateStore<StateStoreDemonstrationState>
{
    public StateStoreDemonstrationStore()
        : base(
            "platform-state-store",
            CreateInitialState,
            ApplyState)
    {
    }

    public int Advance()
        => RunAction(
            nameof(Advance),
            () =>
            {
                State.Count += State.Step;
                State.LastOperation = $"Observed action advanced by {State.Step}";
                return State.Count;
            });

    private static StateStoreDemonstrationState CreateInitialState()
        => new()
        {
            Count = 2,
            Step = 1,
            LastOperation = "Fresh state from the factory",
        };

    private static void ApplyState(
        StateStoreDemonstrationState target,
        StateStoreDemonstrationState source)
    {
        target.Count = source.Count;
        target.Step = source.Step;
        target.LastOperation = source.LastOperation;
    }
}
