using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Annotations;
using RIAPP.DataService.Core.Query;
using RIAPP.DataService.Core.Security;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.EFCore.Utils;
using RIAppDemo.DAL.EF;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.DataManagers
{
    public class CustomerDM : AdWDataManager<Customer>
    {
        [Query]
        public async Task<QueryResult<Customer>> ReadCustomer(bool? includeNav)
        {
            IQueryable<Customer> customers = DB.Customer;
            QueryRequest queryInfo = this.GetCurrentQueryInfo();
            // AddressCount does not exists in Database (we calculate it), so it is needed to sort it manually
            SortItem addressCountSortItem = queryInfo.SortInfo.SortItems.FirstOrDefault(sortItem => sortItem.FieldName == "AddressCount");
            if (addressCountSortItem != null)
            {
                queryInfo.SortInfo.SortItems.Remove(addressCountSortItem);
                if (addressCountSortItem.SortOrder == SortOrder.ASC)
                {
                    customers = customers.OrderBy(c => c.CustomerAddress.Count());
                }
                else
                {
                    customers = customers.OrderByDescending(c => c.CustomerAddress.Count());
                }
            }

            int? totalCount = queryInfo.PageIndex == 0 ? 0 : (int?)null;
            // perform query
            PerformQueryResult<Customer> customersResult = this.PerformQuery(customers.AsNoTracking(), queryInfo.PageIndex == 0 ? (countQuery) => countQuery.CountAsync() : (Func<IQueryable<Customer>, Task<int>>)null);
            System.Collections.Generic.List<Customer> customersList = await customersResult.Data.ToListAsync();
            // only execute total counting if we got full page size of rows, preventing unneeded database call to count total
            if (queryInfo.PageIndex == 0 && customersList.Any())
            {
                int cnt = customersList.Count;
                if (cnt < queryInfo.PageSize)
                {
                    totalCount = cnt;
                }
                else
                {
                    totalCount = totalCount = await customersResult.CountAsync();
                }
            }

            QueryResult<Customer> queryRes = new QueryResult<Customer>(customersList, totalCount);

            if (includeNav == true)
            {
                int[] customerIDs = customersList.Select(c => c.CustomerId).ToArray();
                System.Collections.Generic.List<CustomerAddress> customerAddress = await DB.CustomerAddress.AsNoTracking().Where(ca => customerIDs.Contains(ca.CustomerId)).ToListAsync();
                int[] addressIDs = customerAddress.Select(ca => ca.AddressId).ToArray();

                SubResult subResult1 = new SubResult
                {
                    dbSetName = "CustomerAddress",
                    Result = customerAddress
                };

                SubResult subResult2 = new SubResult
                {
                    dbSetName = "Address",
                    Result = await DB.Address.AsNoTracking().Where(adr => addressIDs.Contains(adr.AddressId)).ToListAsync()
                };

                // since we have loaded customer addresses - update server side calculated field: AddressCount 
                // (which i have introduced for testing purposes as a server calculated field)
                ILookup<int, CustomerAddress> addressLookUp = customerAddress.ToLookup(ca => ca.CustomerId);
                customersList.ForEach(customer =>
                {
                    customer.AddressCount = addressLookUp[customer.CustomerId].Count();
                });

                // return two subresults with the query results
                queryRes.SubResults.Add(subResult1);
                queryRes.SubResults.Add(subResult2);
            }

            return queryRes;
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Insert(Customer customer)
        {
            customer.PasswordHash = Guid.NewGuid().ToString();
            customer.PasswordSalt = new string(Guid.NewGuid().ToString().ToCharArray().Take(10).ToArray());
            DB.Customer.Add(customer);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Update(Customer customer)
        {
            customer.ModifiedDate = DateTime.Now;
            Customer orig = this.GetOriginal<Customer>();
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Customer> entry = DB.Customer.Attach(customer);

            // Using custom extension method - This is a workaround to update owned entities https://github.com/aspnet/EntityFrameworkCore/issues/13890
            entry.SetOriginalValues(orig);


            /*
            entry.OriginalValues.SetValues(orig);
            var _entry2 = DB.Entry<CustomerName>(customer.CustomerName);
            var _entry3 = DB.Entry<CustomerContact>(customer.CustomerName.Contact);
            _entry2.OriginalValues.SetValues(orig.CustomerName);
            _entry3.OriginalValues.SetValues(orig.CustomerName.Contact);
            */
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Delete(Customer customer)
        {
            DB.Customer.Attach(customer);
            DB.Customer.Remove(customer);
        }

        [Refresh]
        public async Task<Customer> RefreshCustomer(RefreshRequest refreshInfo)
        {
            IQueryable<Customer> query = this.GetRefreshedEntityQuery(DB.Customer, refreshInfo);
            return await query.SingleAsync();
        }
    }
}