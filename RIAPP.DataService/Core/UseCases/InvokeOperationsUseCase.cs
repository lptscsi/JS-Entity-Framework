using Pipeline;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Core.UseCases.InvokeMiddleware;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public class InvokeOperationsUseCase<TService>(BaseDomainService service, Func<Exception, string> onError, RequestDelegate<InvokeContext<TService>> pipeline) : IInvokeOperationsUseCase<TService>
         where TService : BaseDomainService
    {
        private readonly BaseDomainService _service = service;
        private readonly IServiceContainer<TService> _serviceContainer = (IServiceContainer<TService>)service.ServiceContainer;
        private readonly Func<Exception, string> _onError = onError ?? throw new ArgumentNullException(nameof(onError));
        private readonly RequestDelegate<InvokeContext<TService>> _pipeline = pipeline;

        public async Task<bool> Handle(InvokeRequest message, IOutputPort<InvokeResponse> outputPort)
        {
            InvokeResponse response = new();

            try
            {
                InvokeContext<TService> context = new(message,
                 response,
                 (TService)_service,
                 _serviceContainer);

                await _pipeline(context);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
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
