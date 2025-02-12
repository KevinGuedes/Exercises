namespace Questao5.Domain.Interfaces.Services;

public interface ISerializerService
{
    string Serialize(object obj);
    T Deserialize<T>(string json);
}
