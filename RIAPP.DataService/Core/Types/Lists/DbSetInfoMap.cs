using RIAPP.DataService.Core.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    public class DbSetInfoMap : Dictionary<string, DbSetInfo>
    {
        private readonly Lazy<ILookup<Type, DbSetInfo>> _dbSetsByEntityType;

        public DbSetInfoMap(IDictionary<string, DbSetRec> dbSetRecMap) :
            base(dbSetRecMap.Select(v => new KeyValuePair<string, DbSetInfo>(v.Key, new DbSetInfo(v.Value.dbSetInfo, v.Value.fieldList))))
        {
            _dbSetsByEntityType = new Lazy<ILookup<Type, DbSetInfo>>(() =>
            {
                return this.Values.ToLookup(v => v.GetEntityType());
            }, true);
        }

        public DbSetInfoMap(IEnumerable<DbSetRec> dbSetRecs) :
            base(dbSetRecs.Select(v => new KeyValuePair<string, DbSetInfo>(v.dbSetInfo.dbSetName, new DbSetInfo(v.dbSetInfo, v.fieldList))))
        {
            _dbSetsByEntityType = new Lazy<ILookup<Type, DbSetInfo>>(() =>
            {
                return this.Values.ToLookup(v => v.GetEntityType());
            }, true);
        }

        public ILookup<Type, DbSetInfo> DbSetsByEntityType => _dbSetsByEntityType.Value;
    }
}
