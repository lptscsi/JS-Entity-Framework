using Pipeline;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core.UseCases.QueryMiddleware
{
    public class ExecuteMiddleware<TService>
         where TService : BaseDomainService
    {
        private readonly RequestDelegate<QueryContext<TService>> _next;

        public ExecuteMiddleware(RequestDelegate<QueryContext<TService>> next, QueryMiddlewareOptions<TService> options)
        {
            _next = next;
        }

        public async Task Invoke(QueryContext<TService> ctx)
        {
            DbSetInfo dbSetInfo = ctx.Request.GetDbSetInfo() ?? throw new InvalidOperationException($"Could not get the DbSet for {ctx.Request.DbSetName}");
            IDataHelper<TService> dataHelper = ctx.ServiceContainer.GetDataHelper();
            IServiceOperationsHelper<TService> serviceHelper = ctx.ServiceContainer.GetServiceHelper();
            RunTimeMetadata metadata = ctx.Service.GetMetadata();

            MethodDescription method = metadata.GetQueryMethod(ctx.Request.DbSetName, ctx.Request.QueryName);

            LinkedList<object> methParams = new LinkedList<object>();

            for (int i = 0; i < method.parameters.Count; ++i)
            {
                methParams.AddLast(ctx.Request.ParamInfo.GetValue(method.parameters[i].Name, method, dataHelper));
            }

            RequestContext req = QueryContext<TService>.CreateRequestContext(ctx.Service, ctx.Request);
            using (RequestCallContext callContext = new RequestCallContext(req))
            {
                MethodInfoData methodData = method.GetMethodData();
                object instance = serviceHelper.GetMethodOwner(dbSetInfo.dbSetName, methodData);
                object invokeRes = methodData.MethodInfo.Invoke(instance, methParams.ToArray());
                QueryResult queryResult = (QueryResult)await PropHelper.GetMethodResult(invokeRes);

                IEnumerable<object> entities = queryResult.Result;
                int? totalCount = queryResult.TotalCount;
                RowGenerator rowGenerator = new RowGenerator(dbSetInfo, entities, dataHelper);
                IEnumerable<Row> rows = rowGenerator.CreateRows();

                SubsetsGenerator subsetsGenerator = new SubsetsGenerator(metadata, dataHelper);
                SubsetList subResults = subsetsGenerator.CreateSubsets(queryResult.SubResults);

                ctx.Response.Columns = dbSetInfo.GetColumns();
                ctx.Response.TotalCount = totalCount;
                ctx.Response.Rows = rows;
                ctx.Response.Subsets = subResults;
                ctx.Response.ExtraInfo = queryResult.ExtraInfo;
            }

            await _next(ctx);
        }
    }
}
