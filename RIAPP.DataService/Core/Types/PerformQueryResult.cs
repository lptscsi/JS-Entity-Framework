using System;
using System.Linq;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core.Types
{
    public class PerformQueryResult<TData>(IQueryable<TData> data, Func<Task<int?>> count)
    {
        public readonly IQueryable<TData> Data = data;
        public readonly Func<Task<int?>> CountAsync = count;
    }
}
