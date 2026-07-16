using Microsoft.AspNetCore.Mvc;
using RIAppDemo.BLL.DataServices;
using RIAppDemo.Utils;

namespace RIAppDemo.Controllers
{
    /// <summary>
    /// Контроллер сервиса данных для UI импорта файлов
    /// </summary>
    /// <param name="domainService"></param>
    [Route("api/folders")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class FolderBrowserServiceController(FolderBrowserService domainService) : DataServiceController<FolderBrowserService>(domainService)
    {
    }
}