using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;
using System;
using System.Security.Claims;

namespace RIAPP.DataService.Core
{
    public class RequestContext(BaseDomainService dataService,
        DbSet dbSet = null,
        ChangeSetRequest changeSet = null,
        RowInfo rowInfo = null,
        QueryRequest queryInfo = null,
        ServiceOperationType operation = ServiceOperationType.None) : IEntityVersionProvider
    {
        public static RequestContext Current
        {
            get
            {
                RequestContext reqCtxt = RequestCallContext.CurrentContext;
                if (reqCtxt == null)
                {
                    throw new InvalidOperationException("Current RequestCallContext is null");
                }

                return reqCtxt;
            }
        }

        public ClaimsPrincipal User => DataService.User;

        public DbSet CurrentDbSet { get; } = dbSet;

        public ChangeSetRequest CurrentChangeSet { get; } = changeSet;

        public RowInfo CurrentRowInfo { get; } = rowInfo;

        public QueryRequest CurrentQueryInfo { get; } = queryInfo;

        public ServiceOperationType CurrentOperation { get; } = operation;

        public dynamic DataBag => _dataBag.Value;

        public BaseDomainService DataService
        {
            get;
        } = dataService ?? throw new ArgumentNullException(nameof(dataService));

        #region Private Fields

        private readonly Lazy<dynamic> _dataBag = null;

        #endregion

        #region IEntityVersionProvider

        object IEntityVersionProvider.GetOriginal()
        {
            return DataService.ServiceContainer.EntityVersionHelper.GetOriginalEntity(CurrentRowInfo);
        }

        public object GetParent(Type entityType)
        {
            return DataService.ServiceContainer.EntityVersionHelper.GetParentEntity(entityType, CurrentRowInfo);
        }

        public TModel GetOriginal<TModel>()
            where TModel : class
        {
            return DataService.ServiceContainer.EntityVersionHelper.GetOriginalEntity<TModel>(CurrentRowInfo);
        }

        public TModel GetParent<TModel>()
            where TModel : class
        {
            return DataService.ServiceContainer.EntityVersionHelper.GetParentEntity<TModel>(CurrentRowInfo);
        }

        #endregion
    }
}