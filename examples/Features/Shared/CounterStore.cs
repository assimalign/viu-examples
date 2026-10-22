using Assimalign.Viu.State;

namespace Assimalign.Viu.Examples;

internal sealed class CounterStore() : StateStore<CounterState>("counter", () => new CounterState(), (target, source) => target.Count = source.Count)
{
    internal void Increment() => RunAction("increment", () => State.Count++);
}
