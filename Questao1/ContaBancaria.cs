using System.Globalization;

namespace Questao1;

public class ContaBancaria
{
    public int Numero { get; init; }
    public string Titular { get; private set; }
    public double Saldo { get; private set; }

    public ContaBancaria(int numero, string titular)
    {
        Numero = numero;
        Titular = titular;
        Saldo = 0;
    }

    public ContaBancaria(int numero, string titular, double depositoInicial)
        : this(numero, titular)
    {
        Saldo = depositoInicial;
    }

    public void AtualizarTitular(string novoTitular)
        => Titular = novoTitular;

    public void Deposito(double quantia)
        => Saldo += quantia;

    public void Saque(double quantia)
        => Saldo -= quantia + 3.5;

    public override string ToString()
    {
        return $"Conta {Numero}, Titular: {Titular}, Saldo: $ {Saldo.ToString("F2", CultureInfo.InvariantCulture)}";
    }
}