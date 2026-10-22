using System;

namespace Assimalign.Viu.Showcase.Cohesion.Client.Services;
/// <summary>
/// A plain application service registered in the client's DI container with a factory, showing that
/// any service — not only framework types — reaches components through <c>Context.Services</c>.
/// </summary>
public sealed class GreetingService
{
    private readonly string _environment;
    public GreetingService(string environment)
    {
        ArgumentNullException.ThrowIfNull(environment);
        _environment = environment;
    }

    public string Greet(string name) => $"Hello {(string.IsNullOrWhiteSpace(name) ? "there" : name)} — composed by Cohesion DI ({_environment}).";
}
