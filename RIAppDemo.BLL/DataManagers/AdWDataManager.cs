using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Core;
using RIAPP.DataService.Core.Query;
using RIAPP.DataService.Core.Types;
using RIAppDemo.BLL.DataServices;
using RIAppDemo.DAL.EF;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.DataManagers
{
    public class AdWDataManager<TModel> : BaseDataManager<RIAppDemoServiceEF, TModel>
        where TModel : class
    {
        protected const string USERS_ROLE = RIAppDemoServiceEF.USERS_ROLE;
        protected const string ADMINS_ROLE = RIAppDemoServiceEF.ADMINS_ROLE;

        protected AdventureWorksLT2012Context DB => DataService.DB;

        protected PerformQueryResult<TEntity> PerformQuery<TEntity>(Func<IQueryable<TEntity>, Task<int>> totalCountFunc)
            where TEntity : class
        {
            DbSet<TEntity> dbset = DB.Set<TEntity>();
            return this.PerformQuery(dbset.AsNoTracking(), totalCountFunc);
        }
    }
}