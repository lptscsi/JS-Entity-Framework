namespace RIAPP.DataService.Core.Types
{
    /// <summary>
    /// Column metadata for the result of query
    /// </summary>
    public class Column
    {
        /// <summary>
        /// Field name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// For object field it contains property names (nested fields)
        /// otherwise it is null
        /// </summary>
        public Column[] Nested { get; set; }
    }
}