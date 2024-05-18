using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class SortItem
    {
        public SortItem()
        {
            FieldName = string.Empty;
            SortOrder = SortOrder.ASC;
        }


        public string FieldName { get; set; }


        public SortOrder SortOrder { get; set; }
    }



    public class SortInfo
    {
        public SortInfo()
        {
            SortItems = new List<SortItem>();
        }


        public List<SortItem> SortItems { get; set; }
    }
}