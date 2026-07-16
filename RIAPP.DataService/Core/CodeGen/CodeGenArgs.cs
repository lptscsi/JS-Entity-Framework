namespace RIAPP.DataService.Core.CodeGen
{
    public class CodeGenArgs(string lang)
    {
        public string lang
        {
            get;
            private set;
        } = lang;

        public string comment
        {
            get;
            set;
        }

        public bool isDraft
        {
            get;
            set;
        }
    }
}
