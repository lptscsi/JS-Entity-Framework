using Microsoft.EntityFrameworkCore;
using RIAPP.DataService.Annotations;
using RIAPP.DataService.Core.Security;
using RIAPP.DataService.Core.Types;
using RIAppDemo.DAL.EF;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.DataManagers
{
    public class CustomerAddressDM : AdWDataManager<CustomerAddress>
    {
        /// <summary>
        /// Refresh Customer's custom ServerCalculated field 'AddressCount' on insert or delete
        /// </summary>
        /// <param name="changeSet"></param>
        /// <param name="refreshResult"></param>
        /// <returns></returns>
        public override async Task AfterChangeSetCommited(ChangeSetRequest changeSet, SubResultList refreshResult)
        {
            DbSetInfo custAddrDbSet = this.GetSetInfoByName("CustomerAddress");
            DbSetInfo customerDbSet = this.GetSetInfoByName("Customer");

            DbSet dbCustAddr = changeSet.dbSets.FirstOrDefault(d => d.dbSetName == custAddrDbSet.dbSetName);
            if (dbCustAddr != null)
            {
                int[] custIDs = dbCustAddr.rows.Where(r => r.changeType == ChangeType.Deleted || r.changeType == ChangeType.Added).Select(r => r.values.First(v => v.fieldName == "CustomerId").val).Select(id => int.Parse(id)).ToArray();

                System.Collections.Generic.List<Customer> customersList = await DB.Customer.AsNoTracking().Where(c => custIDs.Contains(c.CustomerId)).ToListAsync();
                System.Collections.Generic.List<int> customerAddress = await DB.CustomerAddress.AsNoTracking().Where(ca => custIDs.Contains(ca.CustomerId)).Select(ca => ca.CustomerId).ToListAsync();

                customersList.ForEach(customer =>
                {
                    customer.AddressCount = customerAddress.Count(id => id == customer.CustomerId);
                });

                SubResult subResult = new SubResult
                {
                    dbSetName = customerDbSet.dbSetName,
                    Result = customersList
                };
                refreshResult.Add(subResult);
            }
        }

        [Query]
        public async Task<QueryResult<CustomerAddress>> ReadCustomerAddress()
        {
            PerformQueryResult<CustomerAddress> res = PerformQuery<CustomerAddress>(null);
            return new QueryResult<CustomerAddress>(await res.Data.ToListAsync(), totalCount: null);
        }

        [Query]
        public async Task<QueryResult<CustomerAddress>> ReadAddressForCustomers(int[] custIDs)
        {
            int? totalCount = null;
            System.Collections.Generic.List<CustomerAddress> res = await DB.CustomerAddress.Where(ca => custIDs.Contains(ca.CustomerId)).ToListAsync();
            return new QueryResult<CustomerAddress>(res, totalCount);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Insert(CustomerAddress customeraddress)
        {
            DB.CustomerAddress.Add(customeraddress);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Update(CustomerAddress customeraddress)
        {
            customeraddress.ModifiedDate = DateTime.Now;
            CustomerAddress orig = GetOriginal();
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<CustomerAddress> entry = DB.CustomerAddress.Attach(customeraddress);
            entry.OriginalValues.SetValues(orig);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        public void Delete(CustomerAddress customeraddress)
        {
            DB.CustomerAddress.Attach(customeraddress);
            DB.CustomerAddress.Remove(customeraddress);
        }
    }
}