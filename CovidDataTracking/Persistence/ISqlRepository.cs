using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistence
{
    public interface ISqlRepository
    {
        Task<int> InsertAsync(string query);
        Task<IList<T>> QueryAsync<T>(string query);
    }
}
