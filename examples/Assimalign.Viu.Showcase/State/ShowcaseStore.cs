using System;

using Assimalign.Viu.Reactivity;

namespace Assimalign.Viu.Examples.Showcase.State;

public sealed class ShowcaseStore
{
    public ShowcaseStore()
    {
        InteractionSummary = Reactive.Computed(
            () => TotalInteractions.Value == 1
                ? "1 interaction"
                : $"{TotalInteractions.Value} interactions");
    }

    public Reference<bool> IsDarkTheme { get; } = Reactive.Reference(false);

    public Reference<int> TotalInteractions { get; } = Reactive.Reference(0);

    public ReactiveList<string> RecentActivity { get; } = new();

    public Computed<string> InteractionSummary { get; }

    public void ToggleTheme()
    {
        IsDarkTheme.Value = !IsDarkTheme.Value;
        Track(IsDarkTheme.Value ? "Dark theme enabled" : "Light theme enabled");
    }

    public void Track(string message)
    {
        ArgumentException.ThrowIfNullOrEmpty(message);
        Reactive.StartBatch();
        try
        {
            TotalInteractions.Value++;
            RecentActivity.Insert(
                0,
                $"{DateTime.Now:HH:mm:ss} · {message}");
            if (RecentActivity.Count > 6)
            {
                RecentActivity.RemoveAt(RecentActivity.Count - 1);
            }
        }
        finally
        {
            Reactive.EndBatch();
        }
    }
}
