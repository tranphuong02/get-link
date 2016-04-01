using Framework.Datatable.RequestBinder;
using Framework.LinqSupport.LinqSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.Datatable.RequestParser
{
    internal class ViewModelPropertyInfo
    {
        public string MapUnderlyingProperty { get; set; }
        public ValueConverterAttribute ValueConverter { get; set; }
        public ConditionProviderAttribute Condition { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TEntity"></typeparam>
    public class DataTableHelper<TViewModel, TEntity> where TEntity : class, new()
    {
        // ReSharper disable once StaticFieldInGenericType
        private static readonly Dictionary<string, ViewModelPropertyInfo> ViewModelProperties = new Dictionary<string, ViewModelPropertyInfo>();

        static DataTableHelper()
        {
            var vmProperties = typeof(TViewModel).GetProperties();
            foreach (var property in vmProperties)
            {
                var underlyingProperty = property.GetCustomAttribute<MapUnderlyingPropertiesAttribute>();
                var valueConverter = property.GetCustomAttribute<ValueConverterAttribute>(true) ?? new DefaultValueConverterAttribute(property.PropertyType);
                var customCondition = property.GetCustomAttribute<ConditionProviderAttribute>(true) ?? new ConditionProviderAttribute();
                PropertyInfo prop = property;
                ViewModelProperties.Add(property.Name, new ViewModelPropertyInfo()
                {
                    MapUnderlyingProperty = underlyingProperty == null ? property.Name : underlyingProperty.Expression,
                    ValueConverter = valueConverter,
                    Condition = customCondition,
                    PropertyInfo = prop
                });
            }
        }

        private readonly IQueryable<TEntity> _source;
        private readonly Expression<Func<TEntity, TViewModel>> _selectExpression;

        /// <summary>
        ///
        /// </summary>
        /// <param name="source"></param>
        /// <param name="selectExpression"></param>
        public DataTableHelper(IQueryable<TEntity> source, Expression<Func<TEntity, TViewModel>> selectExpression)
        {
            this._source = source;
            _selectExpression = selectExpression;
        }

        public DataTablesResponse GetResponse(IDataTablesRequest param, params FilterHelper.ColumnFilterInfo[] addtitonalFilters)
        {
            var query = GetDataForResponse(param, addtitonalFilters);
            var data = query.Skip(param.Start).Take(param.Length).Select(_selectExpression).ToList();
            return new DataTablesResponse(param.Draw, data, query.Count(), query.Count());
        }

        public IQueryable<TEntity> GetDataForResponse(IDataTablesRequest param, params FilterHelper.ColumnFilterInfo[] addtitonalFilters)
        {
            Expression<Func<TEntity, bool>> genericExpression = null;
            var query = _source;
            foreach (var c in param.Columns)
            {
                ViewModelPropertyInfo vmProperty;
                if (c.Searchable && ViewModelProperties.TryGetValue(c.Data, out vmProperty))
                {
                    var col = new FilterHelper.ColumnFilterInfo()
                    {
                        PropertyNameOrExpression = vmProperty.MapUnderlyingProperty
                    };
                    if (!string.IsNullOrEmpty(c.Search.Value))
                    {
                        vmProperty.ValueConverter.Parse(c.Search.Value, col);
                    }

                    var condition = vmProperty.Condition.GetCondition<TEntity>(col);
                    if (condition != null)
                    {
                        query = query.Where(condition);
                    }

                    //Generic Search
                    if (!string.IsNullOrEmpty(param.Search.Value))
                    {
                        Expression<Func<TEntity, bool>> singleExpression = null;
                        var tmpCol = new FilterHelper.ColumnFilterInfo()
                        {
                            PropertyNameOrExpression = vmProperty.MapUnderlyingProperty
                        };
                        vmProperty.ValueConverter.Parse(param.Search.Value, tmpCol);
                        singleExpression = vmProperty.Condition.GetCondition<TEntity>(tmpCol);
                        if (singleExpression != null)
                        {
                            genericExpression = genericExpression == null
                                                ? singleExpression
                                                : genericExpression.OrElse(singleExpression);
                        }
                    }
                }
            }
            if (addtitonalFilters != null)
            {
                var defaultConditionBuilder = new ConditionProviderAttribute();
                foreach (var addtitonalFilter in addtitonalFilters)
                {
                    var condition = defaultConditionBuilder.GetCondition<TEntity>(addtitonalFilter);
                    if (condition != null)
                    {
                        query = query.Where(condition);
                    }
                }
            }

            if (genericExpression != null)
            {
                query = query.Where(genericExpression);
            }

            //Ordering
            var orderColumns = param.Columns.Where(c => c.IsOrdered).OrderBy(c => c.OrderNumber);
            IOrderedQueryable<TEntity> orderQuery = null;
            foreach (var column in orderColumns)
            {
                ViewModelPropertyInfo vmProperty;
                if (column.Searchable && ViewModelProperties.TryGetValue(column.Data, out vmProperty))
                {
                    orderQuery = column.SortDirection == Column.OrderDirection.Ascendant
                        ? (orderQuery == null
                            ? query.OrderBy(vmProperty.MapUnderlyingProperty)
                            : orderQuery.ThenBy(vmProperty.MapUnderlyingProperty))
                        : (orderQuery == null
                            ? query.OrderByDescending(vmProperty.MapUnderlyingProperty)
                            : orderQuery.ThenByDescending(vmProperty.MapUnderlyingProperty));
                }
            }
            return orderQuery ?? query; //.OrderBy("Id");
        }

        #region New Version - Search/Sort on ViewModel

        public DataTablesResponse GetDataTablesResponse(IDataTablesRequest param, params FilterHelper.ColumnFilterInfo[] addtitonalFilters)
        {
            var query = GetDataVMForResponse(param, addtitonalFilters);
            var data = GetDataToList(param, query);
            return new DataTablesResponse(param.Draw, data, query.Count(), query.Count());
        }

        public IQueryable<TViewModel> GetDataVMForResponse(IDataTablesRequest param,
           params FilterHelper.ColumnFilterInfo[] addtitonalFilters)
        {
            Expression<Func<TViewModel, bool>> genericExpression = null;
            var query = _source.Select(_selectExpression);

            foreach (var c in param.Columns)
            {
                ViewModelPropertyInfo vmProperty;
                if (c.Searchable && ViewModelProperties.TryGetValue(c.Data, out vmProperty))
                {
                    var col = new FilterHelper.ColumnFilterInfo()
                    {
                        PropertyNameOrExpression = vmProperty.MapUnderlyingProperty
                    };
                    if (!string.IsNullOrEmpty(c.Search.Value))
                    {
                        vmProperty.ValueConverter.Parse(c.Search.Value, col);
                    }

                    var condition = vmProperty.Condition.GetCondition<TViewModel>(col);
                    if (condition != null)
                    {
                        query = query.Where(condition);
                    }

                    //Generic Search
                    if (!string.IsNullOrEmpty(param.Search.Value))
                    {
                        var tmpCol = new FilterHelper.ColumnFilterInfo()
                        {
                            PropertyNameOrExpression = vmProperty.MapUnderlyingProperty
                        };
                        vmProperty.ValueConverter.Parse(param.Search.Value, tmpCol);
                        var singleExpression = vmProperty.Condition.GetCondition<TViewModel>(tmpCol);
                        if (singleExpression != null)
                        {
                            genericExpression = genericExpression == null
                                                ? singleExpression
                                                : genericExpression.OrElse(singleExpression);
                        }
                    }
                }
            }
            if (addtitonalFilters != null)
            {
                var defaultConditionBuilder = new ConditionProviderAttribute();
                foreach (var addtitonalFilter in addtitonalFilters)
                {
                    var condition = defaultConditionBuilder.GetCondition<TViewModel>(addtitonalFilter);
                    if (condition != null)
                    {
                        query = query.Where(condition);
                    }
                }
            }

            if (genericExpression != null)
            {
                query = query.Where(genericExpression);
            }

            // Ordering
            var orderColumns = param.Columns.Where(c => c.IsOrdered).OrderBy(c => c.OrderNumber);
            IOrderedQueryable<TViewModel> orderQuery = null;
            foreach (var column in orderColumns)
            {
                ViewModelPropertyInfo vmProperty;
                if (column.Searchable && ViewModelProperties.TryGetValue(column.Data, out vmProperty))
                {
                    orderQuery = column.SortDirection == Column.OrderDirection.Ascendant
                        ? (orderQuery == null
                            ? query.OrderBy(vmProperty.MapUnderlyingProperty)
                            : orderQuery.ThenBy(vmProperty.MapUnderlyingProperty))
                        : (orderQuery == null
                            ? query.OrderByDescending(vmProperty.MapUnderlyingProperty)
                            : orderQuery.ThenByDescending(vmProperty.MapUnderlyingProperty));
                }
            }
            if (orderQuery == null)
            {
                var firstOrDefault = typeof(TViewModel)
                    .GetProperties().FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var propName = firstOrDefault.Name;
                    orderQuery = query.OrderBy("" + propName + "");
                }
            }
            return orderQuery;
        }

        public IList<T> GetDataToList<T>(IDataTablesRequest param, IQueryable<T> query)
        {
            return query.Skip(param.Start).Take(param.Length).ToList();
        }

        #endregion New Version - Search/Sort on ViewModel
    }
}