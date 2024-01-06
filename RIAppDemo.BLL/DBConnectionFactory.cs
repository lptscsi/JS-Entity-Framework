using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.Common;

namespace RIAppDemo.BLL
{
    public class DBConnectionFactory
    {
        public const string CONNECTION_STRING_DEFAULT = "DBConnectionStringADW";

        public DBConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        private readonly IConfiguration _configuration;

        private string GetConnectionString(string name)
        {
            string connstring = _configuration[$"ConnectionStrings:{name}"];
            if (connstring == null)
            {
                throw new ApplicationException(string.Format("Connection string {0} is not found in config file", name));
            }
            return connstring;
        }

        public string GetRIAppDemoConnectionString()
        {
            string connStr = GetConnectionString(CONNECTION_STRING_DEFAULT);
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(connStr);
            return scsb.ToString();
        }

        public DbConnection GetRIAppDemoConnection()
        {
            string connStr = GetRIAppDemoConnectionString();
            DbConnection cn = SqlClientFactory.Instance.CreateConnection();
            cn.ConnectionString = connStr;
            if (cn.State == ConnectionState.Closed)
            {
                cn.Open();
            }

            return cn;
        }
    }
}