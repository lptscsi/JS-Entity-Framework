using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<string> ProductModelData()
        {
            RIAPP.DataService.Core.Types.QueryResponse info = await DomainService.GetQueryData("ProductModel", "ReadProductModel");
            return DomainService.Serializer.Serialize(info);
        }

        [HttpGet]
        public async Task<string> ProductCategoryData()
        {
            RIAPP.DataService.Core.Types.QueryResponse info = await DomainService.GetQueryData("ProductCategory", "ReadProductCategory");
            return DomainService.Serializer.Serialize(info);
        }
    }
}