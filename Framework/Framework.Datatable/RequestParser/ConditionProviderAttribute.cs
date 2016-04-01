using Framework.LinqSupport.LinqSupport;
using System;
using System.Linq.Expressions;

namespace Framework.Datatable.RequestParser
{
    /// <summary>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class ConditionProviderAttribute : Attribute
    {
        /// <summary>
        /// return Expression<Func<TEntity, bool>> to specify condition to filter
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual Expression<Func<T, bool>> GetCondition<T>(FilterHelper.ColumnFilterInfo info)
        {
            return FilterHelper.BuildFilterExpression<T>(info);
        }
    }
}