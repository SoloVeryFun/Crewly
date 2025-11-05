using Crewly.Data;

namespace Crewly.Config;

using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

public static class SerializationConfig
{
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                ti =>
                {
                    if (ti.Type == typeof(UserData))
                    {
                        ti.PolymorphismOptions = new JsonPolymorphismOptions
                        {
                            TypeDiscriminatorPropertyName = "$type",
                            IgnoreUnrecognizedTypeDiscriminators = true,
                            DerivedTypes =
                            {
                                new JsonDerivedType(typeof(ExecutorData), "executor"),
                                new JsonDerivedType(typeof(ClientData), "client")
                            }
                        };
                    }
                }
            }
        }
    };
}
