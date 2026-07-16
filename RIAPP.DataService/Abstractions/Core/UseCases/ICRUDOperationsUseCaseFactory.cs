using RIAPP.DataService.Core.Types;
using System;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public delegate Task ChangeSetExecutor();
    public delegate Task AfterChangeSetExecuted();
    public delegate Task AfterChangeSetCommited(SubResultList subResults);

    public class CRUDServiceMethods(Func<Exception, string> onError,
    Action<RowInfo> trackChanges,
    ChangeSetExecutor executeChangeSet,
    AfterChangeSetExecuted afterChangeSetExecuted,
    AfterChangeSetCommited subResultsExecutor)
    {
        public Func<Exception, string> OnError { get; } = onError;
        public Action<RowInfo> TrackChanges { get; } = trackChanges;
        public ChangeSetExecutor ExecuteChangeSet { get; } = executeChangeSet;
        public AfterChangeSetExecuted AfterChangeSetExecuted { get; } = afterChangeSetExecuted;
        public AfterChangeSetCommited AfterChangeSetCommited { get; } = subResultsExecutor;
    }


    public interface ICRUDOperationsUseCaseFactory
    {
        ICRUDOperationsUseCase Create(BaseDomainService service, CRUDServiceMethods serviceMethods);
    }

    public interface ICRUDOperationsUseCaseFactory<TService> : ICRUDOperationsUseCaseFactory
        where TService : BaseDomainService
    {
        ICRUDOperationsUseCase<TService> Create(TService service, CRUDServiceMethods serviceMethods);
    }

}
