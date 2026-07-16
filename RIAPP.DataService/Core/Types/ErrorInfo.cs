namespace RIAPP.DataService.Core.Types
{

    public class ErrorInfo(string message, string name)
    {
        public ErrorInfo() :
            this(null, null)
        {
        }

        public string Message { get; set; } = message;


        public string Name { get; set; } = name;
    }
}