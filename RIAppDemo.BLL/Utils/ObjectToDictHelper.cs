using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RIAppDemo.BLL.Utils
{
    public static class ObjectToDictionaryHelper
    {
        public static T ToDictionary<T>(this object source, Func<T> dictFactory)
            where T : IDictionary<string, object>
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            var dictionary = dictFactory();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
            {
                object value = property.GetValue(source);
                dictionary.Add(property.Name, value);
            }
            return dictionary;
        }
    }
}
