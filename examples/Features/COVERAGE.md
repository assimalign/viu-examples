# Feature coverage

Each example isolates one feature; related infrastructure stays in Shared. Every focused feature example
uses Cohesion `ViuApplicationBuilder`, with the browser host or the finite in-memory console host. The existing composition
samples demonstrate packaged component libraries and Cohesion client/server hosting.

| Feature family | Examples |
| --- | --- |
| References, computed values, watches, collections, generated reactive objects, batching and scope teardown | Runtime/Reactivity |
| Parameters, typed events, default/named/argument slots, lifecycle, dynamic and asynchronous components, KeepAlive, Suspense, services, component model | Runtime/Components |
| Conditional/list rendering, visibility, bindings and spread, event modifiers, input models, one-time output and trusted HTML | Syntax/Templates and ReactiveList |
| Ordinary, module, scoped, deep/global escape and slotted CSS; native ownership and child boundaries | Syntax/Styles |
| Canonical .viu and compatible .vue authoring | Syntax/VueCompatibility and the remaining catalog |
| Teleport, CSS transitions, keyed group animation, hydration identity, native custom elements | Browser |
| Memory/browser/hash histories, route matching, dynamic parameters, links/outlets and generated Pages tables | Router and Utilities/FileRoutes |
| Setup stores, patch/reset, actions, subscriptions, typed payload handoff and persistence plugins | State |
| String/streaming output, isolated static generation and full SSR hosting | ServerRenderer and ../3-Cohesion |
| Inspection protocol and DOM-free component testing | DevTools |
| Utility candidates and the standalone build package | Utilities |
| Host-neutral reusable libraries, standalone WASM, Cohesion services and SSR | ../1-ComponentLib, ../2-StandAlone, ../3-Cohesion |

Reactive CSS-variable application remains deferred by [STY-6] through [STY-8]; scoped CSS does not
restore that separate feature. Real editor-host testing and benchmark scenarios belong in their
own repositories/harnesses, not in application examples. These examples cover public feature
families; they are not a copy of every API overload or an implementation of still-open board items.
