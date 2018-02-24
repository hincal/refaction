using System.Data;

namespace refactor_me.Database
{
    public interface IDbConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
