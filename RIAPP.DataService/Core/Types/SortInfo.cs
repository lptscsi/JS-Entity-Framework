using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RIAPP.DataService.Core.Types
{
    /// <summary>
    /// Критерий сортировки
    /// </summary>
    public class SortItem
    {
        public SortItem()
        {
            FieldName = string.Empty;
            SortOrder = SortOrder.ASC;
        }

        /// <summary>
        /// Имя поля
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Направление сортировки
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SortOrder SortOrder { get; set; }
    }



    public class SortInfo
    {
        public SortInfo()
        {
            SortItems = [];
        }


        public List<SortItem> SortItems { get; set; }
    }
}