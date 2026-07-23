namespace RIAPP.DataService.Core.Types
{

    public class ValueChange
    {
        public ValueChange()
        {
            val = null;
            orig = null;
            flags = (int)ValueFlags.None;
            fieldName = string.Empty;
        }

        public string val { get; set; }

        public string orig { get; set; }

        public string fieldName { get; set; }

        /// <summary>
        /// Flags has the type <see cref="ValueFlags"/> 
        /// </summary>
        public int flags { get; set; }

        /// <summary>
        /// Nested values used for object field
        /// </summary>
        public ValuesList nested { get; set; }
    }
}