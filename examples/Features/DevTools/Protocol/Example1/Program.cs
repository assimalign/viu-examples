using System.Collections.Generic;
using System;
using Assimalign.Cohesion.Viu.Hosting;
using Assimalign.Viu.DevTools;
using Assimalign.Viu.Examples;

ViuApplicationBuilder builder = ViuApplication.CreateBuilder();
await ConsoleExample.RunAsync(builder, () =>
{
    string message = ProtocolCodec.SerializeBatch(new List<ProtocolEnvelope>());
    Console.WriteLine(message);
    Console.WriteLine(ProtocolCodec.DeserializeBatch(message) is not null);
});
