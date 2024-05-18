namespace RIAPP.DataService.Core.Types
{

    public class DbSetPermit
    {

        public string DbSetName { get; set; }


        public bool CanAddRow { get; set; }


        public bool CanEditRow { get; set; }


        public bool CanDeleteRow { get; set; }


        public bool CanRefreshRow { get; set; }
    }
}