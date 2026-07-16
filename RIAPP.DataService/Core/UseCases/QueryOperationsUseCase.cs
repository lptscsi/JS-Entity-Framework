using Pipeline;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Core.UseCases.QueryMiddleware;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public class QueryOperationsUseCase<TService>(BaseDomainService service, Func<Exception, string> onError, RequestDelegate<QueryContext<TService>> pipeline) : IQueryOperationsUseCase<TService>
         where TService : BaseDomainService
    {
        private readonly BaseDomainService _service = service;
        private readonly IServiceContainer<TService> _serviceContainer = (IServiceContainer<TService>)service.ServiceContainer;
        private readonly Func<Exception, string> _onError = onError ?? throw new ArgumentNullException(nameof(onError));
        private readonly RequestDelegate<QueryContext<TService>> _pipeline = pipeline;

        public async Task<bool> Handle(QueryRequest message, IOutputPort<QueryResponse> outputPort)
        {
            QueryResponse response = new()
            {
                PageIndex = message.PageIndex,
                PageCount = message.PageCount,
                DbSetName = message.DbSetName,
                Rows = new Row[0],
                TotalCount = null,
                Error = null
            };

            try
            {
                Metadata.RunTimeMetadata metadata = _service.GetMetadata();
                DbSetInfo dbSetInfo = metadata.DbSets.Get(message.DbSetName) ?? throw new InvalidOperationException($"The DbSet {message.DbSetName} was not found in metadata");
                message.SetDbSetInfo(dbSetInfo);

                bool isMultyPageRequest = dbSetInfo.enablePaging && message.PageCount > 1;

                QueryContext<TService> context = new(message,
                    response,
                    (TService)_service,
                    _serviceContainer,
                    isMultyPageRequest);

                await _pipeline(context);
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException)
                {
                    ex = ex.InnerException;
                }

                string err = _onError(ex);
                response.Error = new ErrorInfo(err, ex.GetType().Name);
            }

            outputPort.Handle(response);

            return true;
        }
    }
}
