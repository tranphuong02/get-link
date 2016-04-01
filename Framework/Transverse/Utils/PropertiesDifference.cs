using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Transverse.Utils
{
    public static class PropertiesDifference
    {
        /// <summary>
        /// Get difference property value of 2 object. Only handle: primative and class from Model.Business
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns></returns>
        public static List<string> GetDiffStringList(object object1, object object2, string[] excludes = null)
        {
            if (object1 == null || object2 == null)
            {
                return new List<string>();
            }
            Type type = object1.GetType();
            List<string> diffs = new List<string>();
            foreach (var oProperty in type.GetProperties())
            {
                object oldValueObj = oProperty.GetValue(object1, null);
                object newValueObj = oProperty.GetValue(object2, null);
                Type fieldType;

                if (oldValueObj == null && newValueObj == null)
                {
                    continue;
                }
                if (oldValueObj == null && newValueObj != null)
                {
                    fieldType = newValueObj.GetType();
                }
                else
                {
                    if (excludes != null && excludes.Contains(oProperty.Name)) continue;
                    fieldType = oldValueObj.GetType();
                }

                if (fieldType.IsPrimitive || fieldType == typeof(Decimal) || fieldType == typeof(DateTime))
                {
                    //compare primative
                    string oldValue = oldValueObj?.ToString() ?? "null";
                    string newValue = newValueObj?.ToString() ?? "null";
                    if (oldValue != newValue)
                    {
                        diffs.Add($"Change {oProperty.Name} from '{oldValue}' to '{newValue}' ");
                    }
                }
                else if (fieldType.Namespace != null && fieldType.Namespace.Contains("Models.Business"))
                {
                    diffs.AddRange(GetDiffStringList(oldValueObj, newValueObj, excludes));
                }
            }

            return diffs;
        }

        public static string GetDiffString(string propertyName, object oldValue, object newValue)
        {
            return $"Change {propertyName} from '{oldValue}' to '{newValue}' ";
        }

        /// <summary>
        /// Perform a deep Copy of the object, using Json as a serialisation method.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T CloneJson<T>(this T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            // initialize inner objects individually
            // for example in default constructor some list property initialized with some values,
            // but in 'source' these items are cleaned -
            // without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}