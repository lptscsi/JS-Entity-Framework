using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class ChangeSetResponse(ChangeSetRequest request)
    {
        public DbSetList DbSets { get; set; } = request.DbSets;


        public ErrorInfo Error { get; set; }


        public IEnumerable<Subset> Subsets { get; set; }
    }
}