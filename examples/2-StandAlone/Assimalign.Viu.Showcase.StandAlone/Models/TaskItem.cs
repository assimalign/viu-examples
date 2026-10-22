using Assimalign.Viu.Reactivity;

namespace Assimalign.Viu.Showcase.StandAlone.Models;

/// <summary>
/// One task. <see cref="ReactiveAttribute"/> makes the generator emit tracking/triggering bodies for
/// the partial properties, so a render that reads <see cref="IsDone"/> re-renders when it changes —
/// no reflection, no proxy.
/// </summary>
[Reactive]
public partial class TaskItem
{
    public TaskItem(int id, string title)
    {
        Id = id;
        Title = title;
    }

    public int Id { get; }

    public partial string Title { get; set; }

    public partial bool IsDone { get; set; }
}
