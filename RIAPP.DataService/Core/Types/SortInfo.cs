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
            fieldName = string.Empty;
            sortOrder = SortOrder.ASC;
        }

        /// <summary>
        /// Имя поля
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// Направление сортировки
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SortOrder sortOrder { get; set; }
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