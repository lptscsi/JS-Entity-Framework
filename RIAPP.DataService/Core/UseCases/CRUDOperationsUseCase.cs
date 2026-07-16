using Pipeline;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Core.UseCases.CRUDMiddleware;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public class CRUDOperationsUseCase<TService>(BaseDomainService service,
        CRUDServiceMethods serviceMethods,
        RequestDelegate<CRUDContext<TService>> pipeline) : ICRUDOperationsUseCase<TService>
         where TService : BaseDomainService
    {
        private readonly IServiceContainer<TService> _serviceContainer = (IServiceContainer<TService>)service.ServiceContainer;
        private readonly BaseDomainService _service = service;
        private readonly CRUDServiceMethods _serviceMethods = serviceMethods;
        private readonly RequestDelegate<CRUDContext<TService>> _pipeline = pipeline;

        public async Task<bool> Handle(ChangeSetRequest message, IOutputPort<ChangeSetResponse> outputPort)
        {
            ChangeSetResponse response = new(message);

            try
            {
                CRUDContext<TService> context = new(message, response, (TService)_service, _serviceContainer);
                context.Properties.Add(CRUDContext<TService>.CHANGE_METHODS_KEY, _serviceMethods);

                await _pipeline(context);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                string err = _serviceMethods.OnError(ex);
                response.Error = new ErrorInfo(err, ex.GetType().Name);
            }

            outputPort.Handle(response);

            return response.Error == null;
        }
    }
}
