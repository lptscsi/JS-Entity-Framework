using System;
using System.Security.Claims;

namespace RIAppDemo.BLL
{
    public class RIAppDemoServiceEFOptions
    {
        public Func<IServiceProvider, ClaimsPrincipal> GetUser { get; set; }
        public string ConnectionString { get; set; }
    }
}
