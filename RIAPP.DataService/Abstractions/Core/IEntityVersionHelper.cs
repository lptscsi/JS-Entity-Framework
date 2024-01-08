using RIAPP.DataService.Core.Types;
using System;

namespace RIAPP.DataService.Core
{
    public interface IEntityVersionHelper
    {
        object GetOriginalEntity(RowInfo rowInfo);

        object GetOriginalEntity(object entity, RowInfo rowInfo);

        T GetOriginalEntity<T>(RowInfo rowInfo)
            where T : class;
        object GetParentEntity(Type entityType, RowInfo rowInfo);

        T GetParentEntity<T>(RowInfo rowInfo)
            where T : class;
    }

    public interface IEntityVersionHelper<TService> : IEntityVersionHelper
        where TService : BaseDomainService
    {

    }
}