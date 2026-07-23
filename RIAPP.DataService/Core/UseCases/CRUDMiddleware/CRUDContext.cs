using Pipeline;
using RIAPP.DataService.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace RIAPP.DataService.Core.UseCases.CRUDMiddleware
{
    public class CRUDContext<TService>(
        ChangeSetRequest request,
        ChangeSetResponse response,
        TService service,
        IServiceContainer<TService> serviceContainer) : IRequestContext
        where TService : BaseDomainService
    {
        public const string CHANGE_GRAPH_KEY = "change_graph";
        public const string CHANGE_METHODS_KEY = "change_methods";

        private ExceptionDispatchInfo _ExceptionInfo = null;

        public static RequestContext CreateRequestContext(TService service, ChangeSetRequest changeSet, RowInfo rowInfo = null)
        {
            DbSet dbSet = rowInfo == null ? null : changeSet.dbSets.Where(d => d.dbSetName == rowInfo.GetDbSetInfo().dbSetName).Single();
            return new RequestContext(service, changeSet: changeSet, dbSet: dbSet, rowInfo: rowInfo,
                operation: ServiceOperationType.SaveChanges);
        }


        // Gets a key/value collection that can be used to share data between middleware.
        public IDictionary<string, object> Properties { get; } = new Expando();

        public void AddLogItem(string str)
        {
        }

        public ChangeSetRequest Request { get; } = request;
        public ChangeSetResponse Response { get; } = response;
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
