namespace RIAPP.DataService.Core.Types
{

    public class RefreshRequest : IUseCaseRequest<RefreshResponse>
    {
        public string DbSetName { get; set; }

        public RowInfo RowInfo { get; set; }

        internal DbSetInfo _dbSetInfo { get; set; }
    }
}