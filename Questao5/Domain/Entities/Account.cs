namespace Questao5.Domain.Entities;

public sealed class Account(string id, int number, string name, bool isActive)
{
    public string Id { get; set; } = id;
    public int Number { get; set; } = number;
    public string Name { get; set; } = name;
    public bool IsActive { get; set; } = isActive;
}
