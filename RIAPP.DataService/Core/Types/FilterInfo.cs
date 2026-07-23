using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RIAPP.DataService.Core.Types
{

    public class FilterItem
    {
        public FilterItem()
        {
            fieldName = string.Empty;
            values = [];
            kind = FilterType.Equals;
        }

        public string fieldName { get; set; }

        public List<string> values { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FilterType kind { get; set; }
    }


    public class FilterInfo
    {
        public FilterInfo()
        {
            FilterItems = [];
        }


        public List<FilterItem> FilterItems { get; set; }
    }
}