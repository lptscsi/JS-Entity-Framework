using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RIAPP.DataService.Core.Config
{
    public interface IServiceOptions
    {
        Func<IServiceProvider, ClaimsPrincipal> UserFactory { get; }

        Func<IEnumerable<Type>> ClientTypes { get; }

        IServiceCollection Services { get; }
    }
}