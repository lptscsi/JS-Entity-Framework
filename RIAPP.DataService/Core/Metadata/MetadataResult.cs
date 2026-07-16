using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Utils;

namespace RIAPP.DataService.Core.Metadata
{

    /// <summary>
    /// Metadata DTO to send to the client side
    /// serializable to JSON
    /// </summary>
    public class MetadataResult
    {
        public MetadataResult()
        {
            serverTimezone = DateTimeHelper.GetTimezoneOffset();
        }


        public DbSetInfoList dbSets { get; set; } = new DbSetInfoList();


        public AssocList associations { get; set; } = new AssocList();


        public MethodsList methods { get; set; } = new MethodsList();


        public int serverTimezone { get; set; }
    }
}