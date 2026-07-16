using Pipeline;
using RIAPP.DataService.Core.Types;
using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;

namespace RIAPP.DataService.Core.UseCases.QueryMiddleware
{
    public class QueryContext<TService>(
        QueryRequest request,
        QueryResponse response,
        TService service,
        IServiceContainer<TService> serviceContainer,
        bool isMultyPage) : IRequestContext
        where TService : BaseDomainService
    {
        private ExceptionDispatchInfo _ExceptionInfo = null;

        public static RequestContext CreateRequestContext(TService service, QueryRequest queryInfo)
        {
            return new RequestContext(service, queryInfo: queryInfo, operation: ServiceOperationType.Query);
        }


        // Gets a key/value collection that can be used to share data between middleware.
        public IDictionary<string, object> Properties { get; } = new Expando();

        public bool IsMultyPage { get; } = isMultyPage;

        public void AddLogItem(string str)
        {
        }

        public QueryRequest Request { get; } = request;
        public QueryResponse Response { get; } = response;
        public IServiceProvider RequestServices => ServiceContainer.ServiceProvider;

        public void CaptureException(Exception ex)
        {
            _ExceptionInfo = ExceptionDispatchInfo.Capture(ex);
        }

        public Exception ProcessingException => _ExceptionInfo?.SourceException;

        public IServiceContainer<TService> ServiceContainer { get; } = serviceContainer;

        public TService Service { get; } = service;

        public void ReThrow()
        {
            _ExceptionInfo?.Throw();
        }
    }
}
