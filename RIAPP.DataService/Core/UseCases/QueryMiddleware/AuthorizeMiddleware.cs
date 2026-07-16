using Pipeline;
using RIAPP.DataService.Core.Metadata;
using System.Threading.Tasks;

namespace RIAPP.DataService.Core.UseCases.QueryMiddleware
{
    public class AuthorizeMiddleware<TService>(RequestDelegate<QueryContext<TService>> next, QueryMiddlewareOptions<TService> options)
         where TService : BaseDomainService
    {
        private readonly RequestDelegate<QueryContext<TService>> _next = next;

        public async Task Invoke(QueryContext<TService> ctx)
        {
            Security.IAuthorizer<TService> authorizer = ctx.ServiceContainer.GetAuthorizer();
            RunTimeMetadata metadata = ctx.Service.GetMetadata();

            MethodDescription method = metadata.GetQueryMethod(ctx.Request.DbSetName, ctx.Request.QueryName);
            await authorizer.CheckUserRightsToExecute(method.GetMethodData());

            await _next(ctx);
        }
    }
}
