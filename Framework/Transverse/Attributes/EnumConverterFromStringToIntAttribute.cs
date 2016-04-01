using Framework.Datatable.RequestParser;
using Framework.LinqSupport.LinqSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Transverse.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumConverterFromStringToIntAttribute : ValueConverterAttribute
    {
        private readonly Type _enumType;

        public EnumConverterFromStringToIntAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        public override void Parse(string inValue, FilterHelper.ColumnFilterInfo info)
        {
            if (!string.IsNullOrEmpty(inValue))
            {
                inValue = inValue.ToLower().Trim();

                if (_enumType != null)
                {
                    var equalValues = new List<object>();
                    foreach (var field in _enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        var displayAttribute = field.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                        var display = displayAttribute == null ? field.Name : displayAttribute.Name;
                        if (display != null)
                        {
                            if (display.ToLower().Contains(inValue))
                            {
                                field.GetValue(_enumType);
                                equalValues.Add((int)Enum.Parse(_enumType, field.Name));
                            }
                        }
                    }

                    info.EqualValue = equalValues;
                }
            }
        }
    }
}