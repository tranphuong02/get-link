using Framework.LinqSupport.LinqSupport;
using System;

namespace Framework.Datatable.RequestParser
{
    /// <summary>
    ///
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class DateValueConverterAttribute : ValueConverterAttribute
    {
        public override void Parse(string inValue, FilterHelper.ColumnFilterInfo info)
        {
            DateTime value;
            if (DateTime.TryParse(inValue, out value))
            {
                info.GreaterThanOrEqualValue = value.Date;
                info.LessThanValue = value.Date.AddDays(1);
            }
        }
    }
}