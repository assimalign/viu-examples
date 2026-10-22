using System.Collections.Generic;

namespace Assimalign.Viu.Showcase.StandAlone.Services;

/// <summary>Supplies the initial task titles. Resolved from the application's service provider.</summary>
public interface ITaskSeedSource
{
    IReadOnlyList<string> GetInitialTitles();
}

/// <summary>A fixed in-memory seed; a real app might read local storage or call an API.</summary>
public sealed class SampleTaskSeedSource : ITaskSeedSource
{
    public IReadOnlyList<string> GetInitialTitles() =>
    [
        "Read the component library",
        "Add a task of your own",
        "Open a task's detail page",
    ];
}
