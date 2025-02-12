using Dapper;
using System.Data;

namespace Questao5.Infrastructure.Sqlite;

public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString().ToUpper();
        parameter.DbType = DbType.String;
    }

    public override Guid Parse(object value)
        => Guid.Parse((string)value);
}