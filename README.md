# Viu SDK showcase

A complete browser application built through `Assimalign.Viu.Sdk.Browser`, the browser segment
layered on the host-neutral `Assimalign.Viu.Sdk`. The app is a real external consumer: it uses the
SDK and shared-framework packages from a local NuGet feed and has no project references back into
the framework repository.

## What it demonstrates

- A responsive nested-route application shell with hash routing, active links, route metadata,
  route arguments, guards, browser back/forward support, and seven views.
- Compiled `.viu` templates, interpolation, bindings, conditionals, keyed loops, slots, declared
  component parameters, emitted events, and explicit C# script setup.
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
- Global, component-prefixed, and CSS-module styles compiled into the SDK-managed CSS bundle,
  whose stylesheet link is injected automatically. Scoped-style isolation and style `v-bind()`
  are deferred; this showcase uses explicit classes and CSS custom properties instead.
- Component lifecycle hooks, full-lifetime application middleware, command-buffered DOM rendering,
  and browser handle diagnostics.
- Generated `.viu` registrations alongside a code-first `ComponentRegistration.Define` component
  with reactive state; the reactivity view separately demonstrates component-owned
  `ComponentContext.Watch` cleanup.

`Assimalign.Viu.Router` and `Assimalign.Viu.Browser.Router` are opt-in packages rather than members
of the `Assimalign.Viu.App`/`Assimalign.Viu.App.Browser` shared-framework segments. The showcase installs both from the same local
NuGet feed as the SDK, uses `BrowserRouterHistory.CreateWebHash()` (the browser history integration
lives in `Assimalign.Viu.Browser.Router`), and renders the layout and views through
nested `RouterView` components. There is no application-local routing implementation.

When Viu's package version changes, update the `Assimalign.Viu.Sdk` and `Assimalign.Viu.Sdk.Browser`
entries in `global.json` and both Router package references in the showcase project together — the
Browser SDK pins its base SDK at the exact same version.

## Run locally

Prerequisites are the .NET SDK version selected by `global.json` and the `wasm-tools` workload
(`dotnet workload install wasm-tools`). Clone `viu` and `viu-examples` as sibling directories,
then pack the current SDK.

`Install-Local.ps1` produces the complete package inventory: the SDK, App reference/runtime packs,
and opt-in libraries such as `Assimalign.Viu.Router` and `Assimalign.Viu.Browser.Router`:

```powershell
Set-Location ..\viu
pwsh -NoProfile -File .\scripts\Install-Local.ps1

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
increment the displayed interaction count. A template-only edit to `UtilitiesView.viu` should
update the mounted view without resetting that count; edits to `Styles/Utilities.css` should update
the generated utility stylesheet without remounting the application.

## Project shape

```text
examples/Assimalign.Viu.Showcase/
  Components/       .viu application shell, views, shared UI, and demos
  Models/           source-generated reactive and display models
  Routing/          official Viu Router route records, metadata, arguments, and guards
  State/            application-scoped Viu state-store definition
  Runtime/          service provider, middleware, component catalog, async definition
  Styles/           global component CSS and CSS-first Viu Utilities entries
  wwwroot/          host page and JavaScript boot modules
```
