namespace RIAPP.DataService.Core.Types
{

    public class RefreshResponse
    {
        public string DbSetName { get; set; }

        public RowInfo RowInfo { get; set; }

        public ErrorInfo Error { get; set; }
    }
}