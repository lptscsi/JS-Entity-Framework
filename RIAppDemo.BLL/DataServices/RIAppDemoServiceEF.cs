using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RIAPP.DataService.Annotations;
using RIAPP.DataService.Core;
using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Query;
using RIAPP.DataService.Core.Security;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.EFCore;
using RIAppDemo.BLL.Models;
using RIAppDemo.BLL.Utils;
using RIAppDemo.DAL.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResourceHelper = RIAppDemo.BLL.Utils.ResourceHelper;

namespace RIAppDemo.BLL.DataServices
{
    [Authorize]
    public class RIAppDemoServiceEF : EFDomainService<AdventureWorksLT2012Context>, IWarmUp
    {
        internal const string USERS_ROLE = "Users";
        internal const string ADMINS_ROLE = "Admins";

        private readonly ILogger<RIAppDemoServiceEF> _logger;

        public RIAppDemoServiceEF(IServiceContainer serviceContainer,
            AdventureWorksLT2012Context db,
            ILogger<RIAppDemoServiceEF> logger)
            : base(serviceContainer, db)
        {
            _logger = logger;
        }

        /// <summary>
        /// Initialize metadata and make the first use of DbContext
        /// </summary>
        /// <returns></returns>
        async Task IWarmUp.WarmUp()
        {
            MetadataResult metadata = ServiceGetMetadata();
            QueryResponse data = await GetQueryData("ProductCategory", "ReadProductCategory");
        }

        string IWarmUp.Name => "RIAppDemoServiceEF";

        /*
        protected override AdventureWorksLT2012Context CreateDataContext()
        {
            var connection = _connectionFactory.GetRIAppDemoConnectionString();
            DbContextOptionsBuilder<AdventureWorksLT2012Context> optionsBuilder = new DbContextOptionsBuilder<AdventureWorksLT2012Context>();
            optionsBuilder.UseSqlServer(connection, (options)=> {
                // to support SQL SERVER 2008
                // options.UseRowNumberForPaging();
            });
            return new AdventureWorksLT2012Context(optionsBuilder.Options);
        }
        */
        protected override DesignTimeMetadata GetDesignTimeMetadata(bool isDraft)
        {
            if (isDraft)
            {
                return base.GetDesignTimeMetadata(true);
            }
            //  first the uncorrected metadata was saved into xml file and then edited 
            return DesignTimeMetadata.FromXML(ResourceHelper.GetResourceString("RIAppDemo.BLL.Metadata.MainDemo.xml"));
        }

        /// <summary>
        ///     here can be tracked changes to the entities
        ///     for example: product entity changes is tracked and can be seen here
        /// </summary>
        /// <param name="dbSetName"></param>
        /// <param name="changeType"></param>
        /// <param name="diffgram"></param>
        protected override void OnTrackChange(string dbSetName, ChangeType changeType, string diffgram)
        {
            string userName = User.Identity.Name;
            //you can set a breakpoint here and to examine diffgram
            _logger.LogInformation($"User: {userName} action: {diffgram}");
        }

        /// <summary>
        ///     Error logging could be implemented here
        /// </summary>
        /// <param name="ex"></param>
        protected override void OnError(Exception ex)
        {
            string msg = "";
            if (ex != null)
            {
                msg = ex.GetFullMessage();
            }

            _logger.LogError(ex, msg);
        }


        #region ProductModel
        [AllowAnonymous]
        [Query]
        public async Task<QueryResult<ProductModel>> ReadProductModel()
        {
            IEnumerable<ProductModel> res = await this.PerformQuery(DB.ProductModel.AsNoTracking()).ToListAsync();
            return new QueryResult<ProductModel>(res, totalCount: null);
        }

        #endregion

        #region ProductCategory

