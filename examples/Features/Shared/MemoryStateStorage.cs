using System.Collections.Generic;
using Assimalign.Viu.State;

namespace Assimalign.Viu.Examples;

internal sealed class MemoryStateStorage : IStateStorage
{
    private readonly Dictionary<string, string> values = [];
    public bool TryRead(string key, out string value) => values.TryGetValue(key, out value!);
    public void Write(string key, string value) => values[key] = value;
    public void Remove(string key) => values.Remove(key);
}
