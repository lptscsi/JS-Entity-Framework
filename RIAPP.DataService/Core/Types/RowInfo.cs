namespace RIAPP.DataService.Core.Types
{

    public class RowInfo
    {
        public RowInfo()
        {
            ChangeType = ChangeType.None;
            Values = new ValuesList();
            ServerKey = string.Empty;
            _dbSetInfo = null;
            _changeState = null;
        }


        public ValuesList Values { get; set; }


        public ChangeType ChangeType { get; set; }

        /// <summary>
        ///     Unique server row id in DbSet - primary key values concantenated by ;
        /// </summary>

        public string ServerKey { get; set; }

        /// <summary>
        ///     When row change type is added row has empty serverKey
        ///     client assigns unique row id to the added row, so after executing insert operation on server
        ///     the client could find the row in its rows store.
        /// </summary>

        public string ClientKey { get; set; }


        public string Error { get; set; }

        public ValidationErrorInfo[] Invalid { get; set; }

        internal DbSetInfo _dbSetInfo { get; set; }

        internal EntityChangeState _changeState { get; set; }
    }
}