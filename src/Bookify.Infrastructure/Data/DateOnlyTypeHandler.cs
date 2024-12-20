using System.Data;
using Dapper;

namespace Bookify.Infrastructure.Data
{
    // Bcos dapper does not support DateOnly out of the box...hence we create a type handler
    // Register handler in DI
    internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
    {
        public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

        public override void SetValue(IDbDataParameter parameter, DateOnly value)
        {
            parameter.DbType = DbType.Date;
            parameter.Value = value;
        }
    }
}
