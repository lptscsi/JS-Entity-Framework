﻿using RIAPP.DataService.Core;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using System;

namespace RIAPP.DataService.Utils
{
    public interface IDataHelper
    {
        object SerializeField(object entity, Field fieldInfo);
        string SerializeField(object fieldOwner, string fullName, Field fieldInfo);
        object DeserializeField(Type entityType, Field fieldInfo, object value);
        object SetFieldValue(object entity, string fullName, Field fieldInfo, string value);
        object ParseParameter(Type paramType, ParamMetadata pinfo, bool isArray, string val);
        Field GetFieldInfo(DbSetInfo dbSetInfo, string fullName);
        void ForEachFieldInfo(string path, Field rootField, Action<string, Field> callBack);
    }

    public interface IDataHelper<TService> : IDataHelper
        where TService : BaseDomainService
    {
    }
}