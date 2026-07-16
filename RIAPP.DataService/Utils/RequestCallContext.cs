using RIAPP.DataService.Core;

namespace RIAPP.DataService.Utils
{
    public sealed class RequestCallContext(RequestContext context) : CallContext<RequestContext>(context)
    {
    }
}
