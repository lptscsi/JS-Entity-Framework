using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RIAPP.DataService.Core.Config
{
    /// <summary>
    /// Опции для сервиса данных
    /// </summary>
    /// <param name="services"></param>
    public class ServiceOptions(IServiceCollection services) : IServiceOptions
    {
        public Func<IServiceProvider, ClaimsPrincipal> UserFactory { get; set; }

        /// <summary>
        /// Типы данных для кодогенерации typescript
        /// </summary>
        public Func<IEnumerable<Type>> ClientTypes { get; set; }

        /// <summary>
        /// Путь для импорта типов из библиотеки JRIAPP (для кодогенерации typescript)
        /// </summary>
        public string JriappImportPath { get; set; } = "jriapp-lib";

        /// <summary>
        /// Набор зарегистрированных сервисов
        /// </summary>
        public IServiceCollection Services => services;
    }
}