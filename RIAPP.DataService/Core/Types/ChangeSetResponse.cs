using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class ChangeSetResponse(ChangeSetRequest request)
    {
        public DbSetList dbSets { get; set; } = request.dbSets;

        public ErrorInfo error { get; set; }

        public IEnumerable<Subset> subsets { get; set; }
    }
}