using RIAPP.DataService.Core.Types;

namespace Application.Models
{
    /// <summary>
    /// The data which is loaded at the start of application
    /// includes several datasets - so to load them in one roundtrip
    /// </summary>
    public class StaticData
    {
        public QueryResponse ProductModelData { get; set; }

        public QueryResponse ProductCategoryData { get; set; }
    }
}
