using Framework.LinqSupport.LinqSupport;
using System;
using System.Collections.Generic;

namespace Framework.Datatable.RequestParser
{
    /// <summary>
    /// Specify how to convert string value to appropriate value for property or field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public abstract class ValueConverterAttribute : Attribute
    {
        /// <summary>
        /// The function used to convert string value from DataTables to appropriate value for property or field
        /// </summary>
        public abstract void Parse(string inValue, FilterHelper.ColumnFilterInfo info);
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DefaultValueConverterAttribute : ValueConverterAttribute
    {
        private readonly Type toType;

        public DefaultValueConverterAttribute(Type toType)
        {
            this.toType = toType;
        }

        public override void Parse(string inValue, FilterHelper.ColumnFilterInfo info)
        {
            try
            {
                if (toType == typeof(string))
                {
                    info.ContainValue = new List<string>() { inValue.Trim() };
                }
                else
                {
                    var outValue = Convert.ChangeType(inValue, toType);
                    info.EqualValue = new List<object>() { outValue };
                }
            }
            catch (Exception)
            {
            }
        }
    }
}