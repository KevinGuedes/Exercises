namespace Questao5.Domain.Entities;

public sealed class Idempotency(Guid key, string request, string result)
{
    public Guid Key { get; private set; } = key;
    public string Request { get; private set; } = request;
    public string Result { get; private set; } = result;
}
