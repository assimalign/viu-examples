using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Playwright;

if (args.Contains("--install"))
    return Microsoft.Playwright.Program.Main(["install", "chromium"]);
string root = Path.GetFullPath(args[0]);
string? selected = args.Length > 1 ? args[1] : null;
JsonArray examples = JsonNode.Parse(await File.ReadAllTextAsync(Path.Combine(root, "examples.json")))!.AsArray();
using IPlaywright playwright = await Playwright.CreateAsync();
await using IBrowser browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });
using HttpClient client = new()
{
    Timeout = TimeSpan.FromSeconds(2)
};
int passed = 0;
foreach (JsonNode? example in examples)
{
    if (example!["Kind"]!.GetValue<string>() != "Browser")
        continue;
    string feature = example["Feature"]!.GetValue<string>();
    if (selected is not null && feature != selected)
        continue;
    Console.WriteLine("CHECK " + feature);
    string directory = Path.Combine(root, example["Directory"]!.GetValue<string>());
    string project = Directory.GetFiles(directory, "*.csproj").Single();
    ProcessStartInfo start = new("dotnet")
    {
        WorkingDirectory = root,
        UseShellExecute = false,
        CreateNoWindow = true,
        RedirectStandardOutput = true,
        RedirectStandardError = true
    };
    foreach (string argument in new[]
    {
        "run",
        "--project",
        project,
        "--no-build",
        "--no-launch-profile"
    }

    )
        start.ArgumentList.Add(argument);
    const string url = "http://127.0.0.1:5218";
    start.Environment["ASPNETCORE_URLS"] = url;
    ConcurrentQueue<string> log = new();
    using Process server = Process.Start(start)!;
    server.OutputDataReceived += (_, value) =>
    {
        if (value.Data is not null)
            log.Enqueue(value.Data);
    };
    server.ErrorDataReceived += (_, value) =>
    {
        if (value.Data is not null)
            log.Enqueue(value.Data);
    };
    server.BeginOutputReadLine();
    server.BeginErrorReadLine();
    try
    {
        bool ready = false;
        for (int attempt = 0; attempt < 150 && !server.HasExited; attempt++)
        {
            try
            {
                using var response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    ready = true;
                    break;
                }
            }
            catch (HttpRequestException)
            {
            }
            catch (TaskCanceledException)
            {
            }

            await Task.Delay(200);
        }

        if (!ready)
            throw new InvalidOperationException("The sample server did not start.\n" + string.Join('\n', log.TakeLast(20)));
        await using IBrowserContext context = await browser.NewContextAsync();
        IPage page = await context.NewPageAsync();
        ConcurrentQueue<string> errors = new();
        page.PageError += (_, error) => errors.Enqueue(error);
        page.Console += (_, message) =>
        {
            if (message.Type == "error" || message.Type == "warning")
                Console.Error.WriteLine(feature + ": " + message.Text);
        };
        page.RequestFailed += (_, request) => Console.Error.WriteLine(feature + ": " + request.Url + " " + request.Failure);
        await page.GotoAsync(url);
        try
        {
            await page.WaitForFunctionAsync("() => { const app=document.querySelector('#app'); return app && app.textContent.trim() && app.textContent.trim() !== 'Loading…'; }");
        }
        catch
        {
            Console.Error.WriteLine(await page.ContentAsync());
            foreach (string error in errors)
                Console.Error.WriteLine(error);
            throw;
        }

        ILocator button = page.Locator("#app button").First;
        switch (feature)
        {
            case "WebHistory":
            case "HashHistory":
            case "FolderRoutes":
                await page.Locator("a").Last.ClickAsync();
                await Expect(page.Locator("p"), "Person 42");
                if (!page.Url.Contains("people/42", StringComparison.Ordinal))
                {
                    throw new InvalidOperationException("History did not update the URL.");
                }

                await page.GoBackAsync();
                await Expect(page.Locator("p"), "Home");
                break;
            case "Hydration":
                await Assertions.Expect(page.Locator("#app button")).ToHaveAttributeAsync("data-ready", "yes");
                if (!await page.EvaluateAsync<bool>("() => window.preRenderedButton === document.querySelector('#app button')"))
                {
                    throw new InvalidOperationException("Hydration replaced the server button.");
                }

                await button.ClickAsync();
                await Expect(button, "1");
                break;
            case "CustomElement":
                await button.ClickAsync();
                await Expect(button, "1");
                break;
            case "BindingSpread":
                await button.ClickAsync();
                await Assertions.Expect(page.Locator("p")).ToHaveAttributeAsync("title", "Count: 1");
                break;
            case "ScopedEscapes":
                await ExpectColor(page.Locator("p").First, "rgb(0, 128, 0)");
                await ExpectColor(page.Locator("p").Last, "rgb(128, 0, 128)");
                break;
            case "WritableComputed":
                await page.Locator("input").FillAsync("Grace");
                await Expect(page.Locator("output"), "Grace");
                break;
            case "UtilityBuild":
                await Assertions.Expect(page.Locator("p")).ToHaveCSSAsync("font-weight", "700");
                await Assertions.Expect(page.Locator("p")).ToHaveCSSAsync("text-decoration-line", "underline");
                break;
            case "Reference":
                await button.ClickAsync();
                await Expect(page.Locator("button"), "1");
                break;
            case "Computed":
                await button.ClickAsync();
                await Expect(button, "1 × 2 = 2");
                break;
            case "Watch":
                await button.ClickAsync();
                await Expect(page.Locator("output"), "0 → 1");
                break;
            case "Batch":
                await button.ClickAsync();
                await Expect(page.Locator("output"), "3");
                break;
            case "ReactiveList":
            case "TransitionGroup":
                await button.ClickAsync();
                await page.WaitForFunctionAsync("() => document.querySelectorAll('#app li').length === 2");
                break;
            case "TextModel":
                await page.Locator("input").FillAsync("edited");
                await Expect(page.Locator("output"), "edited");
                break;
            case "CheckboxModel":
                await page.Locator("input").CheckAsync();
                await Expect(page.Locator("output"), "true");
                break;
            case "SelectModel":
                await page.Locator("select").SelectOptionAsync("Bonjour");
                await Expect(page.Locator("output"), "Bonjour");
                break;
            case "NumericModel":
                await page.Locator("input").FillAsync("7");
                await Expect(page.Locator("output"), "7");
                break;
            case "Events":
                await button.ClickAsync();
                await Expect(page.Locator("output"), "blue");
                break;
            case "ComponentModel":
                await button.ClickAsync();
                await Expect(page.Locator("output"), "Bonjour");
                break;
            case "Conditionals":
                await button.ClickAsync();
                await Expect(page.Locator("p"), "Hidden branch");
                break;
            case "Show":
                await button.ClickAsync();
                await page.Locator("p").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                break;
            case "EventModifiers":
                await button.ClickAsync();
                await button.ClickAsync();
                await Expect(page.Locator("output"), "1");
                break;
            case "Once":
                await button.ClickAsync();
                await Expect(page.Locator("p").First, "Frozen: 0");
                await Expect(page.Locator("p").Last, "Live: 1");
                break;
            case "Scoped":
                await ExpectColor(page.Locator("p").First, "rgb(0, 0, 255)");
                await ExpectColor(page.Locator("p").Last, "rgb(255, 0, 0)");
                break;
            case "Slotted":
                await ExpectColor(page.Locator("p").First, "rgb(128, 0, 128)");
                break;
            case "CssModules":
                await ExpectColor(page.Locator("p"), "rgb(0, 128, 128)");
                break;
            case "GlobalStyle":
                await ExpectColor(page.Locator("p"), "rgb(255, 140, 0)");
                break;
            case "ClassBinding":
                await button.ClickAsync();
                await ExpectColor(page.Locator("p"), "rgb(128, 128, 128)");
                break;
            case "StyleBinding":
                await button.ClickAsync();
                await ExpectColor(page.Locator("p"), "rgb(128, 128, 128)");
                break;
            case "ReactiveObject":
                await button.ClickAsync();
                await Expect(button, "1");
                break;
            case "KeepAlive":
                ILocator counter = page.Locator("button").Last;
                await counter.ClickAsync();
                await Expect(counter, "Counter: 1");
                await button.ClickAsync();
                await Expect(counter, "Counter: 0");
                await button.ClickAsync();
                await Expect(counter, "Counter: 1");
                break;
            case "AsynchronousComponent":
            case "Suspense":
                await Expect(page.Locator("#app"), "Loaded greeting");
                break;
            case "Teleport":
                await Expect(page.Locator("#overlay"), "Teleported content");
                break;
            case "Transition":
                await button.ClickAsync();
                await page.Locator("p").WaitForAsync(new() { State = WaitForSelectorState.Hidden });
                break;
        }

        if (!errors.IsEmpty)
            throw new InvalidOperationException(string.Join('\n', errors));
        Console.WriteLine("PASS " + feature);
        passed++;
    }
    finally
    {
        if (!server.HasExited)
            server.Kill(entireProcessTree: true);
        await server.WaitForExitAsync();
    }
}

if (passed == 0)
    throw new InvalidOperationException("No browser examples selected.");
Console.WriteLine($"Passed {passed} browser feature examples.");
return 0;
static async Task Expect(ILocator locator, string text) => await Assertions.Expect(locator).ToContainTextAsync(text);
static async Task ExpectColor(ILocator locator, string color) => await Assertions.Expect(locator).ToHaveCSSAsync("color", color);
