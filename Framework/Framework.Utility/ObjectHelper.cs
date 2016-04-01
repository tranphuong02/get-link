//////////////////////////////////////////////////////////////////////
// File Name    : ObjectHelper
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 6:18:18 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.Linq.Expressions;
using System.Web.Script.Serialization;

namespace Framework.Utility
{
    /// <summary>
    /// <see cref="Object"/> helper
    /// </summary>
    public partial class ObjectHelper
    {
        /// <summary>
        ///     Extract property name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">'propertyExpression' should be a member expression</exception>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException($"'{propertyExpression}' should be a member expression");
            return body.Member.Name;
        }

        /// <summary>
        ///     De-serialize <see langword="object"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The <paramref name="data" />
        /// length exceeds the value of
        /// <see cref="P:System.Web.Script.Serialization.JavaScriptSerializer.MaxJsonLength" />
        /// . -or- The recursion limit defined by
        /// <see cref="P:System.Web.Script.Serialization.JavaScriptSerializer.RecursionLimit" />
        /// was exceeded. -or- <paramref name="input" /> contains an unexpected
        /// character sequence. -or- <paramref name="data" /> is a dictionary
        /// type and a non-string key value was encountered. -or-
        /// <paramref name="data" /> includes member definitions that are not
        /// available on type <paramref name="T" />. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="data" /> is
        /// null. </exception>
        /// <exception cref="InvalidOperationException"><paramref name="data" />
        /// contains a "__type" property that indicates a custom type, but the
        /// type resolver associated with the serializer cannot find a
        /// corresponding managed type. -or- <paramref name="data" /> contains a
        /// "__type" property that indicates a custom type, but the result of
        /// deserializing the corresponding JSON string cannot be assigned to
        /// the expected target type. -or- <paramref name="data" /> contains a
        /// "__type" property that indicates either
        /// <see cref="T:System.Object" /> or a non-instantiable type (for
        /// example, an abstract types or an interface). -or- An attempt was
        /// made to convert a JSON array to an array-like managed type that is
        /// not supported for use as a JSON deserialization target. -or- It is
        /// not possible to convert <paramref name="data" /> to
        /// <paramref name="T" />. </exception>
        public static T MapperObject<T>(object data) where T : class
        {
            var serializer = new JavaScriptSerializer();
            var obj = serializer.Deserialize<T>(serializer.Serialize(data));

            return obj;
        }
    }
}