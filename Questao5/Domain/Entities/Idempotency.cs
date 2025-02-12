namespace Questao5.Domain.Entities;

public sealed class Idempotency(Guid key, string request, string result)
{
    public Guid Key { get; set; } = key;
    public string Request { get; set; } = request;
    public string Result { get; set; } = result;
}
