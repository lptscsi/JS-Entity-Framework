using Microsoft.Extensions.DependencyInjection;
using RIAPP.DataService.Core.Config;
using RIAppDemo.BLL.Utils;
using System;

namespace RIAppDemo.BLL.DataServices.Config
{
    public static class FolderBrowserServiceConfig
    {
        public static void AddFolderBrowser(this IServiceCollection services,
            Action<FolderBrowserServiceOptions> configure)
        {
            services.AddDomainService<FolderBrowserService>((options) =>
            {
                FolderBrowserServiceOptions svcOptions = new FolderBrowserServiceOptions();
                configure?.Invoke(svcOptions);

                options.UserFactory = svcOptions.GetUser;
            });

            services.AddScoped<IWarmUp>((sp => sp.GetRequiredService<FolderBrowserService>()));
        }
    }
}
