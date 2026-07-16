using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class ChangeSetResponse
    {
        public ChangeSetResponse(ChangeSetRequest request)
        {
            DbSets = request.DbSets;
        }


        public DbSetList DbSets { get; set; }


        public ErrorInfo Error { get; set; }


        public IEnumerable<Subset> Subsets { get; set; }
    }
}