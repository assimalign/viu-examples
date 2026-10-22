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
| Viu SDK and packages | `global.json` → `Assimalign.Viu.Sdk[.Browser]` | `10.0.0-preview.0.ci.7` |
| Cohesion SDK | `global.json` → `Assimalign.Cohesion.Sdk[.Web]` | `10.0.0-preview.0.ci.70` |
| Cohesion.Viu hosting | `Directory.Build.props` → `CohesionViuVersion` | `10.0.0-preview.0.ci.12` |

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

- **Components**: generated catalogs include the app and referenced Viu libraries by default, so
  templates use `<Card>`, `<Pill>`, … directly. Typed definitions select roots with
  `ComponentNode.For<App>()`; no catalog registration boilerplate is needed.
- **Styles**: component `<style>` blocks are extracted into one library stylesheet. The Browser SDK
  flows both package and project-reference styles into `_content/PackageId/PackageId.viu.css` and
  injects their links before the application's bundle. No custom targets or SPA host-page links
  are needed. A custom server-rendered document shell still links the styles it uses.
- **Theming**: components read CSS custom properties (`--viu-accent`, `--viu-border`, …) with
  fallbacks; each app sets its own palette in `App.viu`.

## 2 — StandAlone SPA

A browser-only app (`Assimalign.Viu.Sdk.Browser`). `dotnet run` builds it and serves `wwwroot` with the
SDK's default WebAssembly dev host — no server project.

- **Composition** ([`Program.cs`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Program.cs)):
  `BrowserApplicationBuilder`, a typed root, default generated catalogs with extra router and
  code-first registrations, and `UseRouter`.
- **Services without a container**: [`AppServices`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Services/AppServices.cs)
  is a plain `IServiceProvider`; components resolve services through `Context.Services`.
- **Routing**: hash history, route meta (page titles), a parameterized `/tasks/:id` route passed to the
  page through `RouteComponentArguments.FromParameters()`, and a catch-all not-found route. The
  table uses `routes.Add("/", "HomePage", "Home")` and `routes.Add<TaskDetailPage>(...)`.
- **State**: [`TaskStore`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/State/TaskStore.cs) is a
  `StateStoreDefinition<T>` shared by every page, with a `ReactiveList<T>`, computed counts, and
  `Reactive.Batch()`. [`TaskItem`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Models/TaskItem.cs)
  is a source-generated `[Reactive]` object.
- **Templates**: keyed `v-for`, `v-if`/`v-else`, native `v-model.trim`, component `v-model`, the
  explicit `:model-value` + `@update:model-value` form, `<component :is>`, and a code-first component
  ([`CodeFirstComponents`](examples/2-StandAlone/Assimalign.Viu.Showcase.StandAlone/Components/CodeFirstComponents.cs)).
  Model targets are typed properties or references; forms in Card slots use `@submit.prevent` and
  `@keydown.enter.prevent`. The key handler prevents native Enter submission so a task is added once.
  Typed event listeners, static template helpers, compound interpolations, and boolean ARIA bindings
  work directly.

## 3 — Cohesion client + server

Three projects:

| Project | SDK | Role |
| --- | --- | --- |
| `…Cohesion.Shared` | `Assimalign.Viu.Sdk` | The `ITaskCatalog` contract, JSON wire types, and the server-rendered `ServerReport.viu` page |
| `…Cohesion.Client` | `Assimalign.Viu.Sdk.Browser` | The WebAssembly app, composed with the Cohesion hosting model |
| `…Cohesion.Server` | `Assimalign.Cohesion.Sdk.Web` | Serves the client, a JSON API, and server-rendered pages |

Run the **Server**; its `<ProjectReference ... ViuClient="true" />` restores and builds the client.
`builder.Services.AddViuClient()` infers that client and serves its output. Publishing the server
also publishes the trimmed Release client and bundles its complete `wwwroot`.

### Dependency injection — client

[`Client/Program.cs`](examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Client/Program.cs) composes
through `ViuApplication.CreateBuilder()` (Cohesion DI, configuration, and host lifetime):

