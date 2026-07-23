namespace RIAPP.DataService.Core.Types
{

    public class ErrorInfo(string message, string name)
    {
        public ErrorInfo() :
            this(null, null)
        {
        }

        public string message { get; set; } = message;

        public string name { get; set; } = name;
    }
}