using Assimalign.Viu.Reactivity;

namespace Assimalign.Viu.Examples.Showcase.Models;

[Reactive]
public partial class ReactiveProfile
{
    public partial string Name { get; set; }

    public partial int Score { get; set; }
}
