using System;
using System.Security.Claims;

namespace RIAppDemo.BLL
{
    public class FolderBrowserServiceOptions
    {
        public Func<IServiceProvider, ClaimsPrincipal> GetUser { get; set; }
    }
}
