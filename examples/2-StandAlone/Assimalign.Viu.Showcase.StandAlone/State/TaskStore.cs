using System;
using System.Linq;

using Assimalign.Viu.Reactivity;
using Assimalign.Viu.Showcase.StandAlone.Models;
using Assimalign.Viu.Showcase.StandAlone.Services;
using Assimalign.Viu.State;

namespace Assimalign.Viu.Showcase.StandAlone.State;

/// <summary>
/// Application-wide task state. Every component that calls <c>TaskStore.Definition.Use(Context!)</c>
/// receives this same instance, so the home page summary and the tasks page stay in sync.
/// </summary>
public sealed class TaskStore
{
    /// <summary>The store definition: one key, one activator, one instance per registry.</summary>
    public static StateStoreDefinition<TaskStore> Definition { get; } = new(
        "tasks",
        static context => new TaskStore(
            (ITaskSeedSource?)context.Services?.GetService(typeof(ITaskSeedSource))
            ?? new SampleTaskSeedSource()));

    private int _nextId = 1;

    private TaskStore(ITaskSeedSource seedSource)
    {
        foreach (string title in seedSource.GetInitialTitles())
        {
            Add(title);
        }

        // Computed values re-evaluate only when the list or an item's IsDone changes.
        Remaining = Reactive.Computed(() => Items.Count(item => !item.IsDone));
        Completed = Reactive.Computed(() => Items.Count - Remaining.Value);
    }

    public ReactiveList<TaskItem> Items { get; } = new();

    public Computed<int> Remaining { get; }

    public Computed<int> Completed { get; }

    public void Add(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return;
        }

        Items.Add(new TaskItem(_nextId++, title.Trim()));
    }

    public void Remove(TaskItem item) => Items.Remove(item);

    public void ClearCompleted()
    {
        // Batch so dependents re-run once instead of once per removal.
        using (Reactive.Batch())
        {
            for (int index = Items.Count - 1; index >= 0; index--)
            {
                if (Items[index].IsDone)
                {
                    Items.RemoveAt(index);
                }
            }
        }
    }

    public TaskItem? Find(int id) => Items.FirstOrDefault(item => item.Id == id);
}
