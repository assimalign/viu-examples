using System;

using Assimalign.Viu.Components;
using Assimalign.Viu.Reactivity;
using Assimalign.Viu.Router;

namespace Assimalign.Viu.Examples.Showcase.Runtime;

public static class ShowcaseComponentCatalog
{
    public static ComponentFactory CreateFactory()
    {
        ComponentFactory factory = new();
        GeneratedViuComponents.Register(factory);
        factory.Register(RouterLink.Registration);
        factory.Register(RegisterByName("RouterLink", RouterLink.Registration));
        factory.Register(RouterView.Registration);
        factory.Register(RegisterByName("RouterView", RouterView.Registration));
        factory.Register(ShowcaseAsynchronousComponents.Insight.Registration);
        factory.Register(CreateCodeFirstPanelRegistration());
        return factory;
    }

    private static ComponentRegistration CreateCodeFirstPanelRegistration()
        => ComponentRegistration.Define(
            "CodeFirstPanel",
            new ComponentContract(
                displayName: "CodeFirstPanel",
                flags: ComponentFlags.None),
            context =>
            {
                Reference<int> interactions = Reactive.Reference(0);
                ShowcaseRuntimeStatus? runtimeStatus =
                    context.Services?.GetService(typeof(ShowcaseRuntimeStatus))
                        as ShowcaseRuntimeStatus;

                void RecordInteraction(object? _)
                {
                    interactions.Value++;
                    runtimeStatus?.AddDiagnostic(
                        "Code-first component event dispatched");
                }

                return _ => new ElementNode(
                    new QualifiedName("section"),
                    bindings:
                    [
                        Class("code-first-panel"),
                        ElementBinding.Attribute(
                            new QualifiedName("aria-label"),
                            "Code-first component demonstration"),
                    ],
                    children:
                    [
                        new ElementNode(
                            new QualifiedName("div"),
                            bindings: [Class("code-first-panel__copy")],
                            children:
                            [
                                new ElementNode(
                                    new QualifiedName("span"),
                                    bindings: [Class("section-kicker")],
                                    children: [new TextNode("Code-first authoring")]),
                                new ElementNode(
                                    new QualifiedName("h2"),
                                    children:
                                    [
                                        new TextNode(
                                            "Compiled templates and delegate-defined components share one runtime."),
                                    ]),
                                new ElementNode(
                                    new QualifiedName("p"),
                                    children:
                                    [
                                        new TextNode(
                                            "This panel is registered with ComponentRegistration.Define, resolves application services, and updates through the same reactive scheduler."),
                                    ]),
                                new ElementNode(
                                    new QualifiedName("button"),
                                    bindings:
                                    [
                                        Class("code-first-panel__button"),
                                        ElementBinding.Attribute(
                                            new QualifiedName("type"),
                                            "button"),
                                        ElementBinding.Event(
                                            "click",
                                            (Action<object?>)RecordInteraction),
                                    ],
                                    children: [new TextNode("Exercise code-first reactivity")]),
                            ]),
                        new ElementNode(
                            new QualifiedName("div"),
                            bindings:
                            [
                                Class("code-first-panel__metric"),
                                ElementBinding.Attribute(
                                    new QualifiedName("aria-live"),
                                    "polite"),
                            ],
                            children:
                            [
                                new ElementNode(
                                    new QualifiedName("strong"),
                                    children:
                                    [
                                        new TextNode(interactions.Value.ToString()),
                                    ]),
                                new ElementNode(
                                    new QualifiedName("span"),
                                    children: [new TextNode("local interactions")]),
                            ]),
                    ]);
            });

    private static ElementBinding Class(string value)
        => ElementBinding.Attribute(new QualifiedName("class"), value);

    private static ComponentRegistration RegisterByName(
        string name,
        ComponentRegistration registration)
        => new(
            ComponentReference.ForName(name),
            registration.Contract,
            registration.Activator);
}
