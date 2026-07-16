namespace RIAPP.DataService.Core.Types
{

    public class ErrorInfo
    {
        public ErrorInfo() :
            this(null, null)
        {
        }

        public ErrorInfo(string message, string name)
        {
            this.Message = message;
            this.Name = name;
        }


        public string Message { get; set; }


        public string Name { get; set; }
    }
}