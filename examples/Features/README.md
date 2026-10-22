# Focused Viu examples

One feature per small project, organized by area and `Example1`, following the Cohesion Web example layout.
Each project is independently runnable and composes through Cohesion `ViuApplicationBuilder`.
Browser examples select `ConfigureBrowser()`; finite console demos share a small in-memory host in
`Shared/ConsoleExample.cs`. State examples link small explicit models/serializers from `Shared/`. No project references framework source across repositories.

## Setup

Keep `viu` and `viu-examples` as siblings, configure the GitHub Packages credentials described in
the repository README for the published Cohesion packages, and pack the matching Viu branch first:

```powershell
pwsh ../viu/scripts/Install-Local.ps1
dotnet build examples/Features/Assimalign.Viu.Features.slnx
pwsh scripts/Test-Features.ps1
```

This catalog has its own SDK/package pin and NuGet cache. Existing composition examples retain their
published Viu/Cohesion pins. Repacking the same Viu version requires a fresh isolated cache; for example,
set `NUGET_PACKAGES` to a new directory beneath `.nuget/` before building. MSBuild may hold old SDK files
open, so do not try to delete an active SDK cache. `dotnet run` in an individual example starts its server
or prints its console result. The smoke script uses headless Chromium and kills only its own servers. Cohesion hosting packages use
the central `CohesionViuVersion` pin. This catalog uses the Browser SDK development server and does not
add a Cohesion Web project.

## Application composition

The shared browser entry point keeps the hosting model visible without repeating it in every example:

```csharp
ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
builder.Services.AddComponents();
builder.UseRootComponent<App>();
builder.ConfigureBrowser(options => options.MountTargetSelector = "#app");
await using ViuApplication application = builder.Build();
await application.RunAsync();
```

Examples with special initialization have their own `Program.cs`. Services register through
`builder.Services`; routes use Cohesion `AddRouting` and `UseRouter`. Console examples create the
same builder and call `ConsoleExample.RunAsync(builder, ...)`, which starts the in-memory Viu host,
executes the finite demonstration, and stops/disposes the application and provider.

## Catalog

