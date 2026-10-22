using Assimalign.Viu.Browser.Router;
using Assimalign.Viu.Components;
using Assimalign.Viu.Router;
using Assimalign.Viu.Examples;

await RoutingBootstrap.RunAsync(BrowserRouterHistory.CreateWeb(), [new RouteRecord("/", component: new ElementNode(new QualifiedName("p"), children: [new TextNode("Home")])), new RouteRecord("/people/:id", component: ComponentNode.For<Person>(), argumentsResolver: RouteComponentArguments.FromParameters())]);
