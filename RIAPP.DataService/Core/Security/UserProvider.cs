using System;
using System.Security.Claims;

namespace RIAPP.DataService.Core.Security
{
    public class UserProvider(Func<ClaimsPrincipal> userFactory) : IUserProvider
    {
        private readonly Lazy<ClaimsPrincipal> _user = new Lazy<ClaimsPrincipal>(userFactory, true);

        public ClaimsPrincipal User => _user.Value;
    }
}
