using System;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    public static class DbSetInfoEx
    {
        /// <summary>
        /// Returns columns (field names) including nested columns
        /// </summary>
        /// <param name="dbSetInfo"></param>
        /// <returns></returns>
        public static Column[] GetColumns(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo.GetInResultFields().Select(fi =>
                            new Column
                            {
                                Name = fi.fieldName,
                                Nested = fi.fieldType == FieldType.Object ? fi.GetNames() : null
                            }).ToArray();
        }

        /// <summary>
        /// Returns true if tracking changes (diffgram) is enabled 
        /// </summary>
        /// <param name="dbSetInfo"></param>
        /// <returns></returns>
        public static bool GetIsTrackChanges(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo._isTrackChanges;
        }

        public static void SetIsTrackChanges(this DbSetInfo dbSetInfo, bool value)
        {
            dbSetInfo._isTrackChanges = value;
        }

        public static Type GetEntityType(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo._EntityType;
        }

        public static void SetEntityType(this DbSetInfo dbSetInfo, Type value)
        {
            dbSetInfo._EntityType = value;
        }

        public static Type GetHandlerType(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo._HandlerType;
        }

        public static void SetHandlerType(this DbSetInfo dbSetInfo, Type value)
        {
            dbSetInfo._HandlerType = value;
        }

        public static Type GetValidatorType(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo._ValidatorType;
        }

        public static void SetValidatorType(this DbSetInfo dbSetInfo, Type value)
        {
            dbSetInfo._ValidatorType = value;
        }
    }
}