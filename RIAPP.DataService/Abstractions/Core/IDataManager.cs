using RIAPP.DataService.Core.Types;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core
{
    /// <summary>
    /// Interface that must be implemented by DataManagers to handle updates
    /// </summary>
    public interface IDataManager
    {
        Task AfterExecuteChangeSet(ChangeSetRequest changeSet);

        Task AfterChangeSetCommited(ChangeSetRequest changeSet, SubResultList refreshResult);
    }
}