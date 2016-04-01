using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Framework.LinqSupport.LinqSupport
{
    public static class FilterHelper
    {
        public class ColumnFilterInfo
        {
            public string PropertyNameOrExpression { get; set; }
            public IEnumerable<object> EqualValue { get; set; }
            public IEnumerable<object> NotEqualValue { get; set; }
            public object LessThanValue { get; set; }
            public object LessThanOrEqualValue { get; set; }
            public object GreaterThanValue { get; set; }
            public object GreaterThanOrEqualValue { get; set; }
            public IEnumerable<string> ContainValue { get; set; }

            public Type GetResultType<T>()
            {
                return ExpressionHelper.GetExpressionResultType<T>(PropertyNameOrExpression);
            }
        }

        public static Expression<Func<T, bool>> BuildFilterExpression<T>(ColumnFilterInfo filterInfo)
        {
            Expression<Func<T, bool>> filterExpression = null;
            if (filterInfo.EqualValue != null)
            {
                var any = false;
                Expression<Func<T, bool>> equalExpression = item => false;
                foreach (var equalValue in filterInfo.EqualValue)
                {
                    equalExpression =
                        equalExpression.OrElse(ExpressionHelper.Equal<T>(filterInfo.PropertyNameOrExpression, equalValue));
                    any = true;
                }
                if (any)
                {
                    filterExpression = equalExpression;
                }
            }
            if (filterInfo.NotEqualValue != null)
            {
                foreach (var notEqualValue in filterInfo.NotEqualValue)
                {
                    filterExpression =
                        filterExpression.AndAlso(ExpressionHelper.NotEqual<T>(filterInfo.PropertyNameOrExpression, notEqualValue));
                }
            }
            if (filterInfo.LessThanValue != null)
            {
                filterExpression =
                    filterExpression.AndAlso(ExpressionHelper.LessThan<T>(filterInfo.PropertyNameOrExpression, filterInfo.LessThanValue));
            }
            if (filterInfo.LessThanOrEqualValue != null)
            {
                filterExpression =
                    filterExpression.AndAlso(ExpressionHelper.LessThanOrEqual<T>(filterInfo.PropertyNameOrExpression, filterInfo.LessThanOrEqualValue));
            }
            if (filterInfo.GreaterThanValue != null)
            {
                filterExpression =
                    filterExpression.AndAlso(ExpressionHelper.GreaterThan<T>(filterInfo.PropertyNameOrExpression, filterInfo.GreaterThanValue));
            }
            if (filterInfo.GreaterThanOrEqualValue != null)
            {
                filterExpression =
                    filterExpression.AndAlso(ExpressionHelper.GreaterThanOrEqual<T>(filterInfo.PropertyNameOrExpression, filterInfo.GreaterThanOrEqualValue));
            }
            if (filterInfo.ContainValue != null)
            {
                var any = false;
                Expression<Func<T, bool>> containExpression = item => false;
                foreach (var containValue in filterInfo.ContainValue)
                {
                    containExpression =
                        containExpression.OrElse(ExpressionHelper.Contains<T>(filterInfo.PropertyNameOrExpression, containValue));
                    any = true;
                }
                if (any)
                {
                    filterExpression = filterExpression.AndAlso(containExpression);
                }
            }
            return filterExpression;
        }

        public static IQueryable<T> FilteringHelper<T>(IQueryable<T> source, IEnumerable<ColumnFilterInfo> filterInfos, params Expression<Func<T, bool>>[] additionalConditions)
        {
            if (filterInfos != null)
            {
                foreach (var columnFilterInfo in filterInfos)
                {
                    var condition = BuildFilterExpression<T>(columnFilterInfo);
                    if (condition != null)
                    {
                        source = source.Where(condition);
                    }
                }
            }
            foreach (var additionalCondition in additionalConditions)
            {
                if (additionalCondition != null)
                {
                    source = source.Where(additionalCondition);
                }
            }
            return source;
        }
    }
}