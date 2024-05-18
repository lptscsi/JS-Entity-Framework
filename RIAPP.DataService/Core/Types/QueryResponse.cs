using System.Collections.Generic;

namespace RIAPP.DataService.Core.Types
{

    public class QueryResponse
    {
        /// <summary>
        /// Field names returned in the rows
        /// </summary>
        public IEnumerable<Column> Columns { get; set; }


        public IEnumerable<Row> Rows { get; set; }


        public int? PageIndex { get; set; }


        public int? PageCount { get; set; }


        public string DbSetName { get; set; }

        /// <summary>
        /// Client can ask to return rows totalcount (in paging scenarios)
        /// </summary>
        public int? TotalCount { get; set; }


        public object ExtraInfo { get; set; }

        /// <summary>
        ///  Client must first check this field
        ///  if all ok, then error is empty
        ///  otherwise it contains error message
        /// </summary>
        public ErrorInfo Error { get; set; }


        /// <summary>
        /// related child entities (from navigation properties) included in the main result
        /// </summary>
        public IEnumerable<Subset> Subsets { get; set; }
    }
}