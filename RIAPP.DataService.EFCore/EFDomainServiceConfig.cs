using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RIAPP.DataService.Core.CodeGen;
using RIAPP.DataService.Core.Config;
using RIAPP.DataService.EFCore;
using RIAPP.DataService.EFCore.Utils;
using System;

namespace RIAppDemo.BLL.DataServices.Config
{
    /// <summary>
    /// Класс для добавления службы <see cref="EFDomainService{TDB}"/> в DI
    /// </summary>
    public static class EFDomainServiceConfig
    {
        /// <summary>
        /// Добавляет сервисы в DI
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TDB"></typeparam>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        public static void AddEFDomainService<TService, TDB>(this IServiceCollection services,
           Action<ServiceOptions> configure)
             where TService : EFDomainService<TDB>
             where TDB : DbContext
        {
            services.AddDomainService<TService>((options) =>
            {
                configure?.Invoke(options);
            });

            services.AddScoped<ICodeGenProviderFactory<TService>>((sp) =>
            {
                return new CsharpProviderFactory<TService, TDB>();
            });
        }
    }
}
