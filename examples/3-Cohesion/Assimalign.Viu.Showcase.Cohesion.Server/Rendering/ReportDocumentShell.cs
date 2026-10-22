using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Assimalign.Viu.ServerRenderer;

namespace Assimalign.Viu.Showcase.Cohesion.Server.Rendering;

/// <summary>
/// The HTML document around a server-rendered page. Viu streams the prefix, then the component
/// markup, then the suffix. The report is static HTML, so the shell links only stylesheets (the
/// component library's CSS is served from the client's wwwroot) and loads no WebAssembly.
/// </summary>
public sealed class ReportDocumentShell : IServerRenderDocumentShell
{
    public static ReportDocumentShell Instance { get; } = new();

    private const string Prefix = """
        <!doctype html>
        <html lang="en">
        <head>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
          <title>Task report · Viu on Cohesion</title>
          <link rel="stylesheet" href="/_content/Assimalign.Viu.Showcase.Components/Assimalign.Viu.Showcase.Components.viu.css" />
          <style>
            body { margin: 0; background: #f1f5f9; color: #0f172a; font-family: system-ui, sans-serif; }
            .report { display: grid; gap: 1rem; max-width: 40rem; margin: 0 auto; padding: 1.5rem; }
            .report__header { display: flex; align-items: center; gap: 0.75rem; }
            .report__header h1 { margin: 0; }
            .report__list { display: grid; gap: 0.5rem; margin: 0; padding: 0; list-style: none; }
            .report__list li { display: flex; justify-content: space-between; }
          </style>
        </head>
        <body>
        <div id="report">
        """;

    private const string Suffix = """
        </div>
        </body>
        </html>
        """;

    public ValueTask WritePrefixAsync(IServerRenderOutput output, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);
        return output.WriteAsync(Prefix.AsMemory(), cancellationToken);
    }

    public ValueTask WriteSuffixAsync(
        IServerRenderOutput output,
        IReadOnlyDictionary<string, string> teleports,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(teleports);

        // The report uses no <Teleport>; a shell that supports them writes each payload here.
        return output.WriteAsync(Suffix.AsMemory(), cancellationToken);
    }
}
