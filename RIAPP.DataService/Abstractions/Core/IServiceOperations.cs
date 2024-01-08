using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    public interface IServiceOperations
    {
        Task DeleteEntity(RunTimeMetadata metadata, RowInfo rowInfo);
      
        Task InsertEntity(RunTimeMetadata metadata, RowInfo rowInfo);

        Task UpdateEntity(RunTimeMetadata metadata, RowInfo rowInfo);

        Task<bool> ValidateEntity(RunTimeMetadata metadata, RequestContext requestContext);
    }

    public interface IServiceOperations<TService> : IServiceOperations
        where TService : BaseDomainService
    {

    }
}