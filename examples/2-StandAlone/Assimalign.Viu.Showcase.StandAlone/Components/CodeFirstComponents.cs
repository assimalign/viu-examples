using Assimalign.Viu.Components;

namespace Assimalign.Viu.Showcase.StandAlone.Components;

/// <summary>
/// Components do not have to be .viu files. A code-first component is a registration whose setup
/// returns a render function that builds virtual nodes directly.
/// </summary>
public static class CodeFirstComponents
{
    /// <summary>Renders <c>&lt;p class="footnote"&gt;</c> with the text of its <c>text</c> parameter.</summary>
    public static ComponentRegistration Footnote { get; } = ComponentRegistration.Define(
        "Footnote",
        new ComponentContract(
            displayName: "Footnote",
            parameters: [new ComponentParameter("text", isRequired: true, parameterType: typeof(string))]),
        context => _ => new ElementNode(
            new QualifiedName("p"),
            bindings: [ElementBinding.Attribute(new QualifiedName("class"), "footnote")],
            children: [new TextNode((string?)context.Bindings.Parameters["text"] ?? string.Empty)]));
}
