using Application.Models;
using Microsoft.AspNetCore.Mvc;
using RIAPP.DataService.Core.Types;
using RIAppDemo.BLL.DataServices;
using RIAppDemo.Utils;
using System.Threading.Tasks;

namespace RIAppDemo.Controllers
{
    public class RIAppDemoServiceEFController : DataServiceController<RIAppDemoServiceEF>
    {
        public RIAppDemoServiceEFController(RIAppDemoServiceEF domainService) :
            base(domainService)
        {

        }

        [ActionName("static")]
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