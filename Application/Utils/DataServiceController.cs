using Microsoft.AspNetCore.Mvc;
using RIAPP.DataService.Core;
using RIAPP.DataService.Core.CodeGen;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace RIAppDemo.Utils
{
    [Route("[controller]/[action]")]
    [ApiController]
    public abstract class DataServiceController<TService> : ControllerBase
        where TService : BaseDomainService
    {

        public DataServiceController(TService domainService)
        {
            DomainService = domainService;
        }

        protected TService DomainService
        {
            get;
        }


        #region CodeGen

        [ActionName("typescript")]
        [HttpGet]
        public ActionResult GetTypeScript()
        {
            string url = $"{ControllerContext.HttpContext.Request.Path}{ControllerContext.HttpContext.Request.QueryString}";
            DateTime now = DateTime.Now;
            string comment = $"\tGenerated from: {url} on {now:yyyy-MM-dd} at {now:HH:mm}\r\n\tDon't make manual changes here, they will be lost when this interface will be regenerated!";
            string content = DomainService.ServiceCodeGen(new CodeGenArgs("ts") { comment = comment });
            ContentResult res = new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = content
            };
            return res;
        }

        [ActionName("xaml")]
        [HttpGet]
        public ActionResult GetXAML(bool isDraft = true)
        {
            string content = DomainService.ServiceCodeGen(new CodeGenArgs("xaml") { isDraft = isDraft });
            ContentResult res = new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = content
            };
            return res;
        }

        [ActionName("csharp")]
        [HttpGet]
        public ActionResult GetCSharp()
        {
            string content = DomainService.ServiceCodeGen(new CodeGenArgs("csharp"));
            ContentResult res = new ContentResult
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = content
            };
            return res;
        }

        [ActionName("code")]
        [HttpGet("{lang?}")]
        public ActionResult GetCode(string lang)
        {
            if (string.IsNullOrEmpty(lang))
            {
                lang = Request.Query["lang"];
            }

            switch (lang?.ToLowerInvariant())
            {
                case "ts":
                case "typescript":
                    return GetTypeScript();
                case "xml":
                case "xaml":
                    return GetXAML();
                case "cs":
                case "csharp":
                    return GetCSharp();
                default:
                    throw new Exception(string.Format("Unknown lang argument: {0}", lang));
            }
        }

        #endregion

        [ActionName("metadata")]
        [HttpGet]
        public ActionResult GetMetadata()
        {
            MetadataResult res = DomainService.ServiceGetMetadata();
            return new ChunkedResult<MetadataResult>(res, DomainService.Serializer);
        }

        [ActionName("permissions")]
        [HttpGet]
        public async Task<ActionResult> GetPermissions()
        {
            Permissions res = await DomainService.ServiceGetPermissions();
            return new ChunkedResult<Permissions>(res, DomainService.Serializer);
        }

        [ActionName("query")]
        [HttpPost]
        public async Task<ActionResult> PerformQuery()
        {
            try
            {
                string body = await Request.GetRawBodyAsync();
                QueryRequest request = (QueryRequest)DomainService.Serializer.DeSerialize(body, typeof(QueryRequest));
                QueryResponse res = await DomainService.ServiceGetData(request);
                return new ChunkedResult<QueryResponse>(res, DomainService.Serializer);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [ActionName("save")]
        [HttpPost]
        public async Task<ActionResult> Save()
        {
            string body = await Request.GetRawBodyAsync();
            ChangeSetRequest changeSet = (ChangeSetRequest)DomainService.Serializer.DeSerialize(body, typeof(ChangeSetRequest));
            ChangeSetResponse res = await DomainService.ServiceApplyChangeSet(changeSet);
            return new ChunkedResult<ChangeSetResponse>(res, DomainService.Serializer);
        }

        [ActionName("refresh")]
        [HttpPost]
        public async Task<ActionResult> Refresh()
        {
            string body = await Request.GetRawBodyAsync();
            RefreshRequest refreshInfo = (RefreshRequest)DomainService.Serializer.DeSerialize(body, typeof(RefreshRequest));
            RefreshResponse res = await DomainService.ServiceRefreshRow(refreshInfo);
            return new ChunkedResult<RefreshResponse>(res, DomainService.Serializer);
        }

        [ActionName("invoke")]
        [HttpPost]
        public async Task<ActionResult> Invoke()
        {
            string body = await Request.GetRawBodyAsync();
            InvokeRequest invokeInfo = (InvokeRequest)DomainService.Serializer.DeSerialize(body, typeof(InvokeRequest));
            InvokeResponse res = await DomainService.ServiceInvokeMethod(invokeInfo);
            return new ChunkedResult<InvokeResponse>(res, DomainService.Serializer);
        }
    }
}
