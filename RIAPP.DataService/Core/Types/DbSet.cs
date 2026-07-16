namespace RIAPP.DataService.Core.Types
{

    public class DbSet
    {
        public DbSet()
        {
            Rows = [];
        }


        public string DbSetName { get; set; }


        public RowsList Rows { get; set; }
    }
}
