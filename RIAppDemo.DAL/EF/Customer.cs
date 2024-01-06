using System;
using System.Collections.Generic;

namespace RIAppDemo.DAL.EF
{
    public partial class Customer
    {
        public Customer()
        {
            CustomerAddress = new HashSet<CustomerAddress>();
            SalesOrderHeader = new HashSet<SalesOrderHeader>();
            CustomerName = new CustomerName();
        }

        public int CustomerId { get; set; }
        public bool NameStyle { get; set; }
        public string Title { get; set; }

        public CustomerName CustomerName { get; set; }

        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public Guid Rowguid { get; set; }
        public DateTime ModifiedDate { get; set; }
        /// <summary>
        ///     This field is for testing server side calculated fields
        /// </summary>
        public int? AddressCount { get; set; }

        public ICollection<CustomerAddress> CustomerAddress { get; set; }
        public ICollection<SalesOrderHeader> SalesOrderHeader { get; set; }
    }
}
