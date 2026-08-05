# Viu SDK showcase

A complete browser application built through `Assimalign.Viu.Sdk`. The app is a real external
consumer: it uses the SDK and shared-framework packages from a local NuGet feed and has no project
references back into the framework repository.

## What it demonstrates

- A responsive nested-route application shell with hash routing, active links, route metadata,
  route arguments, guards, browser back/forward support, and seven views.
- Compiled `.viu` and tag-based `.vue` templates, interpolation, bindings, conditionals, keyed
  loops, slots, declared component parameters, emitted events, and explicit C# script setup.
- Standalone Viu Utilities with CSS-first theme configuration, responsive and custom variants,
  custom utilities, and generated utility CSS without Tailwind, Node, or PostCSS dependencies.
- References, computed values, effects, watchers, batching, reactive collections, and a
  source-generated `[Reactive]` object.
- Application-scoped state through `StateStoreDefinition<T>` and `StateStoreRegistry`.
- Text, textarea, checkbox, checkbox-list, radio, select, dynamic, `.trim`, `.number`, and `.lazy`
  `v-model` bindings.
- DOM event, key, `.prevent`, `.stop`, `.self`, and `.once` modifiers.
- `v-show`, `<Transition>`, keyed `<TransitionGroup>`, `<Teleport>`, `<KeepAlive>`, `<Suspense>`,
  and an asynchronous component.
- Global, scoped, CSS-module, and reactive `v-bind()` styles compiled into the SDK-managed CSS
  bundle, whose stylesheet link is injected automatically.
- Component lifecycle hooks, an application plugin, command-buffered DOM rendering, and browser
  handle diagnostics.

`Assimalign.Viu.Router` and `Assimalign.Viu.Router.Browser` are opt-in packages rather than members
of the current `Assimalign.Viu.App` shared framework. The showcase installs both from the same local
NuGet feed as the SDK, uses `RouterHistory.CreateWebHash()`, and renders the layout and views through
nested `RouterView` components. There is no application-local routing implementation.

When Viu's package version changes, update the `Assimalign.Viu.Sdk` entry in `global.json` and both
Router package references in the showcase project together.

## Run locally

Prerequisites are the .NET SDK version selected by `global.json` and the `wasm-tools` workload
(`dotnet workload install wasm-tools`). Clone `viu` and `viu-examples` as sibling directories,
then pack the current SDK.

`Install-Local.ps1` produces the SDK plus the App reference/runtime packs. The following
`PackViuProjects=true` command also places opt-in library packages such as
`Assimalign.Viu.Router` and `Assimalign.Viu.Router.Browser` in the same feed:

```powershell
Set-Location ..\viu
.\scripts\Install-Local.ps1
dotnet pack .\sdks\Assimalign.Viu.Sdk\Tasks\Assimalign.Viu.Sdk.Tasks.csproj `
  --configuration Release `
  -p:PackViuProjects=true `
  -p:PackageOutputPath="$PWD\_out\packages"

Set-Location ..\viu-examples
dotnet build Assimalign.Viu.Examples.slnx
dotnet run --project examples\Assimalign.Viu.Showcase
```

Repacking the same preview version no longer needs a manual cache prune. This repository redirects
its extracts with `globalPackagesFolder` in `nuget.config`, and the framework's
`scripts/Install-Local.ps1` discovers that cache and prunes it alongside the machine-wide one, so a
repack is picked up on the next restore:

```powershell
# from the viu repository
.\scripts\Install-Local.ps1
```

If you pack by some other route and need to clear this repository's cached Viu packages by hand:

```powershell
$localViuPackageCache = Join-Path $PWD '.nuget\packages'
Get-ChildItem -LiteralPath $localViuPackageCache `
  -Directory `
  -Filter 'assimalign.viu.*' `
  -ErrorAction SilentlyContinue |
  Remove-Item -Recurse -Force
dotnet restore Assimalign.Viu.Examples.slnx --force --no-cache
```

Open the URL printed by `dotnet run`. The starting route is `#/`; deep links such as
`#/reactivity`, `#/components`, `#/forms`, `#/motion`, `#/platform`, and `#/utilities` can be
refreshed safely because routing stays behind the document hash.

For the focused hot-reload probe, run the project with `dotnet watch`, open `#/utilities`, and
increment the displayed interaction count. A template-only edit to `UtilitiesView.vue` should
update the mounted view without resetting that count; edits to `Styles/Utilities.css` should update
the generated utility stylesheet without remounting the application.

## Project shape

```text
examples/Assimalign.Viu.Showcase/
  Components/       .viu/.vue application shell, views, shared UI, and demos
  Models/           source-generated reactive and display models
  Routing/          official Viu Router route records, metadata, arguments, and guards
  State/            application-scoped Viu state-store definition
  Runtime/          service provider, plugin, component catalog, async definition
  Styles/           global component CSS and CSS-first Viu Utilities entries
  wwwroot/          host page and JavaScript boot modules
```