        /// <summary>
        /// An example how to return query result of another type as entity
        /// </summary>
        /// <returns>Query result</returns>
        [AllowAnonymous]
        [Query]
        [DbSetName("ProductCategory")]
        public async Task<QueryResult<object>> ReadProductCategory()
        {
            // we return anonymous type from query instead of real entities
            // the framework does not care about the real type of the returned entities as long as they contain all the fields
            IQueryable<ProductCategory> query = this.PerformQuery(DB.ProductCategory.AsNoTracking());
            var res = await query.Select(p =>
            new
            {
                ProductCategoryId = p.ProductCategoryId,
                ParentProductCategoryId = p.ParentProductCategoryId,
                Name = p.Name,
                Rowguid = p.Rowguid,
                ModifiedDate = p.ModifiedDate
            }).ToListAsync();
            return new QueryResult<object>(res, totalCount: null);
        }

        #endregion

        [Query]
        public async Task<QueryResult<SalesInfo>> ReadSalesInfo()
        {
            QueryRequest queryInfo = this.GetCurrentQueryInfo();
            string startsWithVal = queryInfo.FilterInfo.FilterItems[0].Values.First().TrimEnd('%');
            IQueryable<SalesInfo> res = DB.Customer.AsNoTracking().Where(c => c.SalesPerson.StartsWith(startsWithVal))
                    .Select(s => s.SalesPerson)
                    .Distinct()
                    .OrderBy(s => s)
                    .Select(s => new SalesInfo { SalesPerson = s });

            List<SalesInfo> resPage = await res.Skip(queryInfo.PageIndex * queryInfo.PageSize).Take(queryInfo.PageSize).ToListAsync();

            return new QueryResult<SalesInfo>(resPage, await res.CountAsync());
        }

        [Query]
        public async Task<QueryResult<AddressInfo>> ReadAddressInfo()
        {
            List<AddressInfo> res = await this.PerformQuery(DB.Address.AsNoTracking())
                   .Select(a => new AddressInfo
                   {
                       AddressId = a.AddressId,
                       AddressLine1 = a.AddressLine1,
                       City = a.City,
                       CountryRegion = a.CountryRegion
                   }).ToListAsync();

            return new QueryResult<AddressInfo>(res, totalCount: null);
        }

        /// <summary>
        ///     if you return a Task<SomeType>
        ///     result from the Invoke method then it will be asynchronous
        ///     if instead you return SomeType type then the method will be executed synchronously
        /// </summary>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Invoke]
        public string TestInvoke(byte[] param1, string param2)
        {
            IHostAddrService ipAddressService = ServiceContainer.GetRequiredService<IHostAddrService>();
            string userIPaddress = ipAddressService.GetIPAddress();

            StringBuilder sb = new StringBuilder();

            Array.ForEach(param1, item =>
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(item);
            });

            /*
            int rand = (new Random(DateTime.Now.Millisecond)).Next(0, 999);
            if ((rand % 3) == 0)
                throw new Exception("Error generated randomly for testing purposes. Don't worry! Try again.");
            */

            return string.Format("<b>param1:</b> {0}<br/> <b>param2:</b> {1} User IP: {2}",
                    sb, param2, userIPaddress);
        }

        [Invoke]
        public byte[] TestComplexInvoke(AddressInfo info, KeyVal[] keys)
        {
            string vals = string.Join(",", keys?.Select(k => k.val).ToArray());
            // System.Diagnostics.Debug.WriteLine(info);
            // System.Diagnostics.Debug.WriteLine(vals);
            return BitConverter.GetBytes(DateTime.Now.Ticks);
        }