- `builder.Services.AddSingleton(...)` registers an `HttpClient`, `ITaskCatalog` → `HttpTaskCatalog`,
  a `GreetingService`, and the `IStateStoreRegistry` — instances and factories only (WebAssembly is
  trimmed and AOT-compiled, so nothing is activated by reflection).
- `builder.Services.AddComponents()` includes the generated client and referenced-library catalogs;
  `builder.UseRootComponent<App>()` selects the typed root.
- `builder.Services.AddRouting(...)` registers the router, browser path history, `RouterView`, and
  `RouterLink`; `routing.Routes.Add(...)` defines routes and `application.UseRouter()` installs them.
- `builder.AddClientConfiguration(http)` loads the server's `/_cohesion/client.json` before startup.
- Components resolve all of it through `Context.Services` (see `HomePage.viu` and `TasksPage.viu`).

### Dependency injection — server

[`Server/Program.cs`](examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Server/Program.cs) composes
everything through `WebApplication.CreateBuilder(args).Services`: the singleton `ITaskCatalog`,
routing, source-generated JSON serialization, `AddViuServerRendering(...)`, and `AddViuClient()`.
Server-rendered components resolve the Web container's services through `Context.Services`.
Each render owns a DI scope; the Web host owns startup, shutdown, and disposal.

### Server rendering

- `ServerReport.viu` is an ordinary component. `OnServerPrefetch` loads data from the injected
  `ITaskCatalog`; the server awaits it before serializing, so the HTML already contains the data.
- `AddViuServerRendering` selects `ComponentNode.For<ServerReport>()` and the document shell.
  Generated component and server-render catalogs include Shared and the library automatically,
  including when the Web executable only uses the Cohesion SDK. The pipeline calls
  `UseViuServerRenderer(context => context.Request.Path.Value == "/report")`.
- [`ReportDocumentShell`](examples/3-Cohesion/Assimalign.Viu.Showcase.Cohesion.Server/Rendering/ReportDocumentShell.cs)
  writes the HTML around the rendered component. The page ships no JavaScript.
- A task added in the browser (`POST /api/tasks`) appears in the server-rendered report, because both
  the API and server-rendered components resolve the same singleton catalog.

### Pipeline order

```text
UseRouting → MapGet/MapPost /api/tasks → Map("/api") → UseViuServerRenderer(/report) → UseViuApplication
```

Cohesion runs a matched endpoint at the end of the pipeline. `UseViuApplication` delegates requests
with a selected endpoint, so mapped APIs run without a separate branch. The non-rejoining
`Map("/api")` branch additionally reserves the entire API namespace: unknown API paths end as 404
instead of receiving the client fallback. Other unmatched extensionless GET/HEAD navigation paths
still receive `index.html`.

## End-to-end validation

These examples exercise the pinned Viu and viu-platforms builds end to end. Start fresh
instances of both apps (StandAlone on 5180, the Cohesion server on 5181; the server keeps tasks in
memory), then:

```powershell
npm install --prefix _out/validation playwright
node scripts/validate-browser.mjs local
./scripts/Verify-LibraryStyles.ps1 -WebRoot ./_out/standalone/wwwroot
```

The browser script drives both apps in Chrome (set `CHROME_PATH` elsewhere), checks counters,
component and native `v-model`, forms with `@submit.prevent` and Enter, routing, library styles,
the JSON API, and the server-rendered `/report`, and fails on any console warning or error other
than the missing favicon. `Verify-LibraryStyles.ps1` checks that a published library stylesheet and
its compressed variants contain CSS.

The former Viu ci.4 workarounds have all been removed; no known issues remain on the pinned builds.

## Development notes

- Each runnable project has a `Properties/launchSettings.json` profile; `.claude/launch.json` holds the
  same two entries for Claude Code's preview.
- `examples/**/wwwroot/_content/` is generated by the build (library styles, framework scripts) and
  ignored by git.
