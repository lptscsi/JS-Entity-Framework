namespace RIAPP.DataService.Core.Types
{

    public class ChangeSetRequest : IUseCaseRequest<ChangeSetResponse>
    {
        public ChangeSetRequest()
        {
            dbSets = [];
            trackAssocs = [];
        }

        public DbSetList dbSets { get; set; }

        public TrackAssocList trackAssocs { get; set; }
    }
}