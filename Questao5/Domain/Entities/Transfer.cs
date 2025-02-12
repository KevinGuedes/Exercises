namespace Questao5.Domain.Entities;

public sealed class Transfer(Guid accountId, DateOnly date, string type, decimal value)
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AccountId { get; private set; } = accountId;
    public DateOnly Date { get; private set; } = date;
    public string Type { get; private set; } = type;
    public decimal Value { get; private set; } = value;
}
