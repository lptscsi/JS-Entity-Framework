namespace RIAPP.DataService.Core.Types
{

    public class ValueChange
    {
        public ValueChange()
        {
            Val = null;
            Orig = null;
            Flags = (int)ValueFlags.None;
            FieldName = string.Empty;
        }

        public string Val { get; set; }

        public string Orig { get; set; }

        public string FieldName { get; set; }

        /// <summary>
        /// Flags has the type <see cref="ValueFlags"/> 
        /// </summary>
        public int Flags { get; set; }

        /// <summary>
        /// Nested values used for object field
        /// </summary>
        public ValuesList Nested { get; set; }
    }
}