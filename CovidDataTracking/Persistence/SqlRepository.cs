using System.Collections.Generic;
using AppConstants;
using Dapper;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Persistence
{
    public class SqlRepository : ISqlRepository
    {
        public async Task<int> InsertAsync(string query)
        {
            await using var conn = GetSqlConnection();
            return await conn.ExecuteAsync(query);
        }

        public async Task<IList<T>> QueryAsync<T>(string query)
        {
            await using var conn = GetSqlConnection();
            return await conn.QueryAsync<T>(query) as IList<T>;
        }

        public async Task<T> Find<T>(string query)
        {
            await using var conn = GetSqlConnection();
            return await conn.QueryFirstOrDefaultAsync<T>(query);
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(string.Format(DbConstants.DefaultConnection));
        }
    }
}