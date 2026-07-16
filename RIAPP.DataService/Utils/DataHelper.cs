using RIAPP.DataService.Core;
using RIAPP.DataService.Core.Metadata;
using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RIAPP.DataService.Utils
{
    public class DataHelper<TService> : IDataHelper<TService>
        where TService : BaseDomainService
    {
        private readonly IValueConverter<TService> _valueConverter;
        private readonly ISerializer _serializer;

        public DataHelper(ISerializer serializer, IValueConverter<TService> valueConverter)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer), ErrorStrings.ERR_NO_SERIALIZER);
            _valueConverter = valueConverter ?? throw new ArgumentNullException(nameof(valueConverter));
        }

        protected T Deserialize<T>(string val)
        {
            return (T)_serializer.DeSerialize(val, typeof(T));
        }

        protected object[] SerializeObjectField(object fieldOwner, Field objectFieldInfo)
        {
            Field[] fieldInfos = objectFieldInfo.GetNestedInResultFields();
            object[] res = new object[fieldInfos.Length];

            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                res[i] = SerializeField(fieldOwner, fieldInfos[i]);
            }

            return res;
        }

        protected object[] DeSerializeObjectField(Type entityType, Field objectFieldInfo, object[] values)
        {
            Field[] fieldInfos = objectFieldInfo.GetNestedInResultFields();
            object[] res = new object[fieldInfos.Length];

            for (int i = 0; i < fieldInfos.Length; ++i)
            {
                res[i] = DeserializeField(entityType, fieldInfos[i], values[i]);
            }

            return res;
        }

        public object SetFieldValue(object entity, string fullName, Field fieldInfo, string value)
        {
            return PropHelper.SetFieldValue(entity, fullName, fieldInfo, value, _valueConverter);
        }

        public object SerializeField(object fieldOwner, Field fieldInfo)
        {
            return SerializeField(fieldOwner, fieldInfo, false);
        }

        /// <summary>
        /// extracts field value from entity, and converts value to a serialized form
        /// </summary>
        protected virtual object SerializeField(object fieldOwner, Field fieldInfo, bool optional)
        {
            object propValue = PropHelper.GetValue((dynamic)fieldOwner, fieldInfo.fieldName, !optional);
            Type propType = propValue == null ? typeof(string) : propValue.GetType();

            return (fieldInfo.fieldType == FieldType.Object) ?
                (object)SerializeObjectField(propValue, fieldInfo) :
                _valueConverter.SerializeField(propType, fieldInfo, propValue);
        }

        public string SerializeField(object fieldOwner, string fullName, Field fieldInfo)
        {
            string[] parts = fullName.Split('.');

            if (parts.Length == 1)
            {
                object fieldValue = PropHelper.GetValue(fieldOwner, fieldInfo.fieldName, true);
                Type propType = fieldValue == null ? typeof(string) : fieldValue.GetType();
                return _valueConverter.SerializeField(propType, fieldInfo, fieldValue);
            }

            for (int i = 0; i < parts.Length - 1; i += 1)
            {
                fieldOwner = PropHelper.GetValue(fieldOwner, parts[i], true);
            }

            return SerializeField(fieldOwner, parts[parts.Length - 1], fieldInfo);
        }

        public object DeserializeField(Type entityType, Field fieldInfo, object value)
        {
            bool isDictionary = PropHelper.IsExpando(entityType);
            if (!isDictionary)
            {
                PropertyInfo propInfo = entityType.GetProperty(fieldInfo.fieldName);

                if (propInfo == null)
                {
                    throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_MISSING, entityType.Name, fieldInfo.fieldName));
                }

                if (fieldInfo.fieldType == FieldType.Object)
                {
                    return DeSerializeObjectField(propInfo.PropertyType, fieldInfo, (object[])value);
                }
                else
                {
                    return _valueConverter.DeserializeField(propInfo.PropertyType, fieldInfo, (string)value);
                }
            }
            else
            {
                if (fieldInfo.fieldType == FieldType.Object)
                {
                    return DeSerializeObjectField(typeof(Expando), fieldInfo, (object[])value);
                }
                else
                {
                    return _valueConverter.DeserializeField(fieldInfo, (string)value);
                }
            }

        }

        private IEnumerable ToEnumerable(Type elementType, ParamMetadata pinfo, string[] arr)
        {
            foreach (string v in arr)
            {
                yield return ParseParameter(elementType, pinfo, false, v);
            }
        }

        private object ParseArray(Type paramType, ParamMetadata pinfo, string val)
        {
            string[] arr = Deserialize<string[]>(val);

            if (arr == null)
            {
                return null;
            }

            Type elementType = paramType.GetElementType();

            IEnumerable data = ToEnumerable(elementType, pinfo, arr);

            return data.ToArray(elementType);
        }

        public object ParseParameter(Type paramType, ParamMetadata pinfo, bool isArray, string val)
        {
            return (isArray && val != null) ? ParseArray(paramType, pinfo, val) : _valueConverter.DeserializeValue(paramType, pinfo.DataType, pinfo.DateConversion, val);
        }

        public Field GetFieldInfo(DbSetInfo dbSetInfo, string fullName)
        {
            IReadOnlyDictionary<string, Field> fieldsByName = dbSetInfo.fieldInfos.GetFieldByNames();
            return fieldsByName[fullName];
        }

        public void ForEachFieldInfo(string path, Field rootField, Action<string, Field> callBack)
        {
            if (rootField.fieldType == FieldType.Object)
            {
                callBack(path + rootField.fieldName, rootField);
                rootField.nested.ForEach(
                    fieldInfo => { ForEachFieldInfo(path + rootField.fieldName + ".", fieldInfo, callBack); });
            }
            else
            {
                callBack(path + rootField.fieldName, rootField);
            }
        }
    }
}