        #region CustomerJSON
        /// <summary>
        /// Contrived example of an entity which has JSON data in one of its fields
        /// just to show how to work with these entities on the client side
        /// </summary>
        /// <returns></returns>
        [Query]
        public async Task<QueryResult<CustomerJSON>> ReadCustomerJSON()
        {
            IQueryable<Customer> customers = DB.Customer.AsNoTracking().Where(c => c.CustomerAddress.Any());
            QueryRequest queryInfo = this.GetCurrentQueryInfo();
            int? totalCount = queryInfo.PageIndex == 0 ? 0 : (int?)null;
            // calculate totalCount only when we fetch first page (to speed up query)
            PerformQueryResult<Customer> custQueryResult = this.PerformQuery(customers, queryInfo.PageIndex == 0 ? (countQuery) => countQuery.CountAsync() : (Func<IQueryable<Customer>, Task<int>>)null);
            List<Customer> custList = await custQueryResult.Data.ToListAsync();

            // only execute total counting if we got full page size of rows, preventing unneeded database call to count total
            if (queryInfo.PageIndex == 0 && custList.Any())
            {
                int cnt = custList.Count;
                if (cnt < queryInfo.PageSize)
                {
                    totalCount = cnt;
                }
                else
                {
                    totalCount = await custQueryResult.CountAsync();
                }
            }

            var custAddressesList = await (from cust in custQueryResult.Data
                                           from custAddr in cust.CustomerAddress
                                           join addr in DB.Address on custAddr.AddressId equals addr.AddressId
                                           select new
                                           {
                                               CustomerId = custAddr.CustomerId,
                                               ID = addr.AddressId,
                                               Line1 = addr.AddressLine1,
                                               Line2 = addr.AddressLine2,
                                               City = addr.City,
                                               Region = addr.CountryRegion
                                           }).ToListAsync();

            var custAddressesLookup = custAddressesList.ToLookup((addr) => addr.CustomerId);

            // since i create JSON Data myself because there's no entity in db
            // which has json data in its fields
            IEnumerable<CustomerJSON> res = custList.Select(c => new CustomerJSON()
            {
                CustomerId = c.CustomerId,
                Rowguid = c.Rowguid,
                // serialize to json
                Data = Serializer.Serialize(new
                {
                    Title = c.Title,
                    CompanyName = c.CompanyName,
                    SalesPerson = c.SalesPerson,
                    ModifiedDate = c.ModifiedDate,
                    Level1 = new
                    {
                        FirstName = c.CustomerName.FirstName,
                        MiddleName = c.CustomerName.MiddleName,
                        LastName = c.CustomerName.LastName,
                        // another level of nesting to make it more complex
                        Level2 = new
                        {
                            EmailAddress = c.CustomerName.Contact.EmailAddress,
                            Phone = c.CustomerName.Contact.Phone

                        }
                    },
                    Addresses = custAddressesLookup[c.CustomerId].Select(ca => new { ca.Line1, ca.Line2, ca.City, ca.Region })
                })
            });

            return new QueryResult<CustomerJSON>(res, totalCount);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Insert]
        public void InsertCustomerJSON(CustomerJSON customer)
        {
            //make insert here
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Update]
        public void UpdateCustomerJSON(CustomerJSON customer)
        {
            //make update here
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Delete]
        public void DeleteCustomerJSON(CustomerJSON customer)
        {
            Customer entity = DB.Customer.Where(c => c.CustomerId == customer.CustomerId).Single();
            DB.Customer.Remove(entity);
        }

        #endregion

        #region Address

        [Query]
        public async Task<QueryResult<Address>> ReadAddress()
        {
            List<Address> res = await this.PerformQuery(DB.Address.AsNoTracking()).ToListAsync();
            return new QueryResult<Address>(res, totalCount: null);
        }

        [Query]
        public async Task<QueryResult<Address>> ReadAddressByIds(int[] addressIDs)
        {
            List<Address> res = await DB.Address.AsNoTracking().Where(ca => addressIDs.Contains(ca.AddressId)).ToListAsync();
            return new QueryResult<Address>(res, totalCount: null);
        }

