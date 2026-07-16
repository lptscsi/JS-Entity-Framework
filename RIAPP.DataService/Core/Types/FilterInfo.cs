using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RIAPP.DataService.Core.Types
{

    public class FilterItem
    {
        public FilterItem()
        {
            FieldName = string.Empty;
            Values = [];
            Kind = FilterType.Equals;
        }


        public string FieldName { get; set; }


        public List<string> Values { get; set; }


        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FilterType Kind { get; set; }
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