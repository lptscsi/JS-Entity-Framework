using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class FilterItem
    {
        public FilterItem()
        {
            FieldName = string.Empty;
            Values = new List<string>();
            Kind = FilterType.Equals;
        }


        public string FieldName { get; set; }


        public List<string> Values { get; set; }


        public FilterType Kind { get; set; }
    }


    public class FilterInfo
    {
        public FilterInfo()
        {
            FilterItems = new List<FilterItem>();
        }


        public List<FilterItem> FilterItems { get; set; }
    }
}