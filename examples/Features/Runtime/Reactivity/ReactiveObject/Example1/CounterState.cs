using Assimalign.Viu.Reactivity;

namespace Assimalign.Viu.Examples;
/// <summary>A state object with generated dependency tracking.</summary>
[Reactive]
public partial class CounterState
{
    /// <summary>Gets or sets the observed counter.</summary>
    public partial int Count
    {
        get; set;
    }
}
