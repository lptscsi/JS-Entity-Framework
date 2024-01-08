using RIAPP.DataService.Core.Types;
using RIAPP.DataService.Resources;
using RIAPP.DataService.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace RIAPP.DataService.Utils
{
    public static class PropHelper
    {
        public static bool IsExpando(Type type)
        {
            return type.IsAssignableTo(typeof(IDictionary<string, object>));
        }
        public static object GetValue(object obj, string propertyName, bool throwErrors)
        {
           return _GetValue((dynamic)obj, propertyName, throwErrors);
        }

        public static bool SetValue(object obj, string propertyName, object value, bool throwErrors)
        {
            return _SetValue((dynamic)obj, propertyName, value, throwErrors);
        }

        public static object SetFieldValue(object entity, string fullName, Field fieldInfo, string value, IValueConverter valueConverter)
        {
            return _SetFieldValue((dynamic)entity, fullName, fieldInfo, value, valueConverter);
        }

        public static async Task<object> GetMethodResult(object invokeRes)
        {
            System.Type typeInfo = invokeRes != null ? invokeRes.GetType() : null;
            if (typeInfo != null && invokeRes is Task)
            {
                await ((Task)invokeRes);
                if (typeInfo.IsGenericType)
                {
                    return typeInfo.GetProperty("Result").GetValue(invokeRes, null);
                }
                else
                {
                    return null;
                }
            }
            return invokeRes;
        }

        private static object _GetValue(object obj, string propertyName, bool throwErrors)
        {
            string[] parts = propertyName.Split('.');
            Type enityType = obj.GetType();
            PropertyInfo pinfo = enityType.GetProperty(parts[0]);

            if (pinfo == null && throwErrors)
            {
                throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_MISSING, enityType.Name, propertyName));
            }

            if (pinfo == null)
            {
                return null;
            }

            if (parts.Length == 1)
            {
                return pinfo.GetValue(obj, null);
            }

            object pval = pinfo.GetValue(obj, null) ?? throw new Exception(string.Format(ErrorStrings.ERR_PPROPERTY_ISNULL, enityType.Name, pinfo.Name));

            return GetValue(pval, string.Join(".", parts.Skip(1)), throwErrors);
        }

        private static object _GetValue(IDictionary<string, object> obj, string propertyName, bool throwErrors)
        {
            string[] parts = propertyName.Split('.');

            bool isOk = obj.TryGetValue(propertyName, out object propValue);

            if (!isOk && throwErrors)
            {
                throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_MISSING, obj.GetType().Name, propertyName));
            }

            if (!isOk)
            {
                return null;
            }

            if (parts.Length == 1)
            {
                return propValue;
            }

            if (propValue == null)
            {
                return null;
            }

            return GetValue(propValue, string.Join(".", parts.Skip(1)), throwErrors);
        }

        private static bool _SetValue(object obj, string propertyName, object value, bool throwErrors)
        {
            string[] parts = propertyName.Split('.');
            Type enityType = obj.GetType();
            PropertyInfo pinfo = enityType.GetProperty(parts[0]);

            if (pinfo == null && throwErrors)
            {
                throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_MISSING, enityType.Name, propertyName));
            }

            if (pinfo == null)
            {
                return false;
            }

            if (parts.Length == 1)
            {
                if (!pinfo.CanWrite)
                {
                    if (throwErrors)
                    {
                        throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_READONLY, enityType.Name,
                            propertyName));
                    }

                    return false;
                }

                pinfo.SetValue(obj, value, null);
                return true;
            }

            object pval = pinfo.GetValue(obj, null) ?? throw new Exception(string.Format(ErrorStrings.ERR_PPROPERTY_ISNULL, enityType.Name, pinfo.Name));

            return SetValue(pval, string.Join(".", parts.Skip(1)), value, throwErrors);
        }

        private static bool _SetValue(IDictionary<string, object> obj, string propertyName, object value, bool throwErrors)
        {
            string[] parts = propertyName.Split('.');

            if (parts.Length == 1)
            {
                if (obj.ContainsKey(propertyName))
                {
                    obj[propertyName] = value;
                }
                else
                {
                    obj.Add(propertyName, value);
                }
    
                return true;
            }

            bool isOk = obj.TryGetValue(propertyName, out object propValue);
            if (!isOk)
            {
                propValue = new Expando();
                obj[propertyName] = propValue;
            }

            return SetValue(propValue, string.Join(".", parts.Skip(1)), value, throwErrors);
        }

        private static object _SetFieldValue(object entity, string fullName, Field fieldInfo, string value, IValueConverter valueConverter)
        {
            string[] parts = fullName.Split('.');
            Type enityType = entity.GetType();
            PropertyInfo pinfo = enityType.GetProperty(parts[0]) ?? throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_MISSING, enityType.Name,
                    fieldInfo.fieldName));

            if (parts.Length == 1)
            {
                if (!pinfo.CanWrite)
                {
                    throw new Exception(string.Format(ErrorStrings.ERR_PROPERTY_IS_READONLY, enityType.Name,
                        fieldInfo.fieldName));
                }

                Type propType = pinfo.PropertyType;
                object val = valueConverter.DeserializeField(propType, fieldInfo, value);

                if (val != null)
                {
                    SetValue(entity, parts[0], val, true);
                }
                else
                {
                    if (propType.IsNullableType())
                    {
                        SetValue(entity, parts[0], val, true);
                    }
                    else if (!propType.IsValueType)
                    {
                        SetValue(entity, parts[0], val, true);
                    }
                    else
                    {
                        throw new Exception(string.Format(ErrorStrings.ERR_FIELD_IS_NOT_NULLABLE, fieldInfo.fieldName));
                    }
                }

                return val;
            }

            object pval = pinfo.GetValue(entity, null) ?? throw new Exception(string.Format(ErrorStrings.ERR_PPROPERTY_ISNULL, enityType.Name, pinfo.Name));

            return SetFieldValue(pval, string.Join(".", parts.Skip(1)), fieldInfo, value, valueConverter);
        }

        private static object _SetFieldValue(IDictionary<string, object> entity, string fullName, Field fieldInfo, string value, IValueConverter valueConverter)
        {
            string[] parts = fullName.Split('.');

            if (parts.Length == 1)
            {
                object val = valueConverter.DeserializeField(fieldInfo, value);

                if (val != null)
                {
                    SetValue(entity, parts[0], val, true);
                }
                else
                {
                    if (fieldInfo.isNullable)
                    {
                        SetValue(entity, parts[0], val, true);
                    }
                    else
                    {
                        throw new Exception(string.Format(ErrorStrings.ERR_FIELD_IS_NOT_NULLABLE, fieldInfo.fieldName));
                    }
                }

                return val;
            }

            object pval = GetValue(entity, parts[0], false);
            if (pval == null)
            {
                pval = new Expando();
                entity.Add(parts[0], pval);
            }

            return SetFieldValue(pval, string.Join(".", parts.Skip(1)), fieldInfo, value, valueConverter);
        }
    }
}
