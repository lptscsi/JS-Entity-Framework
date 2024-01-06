using RIAppDemo.BLL.DataServices;
using RIAppDemo.Utils;

namespace RIAppDemo.Controllers
{
    public class FolderBrowserServiceController : DataServiceController<FolderBrowserService>
    {
        public FolderBrowserServiceController(FolderBrowserService domainService) :
            base(domainService)
        {
        }
    }
}