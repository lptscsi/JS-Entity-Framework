using Microsoft.AspNetCore.Mvc;
using RIAPP.DataService.Core;
using RIAPP.DataService.Core.CodeGen;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

#nullable enable

namespace RIAppDemo.Utils
{
    /// <summary>
    /// Базовый контроллер для реализации специализированного контроллера для сервиса данных
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="domainService"></param>
    [ApiController]
    public abstract class DataServiceController<TService>(TService domainService) : ControllerBase
        where TService : BaseDomainService
    {
        /// <summary>
        /// Экземпляр сервиса данных
        /// </summary>
        protected TService DomainService => domainService;


        #region CodeGen

        /// <summary>
        /// Возвращает реализацию сервиса на языке typescript
        /// </summary>
        /// <returns></returns>
        [Route("ts")]
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

        /// <summary>
        /// Возвращает конфигурацию сервиса в формате xml
        /// </summary>
        /// <returns></returns>
        [Route("xaml")]
        [HttpGet]
        public ActionResult GetXAML(bool isDraft = true)
        {
            string content = DomainService.ServiceCodeGen(new CodeGenArgs("xaml") { isDraft = isDraft });
            ContentResult res = new()
            {
                ContentType = MediaTypeNames.Text.Plain,
                Content = content
            };
            return res;
        }

        /// <summary>
        /// Возвращает реализацию сервиса на языке C#
        /// </summary>
        /// <returns></returns>
        [Route("csharp")]
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

        /// <summary>
        /// Возвращает результат сформированный в зависимости от параметра lang
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [Route("code/{lang?}")]
        [HttpGet]
        public ActionResult GetCode(string? lang)
        {
            if (string.IsNullOrEmpty(lang))
            {
                lang = Request.Query["lang"];
            }

            return (lang?.ToLowerInvariant()) switch
            {
                "ts" or "typescript" => GetTypeScript(),
                "xml" or "xaml" => GetXAML(),
                "cs" or "csharp" => GetCSharp(),
                _ => throw new Exception(string.Format("Unknown lang argument: {0}", lang)),
            };
        }

        #endregion

        /// <summary>
        /// Возвращает метаданные
        /// </summary>
        /// <returns></returns>
        [Route("metadata")]
        [HttpGet]
        public ActionResult GetMetadata()
        {
            MetadataResult res = DomainService.ServiceGetMetadata();
            return new ChunkedResult<MetadataResult>(res, DomainService.Serializer);
        }

        /// <summary>
        /// Возвращает разрешения
        /// </summary>
        /// <returns></returns>
        [Route("permissions")]
        [HttpGet]
        public async Task<ActionResult> GetPermissions()
        {
            Permissions res = await DomainService.ServiceGetPermissions();
            return new ChunkedResult<Permissions>(res, DomainService.Serializer);
        }

        /// <summary>
        /// Выполняет запрос
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Route("query")]
        [HttpPost]
        public async Task<ActionResult> PerformQuery([FromBody] QueryRequest request)
        {
            QueryResponse res = await DomainService.ServiceGetData(request);
            return new ChunkedResult<QueryResponse>(res, DomainService.Serializer);
        }

        /// <summary>
        /// Выполняет сохранение изменений
        /// </summary>
        /// <param name="changeSet"></param>
        /// <returns></returns>
        [Route("save")]
        [HttpPost]
        public async Task<ActionResult> Save([FromBody] ChangeSetRequest changeSet)
        {
            ChangeSetResponse res = await DomainService.ServiceApplyChangeSet(changeSet);
            return new ChunkedResult<ChangeSetResponse>(res, DomainService.Serializer);
        }

        /// <summary>
        /// Выполняет обновление
        /// </summary>
        /// <param name="refreshInfo"></param>
        /// <returns></returns>
        [Route("refresh")]
        [HttpPost]
        public async Task<ActionResult> Refresh([FromBody] RefreshRequest refreshInfo)
        {
            RefreshResponse res = await DomainService.ServiceRefreshRow(refreshInfo);
            return new ChunkedResult<RefreshResponse>(res, DomainService.Serializer);
        }

        /// <summary>
        /// Вызывает метод сервиса
        /// </summary>
        /// <param name="invokeInfo"></param>
        /// <returns></returns>
        [Route("invoke")]
        [HttpPost]
        public async Task<ActionResult> Invoke([FromBody] InvokeRequest invokeInfo)
        {
            InvokeResponse res = await DomainService.ServiceInvokeMethod(invokeInfo);
            return new ChunkedResult<InvokeResponse>(res, DomainService.Serializer);
        }
    }
}
