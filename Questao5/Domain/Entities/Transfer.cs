namespace Questao5.Domain.Entities;

public sealed class Transfer
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public DateOnly Date { get; private set; }
    public string Type { get; private set; } = null!;
    public decimal Value { get; private set; }

    /// <summary>
    /// Parameterless constructor required for ORM and fakers
    /// </summary>
    private Transfer()
    {
    }

    public Transfer(Guid accountId, DateOnly date, string type, decimal value)
    {
        Id = Guid.NewGuid();
        AccountId = accountId;
        Date = date;
        Type = type;
        Value = value;
    }
}