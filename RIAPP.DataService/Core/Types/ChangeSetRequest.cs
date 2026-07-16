namespace RIAPP.DataService.Core.Types
{


    public class ChangeSetRequest : IUseCaseRequest<ChangeSetResponse>
    {
        public ChangeSetRequest()
        {
            DbSets = [];
            TrackAssocs = [];
        }


        public DbSetList DbSets { get; set; }


        public TrackAssocList TrackAssocs { get; set; }
    }
}