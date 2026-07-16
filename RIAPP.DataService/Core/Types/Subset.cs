using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class Subset
    {
        /// <summary>
        /// field names
        /// </summary>
        public IEnumerable<Column> Columns { get; set; }

        public IEnumerable<Row> Rows { get; set; }

        public string DbSetName { get; set; }
    }
}