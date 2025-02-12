namespace Questao5.Domain.Entities;

public sealed class Transfer(Guid accountId, DateOnly date, string type, decimal value)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AccountId { get; set; } = accountId;
    public DateOnly Date { get; set; } = date;
    public string Type { get; set; } = type;
    public decimal Value { get; set; } = value;
}
