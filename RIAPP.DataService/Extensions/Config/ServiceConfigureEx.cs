using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pipeline;
using RIAPP.DataService.Core.CodeGen;
using RIAPP.DataService.Core.Config;
using RIAPP.DataService.Core.Security;
using RIAPP.DataService.Core.UseCases;
using RIAPP.DataService.Core.UseCases.CRUDMiddleware;
using RIAPP.DataService.Core.UseCases.InvokeMiddleware;
using RIAPP.DataService.Core.UseCases.QueryMiddleware;
using RIAPP.DataService.Core.UseCases.RefreshMiddleware;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils;
using System;

namespace RIAPP.DataService.Core.Config
{
    public static class ServiceConfigureEx
    {
        public static void AddDomainService<TService>(this IServiceCollection services,
            Action<ServiceOptions> configure)
         where TService : BaseDomainService
        {
            ServiceOptions options = new ServiceOptions(services);
            configure?.Invoke(options);

            Func<IServiceProvider, System.Security.Claims.ClaimsPrincipal> getUser = options.UserFactory ?? throw new ArgumentNullException(nameof(options.UserFactory), ErrorStrings.ERR_NO_USER);

            services.TryAddScoped<IUserProvider>((sp) => new UserProvider(() => getUser(sp)));

            services.TryAddScoped<IAuthorizer<TService>, Authorizer<TService>>();

            services.TryAddSingleton<IValueConverter<TService>, ValueConverter<TService>>();

            services.TryAddSingleton<IDataHelper<TService>, DataHelper<TService>>();

            services.TryAddSingleton<IValidationHelper<TService>, ValidationHelper<TService>>();

            services.TryAddScoped<IServiceOperations<TService>, ServiceOperations<TService>>();

            services.TryAddScoped<IServiceOperationsHelper<TService>, ServiceOperationsHelper<TService>>();

            services.TryAddScoped<IEntityVersionHelper<TService>>(sp =>
            {
                return (IEntityVersionHelper<TService>)sp.GetRequiredService<IServiceOperationsHelper<TService>>();
            });

            services.TryAddScoped<IServiceContainer<TService>, ServiceContainer<TService>>();

            #region Pipeline

            services.TryAddSingleton((sp) =>
            {
                PipelineBuilder<TService, CRUDContext<TService>> builder = new PipelineBuilder<TService, CRUDContext<TService>>(sp);
                Configuration.ConfigureCRUD(builder);
                return builder.Build();
            });

            services.TryAddSingleton((sp) =>
            {
                PipelineBuilder<TService, QueryContext<TService>> builder = new PipelineBuilder<TService, QueryContext<TService>>(sp);
                Configuration.ConfigureQuery(builder);
                return builder.Build();
            });

            services.TryAddSingleton((sp) =>
            {
                PipelineBuilder<TService, InvokeContext<TService>> builder = new PipelineBuilder<TService, InvokeContext<TService>>(sp);
                Configuration.ConfigureInvoke(builder);
                return builder.Build();
            });

            services.TryAddSingleton((sp) =>
            {
                PipelineBuilder<TService, RefreshContext<TService>> builder = new PipelineBuilder<TService, RefreshContext<TService>>(sp);
                Configuration.ConfigureRefresh(builder);
                return builder.Build();
            });

            #endregion

            #region  CodeGen

            services.TryAddScoped<ICodeGenFactory<TService>, CodeGenFactory<TService>>();

            services.AddScoped<ICodeGenProviderFactory<TService>>((sp) =>
            {
                return new XamlProviderFactory<TService>();
            });

            services.AddScoped<ICodeGenProviderFactory<TService>>((sp) =>
            {
                IServiceContainer<TService> sc = sp.GetRequiredService<IServiceContainer<TService>>();
                return new TypeScriptProviderFactory<TService>(sc, options.ClientTypes);
            });

            #endregion

            #region UseCases
            ObjectFactory crudCaseFactory = ActivatorUtilities.CreateFactory(typeof(CRUDOperationsUseCase<TService>),
                new System.Type[] { typeof(BaseDomainService),
                typeof(CRUDServiceMethods)
            });

            services.TryAddScoped<ICRUDOperationsUseCaseFactory<TService>>((sp) => new CRUDOperationsUseCaseFactory<TService>((svc, serviceMethods) =>
                (ICRUDOperationsUseCase<TService>)crudCaseFactory(sp, new object[] { svc, serviceMethods })));

            ObjectFactory queryCaseFactory = ActivatorUtilities.CreateFactory(typeof(QueryOperationsUseCase<TService>), new System.Type[] { typeof(BaseDomainService), typeof(Func<Exception, string>) });

            services.TryAddScoped<IQueryOperationsUseCaseFactory<TService>>((sp) => new QueryOperationsUseCaseFactory<TService>((svc, onError) =>
                (IQueryOperationsUseCase<TService>)queryCaseFactory(sp, new object[] { svc, onError })));

            ObjectFactory refreshCaseFactory = ActivatorUtilities.CreateFactory(typeof(RefreshOperationsUseCase<TService>), new System.Type[] { typeof(BaseDomainService), typeof(Func<Exception, string>) });

            services.TryAddScoped<IRefreshOperationsUseCaseFactory<TService>>((sp) => new RefreshOperationsUseCaseFactory<TService>((svc, onError) =>
                (IRefreshOperationsUseCase<TService>)refreshCaseFactory(sp, new object[] { svc, onError })));

            ObjectFactory invokeCaseFactory = ActivatorUtilities.CreateFactory(typeof(InvokeOperationsUseCase<TService>), new System.Type[] { typeof(BaseDomainService), typeof(Func<Exception, string>) });

            services.TryAddScoped<IInvokeOperationsUseCaseFactory<TService>>((sp) => new InvokeOperationsUseCaseFactory<TService>((svc, onError) =>
                (IInvokeOperationsUseCase<TService>)invokeCaseFactory(sp, new object[] { svc, onError })));

            services.TryAddTransient(typeof(IResponsePresenter<,>), typeof(OperationOutput<,>));
            #endregion


            ObjectFactory serviceFactory = ActivatorUtilities.CreateFactory(typeof(TService), new Type[] { typeof(IServiceContainer<TService>) });

            services.TryAddScoped<TService>((sp) =>
            {
                IServiceContainer<TService> sc = sp.GetRequiredService<IServiceContainer<TService>>();
                return (TService)serviceFactory(sp, new object[] { sc });
            });
        }
    }
}