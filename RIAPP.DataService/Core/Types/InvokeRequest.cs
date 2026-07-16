namespace RIAPP.DataService.Core.Types
{

    public class InvokeRequest : IUseCaseRequest<InvokeResponse>
    {

        public string MethodName { get; set; }


        public MethodParameters ParamInfo { get; set; } = new MethodParameters();
    }
}