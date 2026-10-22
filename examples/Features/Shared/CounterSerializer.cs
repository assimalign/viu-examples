using System.Text.Json;
using Assimalign.Viu.State;

namespace Assimalign.Viu.Examples;

internal sealed class CounterSerializer : IStateStoreSerializer<CounterStore>
{
    public void Serialize(Utf8JsonWriter writer, CounterStore store)
    {
        writer.WriteStartObject();
        writer.WriteNumber("count", store.State.Count);
        writer.WriteEndObject();
    }

    public void Restore(CounterStore store, JsonElement state) => store.Patch(value => value.Count = state.GetProperty("count").GetInt32());
}
