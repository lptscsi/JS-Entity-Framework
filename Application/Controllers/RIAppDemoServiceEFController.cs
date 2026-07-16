using Application.Models;
using Microsoft.AspNetCore.Mvc;
using RIAppDemo.BLL.DataServices;
using RIAppDemo.Utils;
using System.Threading.Tasks;

namespace RIAppDemo.Controllers
{
    /// <summary>
    /// Контроллер сервиса данных
    /// </summary>
    /// <param name="domainService"></param>
    [Route("api/demo")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RIAppDemoServiceEFController(RIAppDemoServiceEF domainService) : DataServiceController<RIAppDemoServiceEF>(domainService)
    {
        [Route("static")]
        [HttpGet]
        public async Task<ActionResult> PreloadData()
        {
            StaticData res = new StaticData()
            {
                ProductModelData = await DomainService.GetQueryData("ProductModel", "ReadProductModel"),
                ProductCategoryData = await DomainService.GetQueryData("ProductCategory", "ReadProductCategory")
            };
            return new ChunkedResult<StaticData>(res, DomainService.Serializer);
        }
    }
}