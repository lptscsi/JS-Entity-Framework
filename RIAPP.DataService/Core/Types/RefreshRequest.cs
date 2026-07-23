namespace RIAPP.DataService.Core.Types
{

    public class RefreshRequest : IUseCaseRequest<RefreshResponse>
    {
        public string dSetName { get; set; }

        public RowInfo rowInfo { get; set; }

        internal DbSetInfo _dbSetInfo { get; set; }
    }
}