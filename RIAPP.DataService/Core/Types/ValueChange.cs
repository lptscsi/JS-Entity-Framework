namespace RIAPP.DataService.Core.Types
{

    public class ValueChange
    {
        public ValueChange()
        {
            Val = null;
            Orig = null;
            Flags = ValueFlags.None;
            FieldName = string.Empty;
        }

        public string Val { get; set; }

        public string Orig { get; set; }

        public string FieldName { get; set; }

        public ValueFlags Flags { get; set; }

        /// <summary>
        /// Nested values used for object field
        /// </summary>
        public ValuesList Nested { get; set; }
    }
}