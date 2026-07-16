using Pipeline;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;
using System;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core.UseCases.RefreshMiddleware
{
    public class ExecuteMiddleware<TService>
         where TService : BaseDomainService
    {
        private readonly RequestDelegate<RefreshContext<TService>> _next;

        public ExecuteMiddleware(RequestDelegate<RefreshContext<TService>> next, RefreshMiddlewareOptions<TService> options)
        {
            _next = next;
        }

        public async Task Invoke(RefreshContext<TService> ctx)
        {
            DbSetInfo dbSetInfo = ctx.Request.GetDbSetInfo() ?? throw new InvalidOperationException($"Could not get the DbSet for {ctx.Request.DbSetName}");
            IServiceOperationsHelper<TService> serviceHelper = ctx.ServiceContainer.GetServiceHelper();
            RunTimeMetadata metadata = ctx.Service.GetMetadata();

            RequestContext req = RefreshContext<TService>.CreateRequestContext(ctx.Service, ctx.Request.RowInfo);
            using (RequestCallContext callContext = new RequestCallContext(req))
            {
                MethodInfoData methodData = metadata.GetOperationMethodInfo(ctx.Request.DbSetName, MethodType.Refresh);
                object instance = serviceHelper.GetMethodOwner(dbSetInfo.dbSetName, methodData);
                object invokeRes = methodData.MethodInfo.Invoke(instance, new object[] { ctx.Request });
                object dbEntity = await PropHelper.GetMethodResult(invokeRes);

                if (dbEntity != null)
                {
                    serviceHelper.UpdateRowInfoFromEntity(dbEntity, ctx.Request.RowInfo);
                }
                else
                {
                    throw new InvalidOperationException($"Refresh Operation for {ctx.Request.DbSetName} did not return a result");
                }
            }

            await _next(ctx);
        }
    }
}
