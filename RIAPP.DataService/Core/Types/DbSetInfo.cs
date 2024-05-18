using System;
using System.ComponentModel;

namespace RIAPP.DataService.Core.Types
{
    public class DbSetInfo
    {
        #region Fields

        private FieldsList _fields = new FieldsList();

        #endregion

        public DbSetInfo(string dbSetName)
        {
            this.dbSetName = dbSetName;
            enablePaging = true;
            pageSize = 100;
            _isTrackChanges = false;
        }

        public DbSetInfo(string dbSetName, FieldsList fields)
            : this(dbSetName)
        {
            _fields = fields;
        }

        public DbSetInfo(DbSetInfo other)
        {
            dbSetName = other.dbSetName;
            _fields = other._fields;
            enablePaging = other.enablePaging;
            pageSize = other.pageSize;
            _isTrackChanges = other._isTrackChanges;
            _EntityType = other._EntityType;
            _HandlerType = other._HandlerType;
            _ValidatorType = other._ValidatorType;
        }

        public DbSetInfo(DbSetInfo other, FieldsList fields)
            : this(other)
        {
            _fields = fields;
        }

        public IFieldsList fieldInfos => _fields;

        public bool enablePaging { get; set; }

        public int pageSize { get; set; }


        public string dbSetName { get; set; }

        #region Server Side Properties

        /// <summary>
        /// Fields which are included into the final result
        /// </summary>
        /// <returns></returns>
        public Field[] GetInResultFields()
        {
            return _fields.GetInResultFields();
        }

        /// <summary>
        /// Fields for the Primary Key
        /// </summary>
        /// <returns></returns>
        public Field[] GetPKFields()
        {
            return _fields.GetPKFields();
        }

        /// <summary>
        /// Field which is used for optimistic locks
        /// </summary>
        /// <returns></returns>
        public Field GetTimeStampField()
        {
            return _fields.GetTimeStampField();
        }

        internal Type _EntityType { get; set; }

        internal Type _HandlerType { get; set; }

        internal Type _ValidatorType { get; set; }

        [DefaultValue(false)]
        internal bool _isTrackChanges { get; set; }

        #endregion

    }
}