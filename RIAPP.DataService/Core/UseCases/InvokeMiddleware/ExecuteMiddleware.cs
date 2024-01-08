using Pipeline;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core.UseCases.InvokeMiddleware
{
    public class ExecuteMiddleware<TService>
         where TService : BaseDomainService
    {
        private readonly RequestDelegate<InvokeContext<TService>> _next;

        public ExecuteMiddleware(RequestDelegate<InvokeContext<TService>> next, InvokeMiddlewareOptions<TService> options)
        {
            _next = next;
        }

        public async Task Invoke(InvokeContext<TService> ctx)
        {
            IDataHelper<TService> dataHelper = ctx.ServiceContainer.GetDataHelper();
            IServiceOperationsHelper<TService> serviceHelper = ctx.ServiceContainer.GetServiceHelper();
            RunTimeMetadata metadata = ctx.Service.GetMetadata();
            MethodDescription method = metadata.GetInvokeMethod(ctx.Request.methodName);

            List<object> methParams = new List<object>();
            for (int i = 0; i < method.parameters.Count; ++i)
            {
                methParams.Add(ctx.Request.paramInfo.GetValue(method.parameters[i].name, method, dataHelper));
            }
            RequestContext req = InvokeContext<TService>.CreateRequestContext(ctx.Service);
            using (RequestCallContext callContext = new RequestCallContext(req))
            {
                MethodInfoData methodData = method.GetMethodData();
                // invoke (aka service methods) are on the Domain Service only, so dbSetName is empty
                object instance = serviceHelper.GetMethodOwner(string.Empty, methodData);
                object invokeRes = methodData.MethodInfo.Invoke(instance, methParams.ToArray());
                object methodResult = await PropHelper.GetMethodResult(invokeRes);

                if (method.methodResult)
                {
                    ctx.Response.result = methodResult;
                }
            }

            await _next(ctx);
        }
    }
}
