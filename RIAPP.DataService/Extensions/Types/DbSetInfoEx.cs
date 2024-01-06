using RIAPP.DataService.Core.Exceptions;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RIAPP.DataService.Core.Types
{
    public static class DbSetInfoEx
    {
        public static FieldName[] GetNames(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo.GetInResultFields().Select(fi =>
                            new FieldName
                            {
                                n = fi.fieldName,
                                p = fi.fieldType == FieldType.Object ? fi.GetNames() : null
                            }).ToArray();
        }

        public static Dictionary<string, Field> GetFieldByNames(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo._fieldsByNames;
        }


        private static void SetOrdinal(Field[] fieldInfos)
        {
            int cnt = fieldInfos.Length;
            for (int i = 0; i < cnt; ++i)
            {
                fieldInfos[i].SetOrdinal(i);
                if (fieldInfos[i].fieldType == FieldType.Object)
                {
                    SetOrdinal(fieldInfos[i].nested.ToArray());
                }
            }
        }

        public static void Initialize(this DbSetInfo dbSetInfo, IDataHelper dataHelper)
        {
            dbSetInfo._fieldsByNames = new Dictionary<string, Field>();
            Field[] fieldInfos = dbSetInfo.fieldInfos.ToArray();
            int cnt = fieldInfos.Length;

            for (int i = 0; i < cnt; ++i)
            {
                dataHelper.ForEachFieldInfo("", fieldInfos[i], (fullName, fieldInfo) =>
                {
                    fieldInfo.SetFullName(fullName);
                    dbSetInfo._fieldsByNames.Add(fullName, fieldInfo);
                });
            }
            SetOrdinal(fieldInfos);
            Field[] pkFields = dbSetInfo.GetPKFields();
            if (pkFields.Length < 1)
            {
                throw new DomainServiceException(string.Format(ErrorStrings.ERR_DBSET_HAS_NO_PK, dbSetInfo.dbSetName));
            }
            Dictionary<string, Field> fbn = dbSetInfo.GetFieldByNames();
        }

        public static FieldsList GetFieldInfos(this DbSetInfo dbSetInfo)
        {
            return dbSetInfo._fieldInfos;
        }

        public static void SetFieldInfos(this DbSetInfo dbSetInfo, FieldsList value)
        {
            dbSetInfo._fieldInfos = value;
        }

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