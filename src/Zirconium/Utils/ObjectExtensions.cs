using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Zirconium.Utils
{
    public static class ObjectExtensions
    {
        public static IDictionary<string, object> ToDictionary(this object source)
        {
            return source.ToDictionary<object>();
        }

        public static IDictionary<string, T> ToDictionary<T>(this object source)
        {
            if (source == null)
                ThrowExceptionWhenSourceArgumentIsNull();

            var dictionary = new Dictionary<string, T>();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(source))
                AddPropertyToDictionary<T>(property, source, dictionary);
            return dictionary;
        }

        private static void AddPropertyToDictionary<T>(PropertyDescriptor property, object source, Dictionary<string, T> dictionary)
        {
            object value = property.GetValue(source);
            if (value is T)
            {
                var jsonProp = property.Attributes.OfType<JsonPropertyAttribute>().FirstOrDefault();
                string propName;
                if (jsonProp == null)
                {
                    propName = property.Name;
                }
                else
                {
                    propName = jsonProp.PropertyName;
                }
                if (jsonProp.Required == Required.Default && value == null){
                    return;
                }
                dictionary.Add(propName, (T)value);
            }
        }

        public static T ToObject<T>(this IDictionary<string, object> source)
            where T : class, new()
        {
            var someObject = new T();
            var someObjectType = someObject.GetType();

            foreach (var item in source)
            {
                string propName = null;
                foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(someObject)) {
                    var jsonProp = prop.Attributes.OfType<JsonPropertyAttribute>().FirstOrDefault();
                    if (jsonProp != null) {
                        if (jsonProp.PropertyName == item.Key) {
                            propName = prop.Name;
                        }
                    }
                }
                if (propName == null) {
                    propName = item.Key;
                }
                someObjectType
                        .GetProperty(propName)
                        .SetValue(someObject, item.Value, null);
            }

            return someObject;
        }

        private static void ThrowExceptionWhenSourceArgumentIsNull()
        {
            throw new ArgumentNullException("source", "Unable to convert object to a dictionary. The source object is null.");
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
                                                                TKey key,
                                                                TValue defaultValue)
        {
            TValue value;
            return dictionary.TryGetValue(key, out value) ? value : defaultValue;
        }

        public static byte[] ToByteArray(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static string ConvertToString(this byte[] bytes)
        {
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
    }
}