using Assimalign.Viu.Reactivity;

namespace Assimalign.Viu.Examples.Showcase.State;

[Reactive]
public partial class StateStoreDemonstrationState
{
    public partial int Count { get; set; }

    public partial int Step { get; set; }

    public partial string LastOperation { get; set; }
}