| Area | Feature | Demonstrates |
| --- | --- | --- |
| Runtime/Reactivity | [Reference](Runtime/Reactivity/Reference/Example1) | A reference schedules rendering when its value changes. |
| Runtime/Reactivity | [Computed](Runtime/Reactivity/Computed/Example1) | A computed value caches its getter until a dependency changes. |
| Runtime/Reactivity | [Watch](Runtime/Reactivity/Watch/Example1) | A component-owned watch receives the current and previous values and stops on unmount. |
| Runtime/Reactivity | [ReactiveList](Runtime/Reactivity/ReactiveList/Example1) | Adding to a reactive collection updates a keyed list. |
| Runtime/Reactivity | [Batch](Runtime/Reactivity/Batch/Example1) | Several writes form one reactive batch. |
| Runtime/Components | [Parameters](Runtime/Components/Parameters/Example1) | A parent supplies one typed child parameter. |
| Runtime/Components | [Events](Runtime/Components/Events/Example1) | A typed child event sends one value to its parent. |
| Runtime/Components | [Slots](Runtime/Components/Slots/Example1) | A child renders caller content through its default slot. |
| Runtime/Components | [NamedSlots](Runtime/Components/NamedSlots/Example1) | Named and default slots fill three independent child regions. |
| Runtime/Components | [SlotArguments](Runtime/Components/SlotArguments/Example1) | A child supplies arguments to caller-owned slot content. |
| Runtime/Components | [Lifecycle](Runtime/Components/Lifecycle/Example1) | The mounted callback updates component-owned state. |
| Runtime/Components | [DynamicComponent](Runtime/Components/DynamicComponent/Example1) | A typed registration selects a component dynamically without reflection activation. |
| Syntax/Templates | [Conditionals](Syntax/Templates/Conditionals/Example1) | A conditional creates and removes its branch. |
| Syntax/Templates | [Show](Syntax/Templates/Show/Example1) | v-show changes display while retaining the mounted element. |
| Syntax/Templates | [ClassBinding](Syntax/Templates/ClassBinding/Example1) | Class bindings follow reactive state. |
| Syntax/Templates | [StyleBinding](Syntax/Templates/StyleBinding/Example1) | A style object binds a reactive CSS property. |
| Syntax/Templates | [EventModifiers](Syntax/Templates/EventModifiers/Example1) | The once modifier removes the listener after its first invocation. |
| Syntax/Templates | [TextModel](Syntax/Templates/TextModel/Example1) | Text input and component state share one writable property. |
| Syntax/Templates | [CheckboxModel](Syntax/Templates/CheckboxModel/Example1) | A checkbox updates a boolean model. |
| Syntax/Templates | [SelectModel](Syntax/Templates/SelectModel/Example1) | Select options update a string model. |
| Syntax/Templates | [NumericModel](Syntax/Templates/NumericModel/Example1) | A numeric input converts its native value to a typed numeric model. |
| Syntax/Templates | [Once](Syntax/Templates/Once/Example1) | v-once retains its first render while an ordinary binding keeps updating. |
| Syntax/Templates | [RawHtml](Syntax/Templates/RawHtml/Example1) | v-html inserts a known HTML string; ordinary interpolation displays the same string as text. |
| Syntax/Styles | [Scoped](Syntax/Styles/Scoped/Example1) | Identical selectors stay isolated across two components. The parent paragraph is blue and the child paragraph is red. |
| Syntax/Styles | [Slotted](Syntax/Styles/Slotted/Example1) | A scoped receiver styles a projected native slot root with :slotted(). |
| Syntax/Styles | [CssModules](Syntax/Styles/CssModules/Example1) | A generated module accessor supplies the collision-free class name. |
| Syntax/Styles | [GlobalStyle](Syntax/Styles/GlobalStyle/Example1) | An ordinary style block emits a global stylesheet rule. |
| Browser | [Teleport](Browser/Teleport/Example1) | Teleport mounts content in a separate target element. |
| Browser | [Transition](Browser/Transition/Example1) | Transition applies enter and leave classes around one conditional element. |
| Browser | [TransitionGroup](Browser/TransitionGroup/Example1) | TransitionGroup animates keyed children as a group. |
| Runtime/Components | [KeepAlive](Runtime/Components/KeepAlive/Example1) | KeepAlive retains one counter per key while the active key changes. |
| Runtime/Components | [ComponentModel](Runtime/Components/ComponentModel/Example1) | A component model uses a typed parameter and an update event. |
| Syntax | [VueCompatibility](Syntax/VueCompatibility/Example1) | The .vue compatibility container accepts explicitly C# script setup and scoped styles. |
| Runtime/Reactivity | [EffectScope](Runtime/Reactivity/EffectScope/Example1) | Stopping an effect scope stops all effects created in it. |
| Runtime/Reactivity | [WatchEffect](Runtime/Reactivity/WatchEffect/Example1) | A watch effect discovers its dependencies and runs cleanup before a replacement run. |
| Runtime/Reactivity | [ReactiveDictionary](Runtime/Reactivity/ReactiveDictionary/Example1) | Dictionary key reads and replacements participate in dependency tracking. |
| Runtime/Reactivity | [ReactiveSet](Runtime/Reactivity/ReactiveSet/Example1) | Set membership and collection size participate in dependency tracking. |
| State | [SetupStore](State/SetupStore/Example1) | A store definition resolves one instance per explicitly owned registry. |
| Router | [MemoryHistory](Router/MemoryHistory/Example1) | Host-free history and navigation update the current route. |
| Router | [RouteMatching](Router/RouteMatching/Example1) | Static routes outrank dynamic routes independently of declaration order. |
| Utilities | [FileRoutes](Utilities/FileRoutes/Example1) | A declarative file-route descriptor becomes an ordinary eager route record. |
| ServerRenderer | [RenderString](ServerRenderer/RenderString/Example1) | Server rendering escapes text and writes complete host-neutral markup. |
| ServerRenderer | [Streaming](ServerRenderer/Streaming/Example1) | A renderer writes to a caller-owned TextWriter. |
| DevTools | [Protocol](DevTools/Protocol/Example1) | DevTools transport batches use source-generated protocol serialization. |
| DevTools | [Testing](DevTools/Testing/Example1) | The in-memory renderer runs without a browser and records host operations. |
| Utilities | [UtilityCandidates](Utilities/UtilityCandidates/Example1) | The standalone utility parser accepts variant and arbitrary-value candidates. |
| Runtime/Components | [AsynchronousComponent](Runtime/Components/AsynchronousComponent/Example1) | An explicit asynchronous definition loads one registered component identity. |
| Runtime/Components | [Suspense](Runtime/Components/Suspense/Example1) | A Suspense boundary owns the asynchronous child pending presentation. |
| Runtime/Components | [Services](Runtime/Components/Services/Example1) | Application composition supplies an IServiceProvider, and the component reads a typed service. |
| Runtime/Reactivity | [ReactiveObject](Runtime/Reactivity/ReactiveObject/Example1) | The SDK source-generates tracking for partial properties on an explicitly marked class. |
| Router | [WebHistory](Router/WebHistory/Example1) | Navigate between two small pages with RouterLink and RouterView; history ownership and startup are explicit. |
| Router | [HashHistory](Router/HashHistory/Example1) | Navigate between two small pages with RouterLink and RouterView; history ownership and startup are explicit. |
| Router | [FolderRoutes](Router/FolderRoutes/Example1) | Navigate between two small pages with RouterLink and RouterView; history ownership and startup are explicit. |
| Browser | [Hydration](Browser/Hydration/Example1) | Attach to server-shaped HTML, retain the existing button, and add reactive event behavior. |
| Browser | [CustomElement](Browser/CustomElement/Example1) | Define a native custom element with a Viu counter inside its open shadow root. |
| Syntax/Templates | [BindingSpread](Syntax/Templates/BindingSpread/Example1) | Spread an object-valued dictionary into element attributes and reactively update it. |
| Syntax/Styles | [ScopedEscapes](Syntax/Styles/ScopedEscapes/Example1) | Use :deep and :global selectively while ordinary scoped selectors retain native author ownership. |
| Runtime/Reactivity | [WritableComputed](Runtime/Reactivity/WritableComputed/Example1) | Bind an input to a computed value with an explicit setter. |
| State | [PatchAndReset](State/PatchAndReset/Example1) | Apply an in-place mutator patch and restore the initial state through an explicit typed copier. |
| State | [Actions](State/Actions/Example1) | Observe a named action without changing the store shape. |
| State | [Subscriptions](State/Subscriptions/Example1) | Subscribe to grouped state mutations for the store lifetime. |
| State | [Serialization](State/Serialization/Example1) | Capture and restore a versioned registry payload with a typed serializer; no reflection discovers state. |
| State | [Persistence](State/Persistence/Example1) | Persist selected state through a host-owned storage boundary and restore it in a new registry. |
| ServerRenderer | [StaticPrerender](ServerRenderer/StaticPrerender/Example1) | Render two complete static documents with an explicit fresh request scope and host-page shell. |
| Utilities | [UtilityBuild](Utilities/UtilityBuild/Example1) | Generate utility CSS from template classes through the standalone build package. |

The manifest contains 65 runnable projects. [Coverage](COVERAGE.md) maps the public feature families to these examples and the existing composition samples.
