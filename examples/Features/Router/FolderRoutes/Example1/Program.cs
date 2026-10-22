using Assimalign.Viu.Browser.Router;
using Assimalign.Viu.Components;
using Assimalign.Viu.Router;
using Assimalign.Viu.Examples;

await RoutingBootstrap.RunAsync(BrowserRouterHistory.CreateWeb(), GeneratedViuFileRoutes.Create());
