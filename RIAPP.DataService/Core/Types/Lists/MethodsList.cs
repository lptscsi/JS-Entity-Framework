using RIAPP.DataService.Core.Metadata;
using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{
    public class MethodsList : List<MethodDescription>
    {
        public MethodsList()
        {
        }

        public MethodsList(IEnumerable<MethodDescription> items) :
            base(items)
        {
        }
    }
}
