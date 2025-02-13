namespace Questao5.Domain.Entities;

public sealed class Idempotency
{
    public Guid Key { get; private set; }
    public string Request { get; private set; } = null!;
    public string Result { get; private set; } = null!;

    /// <summary>
    /// Parameterless constructor required for ORM and fakers
    /// </summary>
    private Idempotency()
    {
    }

    public Idempotency(Guid key, string request, string result)
    {
        Key = key;
        Request = request;
        Result = result;
    }
}
