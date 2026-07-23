using Pipeline;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Core.UseCases.RefreshMiddleware;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public class RefreshOperationsUseCase<TService>(BaseDomainService service, Func<Exception, string> onError, RequestDelegate<RefreshContext<TService>> pipeline) : IRefreshOperationsUseCase<TService>
         where TService : BaseDomainService
    {
        private readonly BaseDomainService _service = service;
        private readonly IServiceContainer<TService> _serviceContainer = (IServiceContainer<TService>)service.ServiceContainer;
        private readonly Func<Exception, string> _onError = onError ?? throw new ArgumentNullException(nameof(onError));
        private readonly RequestDelegate<RefreshContext<TService>> _pipeline = pipeline;

        public async Task<bool> Handle(RefreshRequest message, IOutputPort<RefreshResponse> outputPort)
        {
            RefreshResponse response = new() { rowInfo = message.rowInfo, dbSetName = message.dSetName };

            try
            {
                Metadata.RunTimeMetadata metadata = _service.GetMetadata();
                DbSetInfo dbSetInfo = metadata.DbSets.Get(message.dSetName) ?? throw new InvalidOperationException($"The DbSet {message.dSetName} was not found in metadata");
                message.SetDbSetInfo(dbSetInfo);
                message.rowInfo.SetDbSetInfo(dbSetInfo);

                RefreshContext<TService> context = new(message,
                response,
                (TService)_service,
                _serviceContainer);

                await _pipeline(context);
            }
            catch (Exception ex)
            {

                if (ex is System.Reflection.TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                string err = _onError(ex);
                response.error = new ErrorInfo(err, ex.GetType().Name);
            }

            outputPort.Handle(response);

            return true;
        }
    }
}
