namespace RIAPP.DataService.Core.Types
{


    public class ChangeSetRequest : IUseCaseRequest<ChangeSetResponse>
    {
        public ChangeSetRequest()
        {
            DbSets = new DbSetList();
            TrackAssocs = new TrackAssocList();
        }


        public DbSetList DbSets { get; set; }


        public TrackAssocList TrackAssocs { get; set; }
    }
}