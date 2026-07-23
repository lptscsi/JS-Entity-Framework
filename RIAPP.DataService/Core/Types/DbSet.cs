namespace RIAPP.DataService.Core.Types
{

    public class DbSet
    {
        public DbSet()
        {
            rows = [];
        }


        public string dbSetName { get; set; }


        public RowsList rows { get; set; }
    }
}
