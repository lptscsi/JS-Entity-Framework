using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RIAppDemo.BLL.Models;
using RIAppDemo.BLL.Utils;
using RIAppDemo.DAL.EF;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace RIAppDemo.BLL.DataServices.Config
{
    public static class RIAppDemoServiceEFConfig
    {
        public class CommandInterceptor : DbCommandInterceptor
        {
            public override DbCommand CommandCreated(CommandEndEventData eventData, DbCommand result)
            {
                return base.CommandCreated(eventData, result);
            }
            public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
            {
                return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
            }

            public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
            {
                return base.ReaderExecuting(command, eventData, result);
            }
        }

        public static void AddRIAppDemoService(this IServiceCollection services,
           Action<RIAppDemoServiceEFOptions> configure)
        {
            services.AddEFDomainService<RIAppDemoServiceEF, AdventureWorksLT2012Context>((options) =>
            {
                options.ClientTypes = () => new[] { typeof(TestModel),
                    typeof(KeyVal), typeof(StrKeyVal),
                    typeof(RadioVal), typeof(HistoryItem), typeof(TestEnum2) };


                RIAppDemoServiceEFOptions svcOptions = new RIAppDemoServiceEFOptions();
                configure?.Invoke(svcOptions);

                options.UserFactory = svcOptions.GetUser;

                string connString = svcOptions.ConnectionString ?? throw new ArgumentNullException(nameof(svcOptions.ConnectionString));

                services.AddDbContext<AdventureWorksLT2012Context>((dbOptions) =>
                {
                    dbOptions.UseSqlServer(connString, (sqlOptions) =>
                    {
                        // sqlOptions.UseRowNumberForPaging();
                    }).AddInterceptors(new CommandInterceptor());
                }, ServiceLifetime.Transient);
            });

            services.AddScoped<IWarmUp>((sp => sp.GetRequiredService<RIAppDemoServiceEF>()));
        }
    }
}
