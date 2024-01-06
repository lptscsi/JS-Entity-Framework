using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using RIAPP.DataService.Utils;
using System.Threading.Tasks;

namespace RIAppDemo.Utils
{
    public class ChunkedResult<T> : ActionResult
        where T : class
    {
        private static readonly string ResultContentType = "application/json";
        private readonly ISerializer _serializer;

        public ChunkedResult(T data, ISerializer serializer)
        {
            Data = data;
            _serializer = serializer;
        }

        public T Data { get; }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            Microsoft.AspNetCore.Http.HttpResponse response = context.HttpContext.Response;
            response.ContentType = ResultContentType;
            System.IO.Stream stream = response.Body;

            IHttpResponseBodyFeature bufferingFeature = context.HttpContext.Features.Get<IHttpResponseBodyFeature>();
            if (bufferingFeature != null)
            {
                bufferingFeature.DisableBuffering();
            }

            return _serializer.SerializeAsync<T>(Data, stream);
        }
    }
}