        [Validate]
        public IEnumerable<ValidationErrorInfo> ValidateAddress(Address address, string[] modifiedField)
        {
            return Enumerable.Empty<ValidationErrorInfo>();
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Insert]
        public void InsertAddress(Address address)
        {
            DB.Address.Add(address);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Update]
        public void UpdateAddress(Address address)
        {
            address.ModifiedDate = DateTime.Now;
            Address orig = this.GetOriginal<Address>();
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Address> entry = DB.Address.Attach(address);
            /*
            var dbValues = entry.GetDatabaseValues();
            entry.OriginalValues.SetValues(dbValues);
            */
            entry.OriginalValues.SetValues(orig);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Delete]
        public void DeleteAddress(Address address)
        {
            DB.Address.Attach(address);
            DB.Address.Remove(address);
        }

        #endregion

        #region SalesOrderHeader

        [Query]
        public async Task<QueryResult<SalesOrderHeader>> ReadSalesOrderHeader()
        {
            List<SalesOrderHeader> res = await this.PerformQuery(DB.SalesOrderHeader.AsNoTracking()).ToListAsync();
            return new QueryResult<SalesOrderHeader>(res, totalCount: null);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Insert]
        public void InsertSalesOrderHeader(SalesOrderHeader salesorderheader)
        {
            salesorderheader.SalesOrderNumber = DateTime.Now.Ticks.ToString();
            DB.SalesOrderHeader.Add(salesorderheader);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Update]
        public void UpdateSalesOrderHeader(SalesOrderHeader salesorderheader)
        {
            salesorderheader.ModifiedDate = DateTime.Now;
            SalesOrderHeader orig = this.GetOriginal<SalesOrderHeader>();
            Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<SalesOrderHeader> entry = DB.SalesOrderHeader.Attach(salesorderheader);
            entry.OriginalValues.SetValues(orig);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Delete]
        public void DeleteSalesOrderHeader(SalesOrderHeader salesorderheader)
        {
            DB.SalesOrderHeader.Attach(salesorderheader);
            DB.SalesOrderHeader.Remove(salesorderheader);
        }

        #endregion

        #region SalesOrderDetail

        [Query]
        public async Task<QueryResult<SalesOrderDetail>> ReadSalesOrderDetail()
        {
            List<SalesOrderDetail> res = await this.PerformQuery(DB.SalesOrderDetail.AsNoTracking()).ToListAsync();
            return new QueryResult<SalesOrderDetail>(res, totalCount: null);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Insert]
        public void InsertSalesOrderDetail(SalesOrderDetail salesorderdetail)
        {
            DB.SalesOrderDetail.Add(salesorderdetail);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Update]
        public void UpdateSalesOrderDetail(SalesOrderDetail salesorderdetail)
        {
            salesorderdetail.ModifiedDate = DateTime.Now;
            SalesOrderDetail orig = this.GetOriginal<SalesOrderDetail>();
            DB.SalesOrderDetail.Attach(salesorderdetail);
            DB.Entry(salesorderdetail).OriginalValues.SetValues(orig);
        }

        [AuthorizeRoles(new[] { ADMINS_ROLE })]
        [Delete]
        public void DeleteSalesOrderDetail(SalesOrderDetail salesorderdetail)
        {
            DB.SalesOrderDetail.Attach(salesorderdetail);
            DB.SalesOrderDetail.Remove(salesorderdetail);
        }

        #endregion

        [Invoke]
        public async Task<DEMOCLS> GetClassifiers()
        {
            DEMOCLS res = new DEMOCLS
            {
                prodCategory = await DB.ProductCategory.OrderBy(l => l.Name).Select(d => new KeyVal { key = d.ProductCategoryId, val = d.Name }).ToListAsync(),
                prodDescription = await DB.ProductDescription.OrderBy(l => l.Description).Select(d => new KeyVal { key = d.ProductDescriptionId, val = d.Description }).ToListAsync(),
                prodModel = await DB.ProductModel.OrderBy(l => l.Name).Select(d => new KeyVal { key = d.ProductModelId, val = d.Name }).ToListAsync()
            };

            (res.prodModel as List<KeyVal>).Insert(0, new KeyVal() { key = -1, val = "Not Set (Empty)" });

            return res;
        }


    }
}