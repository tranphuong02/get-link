using Framework.DataAccess.Contracts.Interfaces;
using Framework.Utility;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;

namespace Framework.DataAccess.EntityFramework
{
    /// <summary>
    /// Extension for entity of Entity framework
    /// </summary>
    public static class EntityExtensions
    {
        /// <summary>
        /// Separator character
        /// </summary>
        public static char IdSeparator = '.';

        /// <summary>
        /// Id property name
        /// </summary>
        public static string IdPropertyName = "Id";

        /// <summary>
        /// parentId property name
        /// </summary>
        public static string ParentIdPropertyName = "ParentId";

        /// <summary>
        /// Ancestors property name
        /// </summary>
        public static string HierarchyPropertyName = "Ancestors";

        /// <summary>
        /// Default digit format: D5
        /// </summary>
        public static string DefaultDigitFormat = "D5";

        #region Get table name

        /// <summary>
        ///     Get table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this IBaseDbContext context) where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

            return objectContext.GetTableName<T>();
        }

        /// <summary>
        ///     Get table name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"><paramref name="match" /> is null.</exception>
        /// <exception cref="RegexMatchTimeoutException">A time-out occurred. For more information about time-outs, see the Remarks section.</exception>
        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            string sql = context.CreateObjectSet<T>().ToTraceString();
            var regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }

        #endregion Get table name

        /// <summary>
        ///     Get Id property of <paramref name="entity"/>
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="entity">the entity</param>
        /// <returns></returns>
        public static int GetId<T>(this T entity)
        {
            return (int)entity.GetPropertyValue(IdPropertyName);
        }

        /// <summary>
        ///     Get Parent Id property of <paramref name="entity"/>
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="entity">the entity</param>
        /// <returns></returns>
        public static int? GetParentId<T>(this T entity)
        {
            return (int?)entity.GetPropertyValue(ParentIdPropertyName);
        }

        /// <summary>
        ///     Get Hierarchy property of <paramref name="entity"/>
        /// </summary>
        /// <typeparam name="T">the entity type</typeparam>
        /// <param name="entity">the entity</param>
        /// <returns></returns>
        public static string GetHierarchy<T>(this T entity)
        {
            return (string)entity.GetPropertyValue(HierarchyPropertyName);
        }

        /// <summary>
        ///     Get hierarchy <paramref name="item"/> of entity with id
        /// </summary>
        /// <param name="item">the item id</param>
        /// <returns></returns>
        public static string GetHierarchyValueForRoot(this int item)
        {
            return string.Concat(IdSeparator, item.ToString(DefaultDigitFormat), IdSeparator);
        }
    }
}