using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public interface IServiceOperationsHelper
    {
        Task AfterExecuteChangeSet(ChangeSetRequest message);

        Task AfterChangeSetCommited(ChangeSetRequest changeSet, SubResultList refreshResult);

        object GetMethodOwner(string dbSetName, MethodInfoData methodData);
        void ApplyValues(object entity, RowInfo rowInfo, string path, ValueChange[] values, bool isOriginal);
        void CheckValuesChanges(RowInfo rowInfo, string path, ValueChange[] values);
        bool IsEntityValueChanged(RowInfo rowInfo, string fullName, Field fieldInfo, out string newVal);
        void UpdateEntityFromRowInfo(object entity, RowInfo rowInfo, bool isOriginal);
        void UpdateRowInfoAfterUpdates(RowInfo rowInfo);
        void UpdateRowInfoFromEntity(object entity, RowInfo rowInfo);
        void UpdateValuesFromEntity(object entity, string path, DbSetInfo dbSetInfo, ValueChange[] values);
    }

    public interface IServiceOperationsHelper<TService> : IServiceOperationsHelper
        where TService : BaseDomainService
    {

    }
}