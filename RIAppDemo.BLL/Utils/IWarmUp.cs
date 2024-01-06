using System.Threading.Tasks;

namespace RIAppDemo.BLL.Utils
{
    /// <summary>
    /// Used for initialization of resources which are slow to initialize
    /// </summary>
    public interface IWarmUp
    {
        Task WarmUp();

        string Name { get; }
    }
}
