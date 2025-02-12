namespace Questao5.Domain.Entities;

public sealed class Idempotency(string key, string request, string result)
{
    public string Key { get; set; } = key;
    public string Request { get; set; } = request;
    public string Result { get; set; } = result;
}
