using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Annotations;
using RIAPP.DataService.Core.Query;
using RIAPP.DataService.Core.Security;
using RIAPP.DataService.Core.Types;
using RIAppDemo.BLL.Utils;
using RIAppDemo.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.DataManagers
{
    public class ProductDM : AdWDataManager<Expando>
    {
        [Query]
        public async Task<QueryResult<Expando>> ReadProduct(int[] param1, string param2)
        {
            // var queryInfo = RequestContext.CurrentQueryInfo;
            PerformQueryResult<Product> productsResult = PerformQuery<Product>((countQuery) => countQuery.CountAsync());
            int? totalCount = await productsResult.CountAsync();
            List<Product> productsList = new List<Product>();
            if (totalCount > 0)
            {
                productsList = await productsResult.Data.ToListAsync();
            }
            int[] productIDs = productsList.Select(p => p.ProductId).Distinct().ToArray();
            IEnumerable<Expando> expandoList = productsList.Select(p => p.ToDictionary(() => new Expando())).ToList();
            QueryResult<Expando> queryResult = new QueryResult<Expando>(expandoList, totalCount);


            SubResult subResult = new SubResult
            {
                dbSetName = "SalesOrderDetail",
                Result = await DB.SalesOrderDetail.AsNoTracking().Where(sod => productIDs.Contains(sod.ProductId)).ToListAsync()
            };

            // include related SalesOrderDetails with the products in the same query result
            queryResult.subResults.Add(subResult);

            // example of returning out of band information and use it on the client (of it can be more useful than it)
            queryResult.extraInfo = new { test = "ReadProduct Extra Info: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") };
            return queryResult;
        }

        [Query]
        public async Task<QueryResult<Expando>> ReadProductByIds(int[] productIDs)
        {
            List<Product> productsList = await DB.Product.Where(ca => productIDs.Contains(ca.ProductId)).ToListAsync();
            IEnumerable<Expando> expandoList = productsList.Select(p => p.ToDictionary(() => new Expando())).ToList();

            return new QueryResult<Expando>(expandoList, totalCount: null);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Insert(Expando expando)
        {
            Product product = new Product();

            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Product> entry = DB.Product.Attach(product);
            entry.CurrentValues.SetValues(expando);
            product.ModifiedDate = DateTime.Now;
            entry.State = EntityState.Added;
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Authorize(Policy = "RequireUpdateRights")]
        public void Update(Expando expando)
        {
            expando["ModifiedDate"] = DateTime.Now;
            Product product = new Product();
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Product> entry = DB.Product.Attach(product);
            var orig = GetOriginal();
            entry.OriginalValues.SetValues(orig);
            entry.CurrentValues.SetValues(expando);
            entry.State = EntityState.Modified;
            /*
                var dbValues = entry.GetDatabaseValues();
                entry.OriginalValues.SetValues(dbValues);
            */
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Delete(Expando expando)
        {
            Product product = new Product();
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Product> entry = DB.Product.Attach(product);
            var orig = GetOriginal();
            entry.OriginalValues.SetValues(orig);
            entry.CurrentValues.SetValues(expando);
            entry.State = EntityState.Deleted;
        }

        [Refresh]
        public async Task<Expando> RefreshProduct(RefreshRequest refreshInfo)
        {
            IQueryable<Product> query = DataService.GetRefreshedEntityQuery(DB.Product, refreshInfo);
            Product product = await query.SingleAsync();
            return product.ToDictionary(() => new Expando());
        }
    }
}