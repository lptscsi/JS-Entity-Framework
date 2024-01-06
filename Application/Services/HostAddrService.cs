using Microsoft.AspNetCore.Http;
using RIAppDemo.BLL.Utils;

namespace RIAppDemo.Services
{
    public class HostAddrService : IHostAddrService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HostAddrService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetIPAddress()
        {
            return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}
