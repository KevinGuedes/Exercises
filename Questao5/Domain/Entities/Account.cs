namespace Questao5.Domain.Entities;

public sealed class Account
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }

    public Account()
    {
    }

    public Account(Guid id, int number, string name, bool isActive)
    {
        Id = id;
        Number = number;
        Name = name;
        IsActive = isActive;
    }
}
