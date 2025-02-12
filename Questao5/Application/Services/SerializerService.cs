using Questao5.Domain.Interfaces.Services;
using System.Text.Json;

namespace Questao5.Application.Services;

public sealed class SerializerService : ISerializerService
{
    private readonly JsonSerializerOptions _options = new() { WriteIndented = false };

    public string Serialize(object obj)
        => JsonSerializer.Serialize(obj, _options);

    public T Deserialize<T>(string json)
        => JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException("Invalid json payload");
}
