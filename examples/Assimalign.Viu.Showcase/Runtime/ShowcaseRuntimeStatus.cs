using System;

using Assimalign.Viu.Reactivity;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public sealed class ShowcaseRuntimeStatus
{
    private int DiagnosticSequence { get; set; }

    public Reference<bool> PluginInstalled { get; } =
        Reactive.Reference(false);

    public Reference<string> PluginMessage { get; } =
        Reactive.Reference("Waiting for application initialization");

    public ReactiveList<string> Diagnostics { get; } = new();

    public void RecordPluginInstallation()
    {
        PluginMessage.Value = "Application plugin installed before the first render";
        PluginInstalled.Value = true;
        AddDiagnostic("Plugin installation completed");
    }

    public void RecordWarning(string message)
        => AddDiagnostic($"Warning · {message}");

    public void RecordError(string source, Exception exception)
        => AddDiagnostic($"Error in {source} · {exception.Message}");

    public void RecordNavigationGuard(string from, string to)
        => AddDiagnostic($"Global guard · {from} → {to}");

    public void RecordNavigation(string path, string outcome)
        => AddDiagnostic($"Navigation {outcome} · {path}");

    public void AddDiagnostic(string message)
    {
        DiagnosticSequence++;
        Diagnostics.Insert(
            0,
            $"{DateTime.Now:HH:mm:ss.fff} · {DiagnosticSequence:D3} · {message}");
        if (Diagnostics.Count > 8)
        {
            Diagnostics.RemoveAt(Diagnostics.Count - 1);
        }
    }
}
