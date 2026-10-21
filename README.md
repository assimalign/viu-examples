# Viu examples

Three small, runnable examples of [Viu](https://github.com/assimalign/viu), a C#/.NET UI framework
for the browser (WebAssembly). Each one builds on the previous:

| Folder | What it is | Run |
| --- | --- | --- |
| [`examples/1-ComponentLib`](examples/1-ComponentLib) | A reusable, host-neutral component library | (library, used by 2 and 3) |
| [`examples/2-StandAlone`](examples/2-StandAlone) | A single-page app on the SDK's default dev server | `dotnet run --project examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone` |
| [`examples/3-Cohesion`](examples/3-Cohesion) | A client + Cohesion Web server with DI on both sides and server rendering | `dotnet run --project examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Server` |

StandAlone listens on <http://127.0.0.1:5180>, Cohesion on <http://127.0.0.1:5181>.

## Prerequisites

- The .NET SDK selected by [`global.json`](global.json) (10.0.400 or a later 10.0 feature band) and the
  WebAssembly workload: `dotnet workload install wasm-tools`.
- Read access to the `assimalign` GitHub Packages feed. Every Viu and Cohesion package comes from it,
  and GitHub Packages requires a token with `read:packages` even for public packages. With the GitHub CLI:

  ```bash
  gh auth refresh -h github.com -s read:packages
  ```

  ```bash
  export NuGetPackageSourceCredentials_assimalign="Username=$(gh api user -q .login);Password=$(gh auth token)"
  ```

  (PowerShell: `$env:NuGetPackageSourceCredentials_assimalign = "Username=...;Password=$(gh auth token)"`.)
  The credential is read from the environment and never stored in the repository.

```bash
dotnet build Assimalign.Viu.Examples.slnx
```

## Versions

All pins are CI builds from the GitHub Packages feed and must move together:

| Pin | Where | Value |
| --- | --- | --- |
| Viu SDK and packages | `global.json` → `Assimalign.Viu.Sdk[.Browser]` | `10.0.0-preview.0.ci.4` (viu `main` @ `07facbc5`) |
| Cohesion SDK | `global.json` → `Assimalign.Cohesion.Sdk[.Web]` | `10.0.0-preview.0.ci.21` |
| Cohesion.Viu hosting | `Directory.Build.props` → `CohesionViuVersion` | `10.0.0-preview.0.ci.7` (viu-platforms `main` @ `9f0f7dd`) |

`ViuVersion` is read back from `global.json`, so the SDK and every Viu package reference always agree.
Projects reference packages by id only; versions are applied centrally in `Directory.Build.targets`.

## 1 — Component library

`Assimalign.Viu.Showcase.Components` uses the base `Assimalign.Viu.Sdk` (no Browser dependency), so
the same assembly works in a WebAssembly client and on a server.

| Component | Shows |
| --- | --- |
| `Pill.viu` | Required and defaulted `[Parameter]`s, a computed class |
| `Card.viu` | Default and named slots, slot fallback content, reading `Context.Bindings.Slots` |
| `Counter.viu` | Component-local `Reference<T>`, `Computed<T>`, a typed `[Event]` |
| `Toggle.viu` | Component `v-model` (`modelValue` + `update:modelValue`) |
| `ProgressBar.viu` | Numeric parameters and a computed inline style |

`ViuServerRendering=true` makes the compiler also emit server-markup bodies and a
`GeneratedViuServerRenders` catalog, which 3-Cohesion uses.

Consuming the library:

- **Components**: each Viu assembly generates a `GeneratedViuComponents` catalog in its root namespace;
  the app registers it (`LibraryComponents.Register(factory)`), after which templates use the tags
  (`<Card>`, `<Pill>`, …) directly.
- **Styles**: component `<style>` blocks are extracted into one library stylesheet. A *packaged*
  library registers it through its `buildTransitive` props; for a *ProjectReference*, apps import
  [`build/Assimalign.Viu.Showcase.Components.ProjectReference.targets`](examples/1-ComponentLib/Assimalign.Viu.Showcase.Components/build/Assimalign.Viu.Showcase.Components.ProjectReference.targets),
  which feeds the SDK the same file. Either way the host page links
  `_content/Assimalign.Viu.Showcase.Components/Assimalign.Viu.Showcase.Components.viu.css`.
- **Theming**: components read CSS custom properties (`--viu-accent`, `--viu-border`, …) with
  fallbacks; each app sets its own palette in `App.viu`.

## 2 — StandAlone SPA

A browser-only app (`Assimalign.Viu.Sdk.Browser`). `dotnet run` builds it and serves `wwwroot` with the
SDK's default WebAssembly dev host — no server project.

- **Composition** ([`Program.cs`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Program.cs)):
  `BrowserApplicationBuilder`, a component factory (app + library + router components + one
  code-first component), and `UseRouter`.
- **Services without a container**: [`AppServices`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Services/AppServices.cs)
  is a plain `IServiceProvider`; components resolve services through `Context.Services`.
- **Routing**: hash history, route meta (page titles), a parameterized `/tasks/:id` route passed to the
  page through `RouteComponentArguments.FromParameters()`, and a catch-all not-found route.
- **State**: [`TaskStore`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/State/TaskStore.cs) is a
  `StateStoreDefinition<T>` shared by every page, with a `ReactiveList<T>`, computed counts, and
  `Reactive.Batch()`. [`TaskItem`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Models/TaskItem.cs)
  is a source-generated `[Reactive]` object.
- **Templates**: keyed `v-for`, `v-if`/`v-else`, native `v-model.trim`, component `v-model`, the
  explicit `:model-value` + `@update:model-value` form, `<component :is>`, and a code-first component
  ([`CodeFirstComponents`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Components/CodeFirstComponents.cs)).

## 3 — Cohesion client + server

Three projects:

| Project | SDK | Role |
| --- | --- | --- |
| `…Cohesion.Shared` | `Assimalign.Viu.Sdk` | The `ITaskCatalog` contract, JSON wire types, and the server-rendered `ServerReport.viu` page |
| `…Cohesion.Client` | `Assimalign.Viu.Sdk.Browser` | The WebAssembly app, composed with the Cohesion hosting model |
| `…Cohesion.Server` | `Assimalign.Cohesion.Sdk.Web` | Serves the client, a JSON API, and server-rendered pages |

Run the **Server**; its `ViuClientReference` builds the client and `UseClientOutput` serves it.

### Dependency injection — client

[`Client/Program.cs`](examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Client/Program.cs) composes
through `ViuApplication.CreateBuilder()` (Cohesion DI, configuration, and host lifetime):

- `builder.Services.AddSingleton(...)` registers an `HttpClient`, `ITaskCatalog` → `HttpTaskCatalog`,
  a `GreetingService`, and the `IStateStoreRegistry` — instances and factories only (WebAssembly is
  trimmed and AOT-compiled, so nothing is activated by reflection).
- `builder.Services.AddComponents(...)` contributes the client and library component catalogs.
- `builder.Services.AddRouting(...)` registers the router, browser path history, `RouterView`, and
  `RouterLink`; `application.UseRouter()` installs it.
- `builder.AddClientConfiguration(http)` loads the server's `/_cohesion/client.json` before startup.
- Components resolve all of it through `Context.Services` (see `HomePage.viu` and `TasksPage.viu`).

### Dependency injection — server

[`Server/Program.cs`](examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Server/Program.cs) has two
containers, deliberately sharing one `InMemoryTaskCatalog` instance:

- the **server-side Viu application** (`ViuApplication.CreateBuilder().ConfigureServerRendering()`),
  whose services server-rendered components see through `Context.Services`; and
- the **Cohesion web host** (`WebApplication.CreateBuilder`), which also registers routing and
  source-generated JSON serialization for the API.

### Server rendering

- `ServerReport.viu` is an ordinary component. `OnServerPrefetch` loads data from the injected
  `ITaskCatalog`; the server awaits it before serializing, so the HTML already contains the data.
- The server registers the generated server renders explicitly (`GeneratedViuServerRenders` from the
  Shared and library assemblies) and calls
  `UseViuServerRenderer(viuApplication, serverRenders, documentShell, shouldRender)` for `GET /report`.
- [`ReportDocumentShell`](examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Server/Rendering/ReportDocumentShell.cs)
  writes the HTML around the rendered component. The page ships no JavaScript.
- A task added in the browser (`POST /api/tasks`) appears in the server-rendered report, because both
  containers share the catalog.

### Pipeline order

```text
UseRouting → MapGet/MapPost /api/tasks → Map("/api") → UseViuServerRenderer(/report) → UseViuApplication
```

Cohesion runs a matched endpoint at the end of the pipeline, and `UseViuApplication` answers every
unmatched `GET` with `index.html` (the SPA fallback). The non-rejoining `Map("/api")` branch ends `/api`
requests at the endpoint before the fallback can see them.

## Known issues (Viu ci.4)

The examples work around these framework issues; each workaround is commented where it applies.

1. **Native `v-model` writes back untyped.** The generated write-back assigns `object`, so a native or
   component `v-model` target must be a `Reference<object?>` (templates unwrap references). Typed
   alternatives: an explicit `:checked`/`@change` pair, or `:model-value` + `@update:model-value`.
2. **Native `v-model` inside slot content does not update its model.** An `<input v-model>` placed in
   a `<Card>` slot never commits; the same input in the page's own template works. Inputs therefore sit
   outside slots.
3. **Modifier- and key-wrapped handlers do not run.** `@submit.prevent` and `@keydown.enter` are never
   invoked (a form then submits natively and reloads). Use a plain `@keydown` handler that checks
   `IElementEvent.Key`, and a button `@click` instead of form submission.
4. **Server-render codegen crash.** With `ViuServerRendering` on, an element whose only child is a
   compound interpolation (`{{ A }} / {{ B }}`) crashes the generator (`InvalidCastException`,
   `CompoundExpressionNode`). Use one interpolation of a computed string.
5. **Template helpers must be instance members.** Generated code calls them as `component.Helper(...)`,
   so `static` helpers fail with CS0176.
6. **Typed method groups don't bind as event listeners.** `@changed="OnChanged"` needs
   `OnChanged(object? value)`; `OnChanged(int value)` fails overload resolution.

## Development notes

- Each runnable project has a `Properties/launchSettings.json` profile; `.claude/launch.json` holds the
  same two entries for Claude Code's preview.
- `examples/**/wwwroot/_content/` is generated by the build (library styles, framework scripts) and
  ignored by git.
