using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Annotations;
using RIAPP.DataService.Core.Types;
using RIAppDemo.BLL.Models;
using RIAppDemo.DAL.EF;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.DataManagers
{
    public class LookUpProductDM : AdWDataManager<LookUpProduct>
    {
        [Query]
        public async Task<QueryResult<LookUpProduct>> ReadProductLookUp()
        {
            PerformQueryResult<Product> res = PerformQuery<Product>((countQuery) => countQuery.CountAsync());
            int? totalCount = await res.CountAsync();
            List<LookUpProduct> products = new List<LookUpProduct>();
            if (totalCount > 0)
            {
                products = await res.Data.Select(p => new LookUpProduct { ProductId = p.ProductId, Name = p.Name }).ToListAsync();
            }
            return new QueryResult<LookUpProduct>(products, totalCount);
        }
    }
}