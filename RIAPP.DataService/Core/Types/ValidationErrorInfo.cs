namespace RIAPP.DataService.Core.Types
{

    public class ValidationErrorInfo
    {
        public ValidationErrorInfo()
        {
        }

        public ValidationErrorInfo(string message)
        {
            this.Message = message;
        }

        public ValidationErrorInfo(string fieldName, string message)
        {
            this.FieldName = fieldName;
            this.Message = message;
        }


        public string FieldName { get; set; }


        public string Message { get; set; }
    }
}