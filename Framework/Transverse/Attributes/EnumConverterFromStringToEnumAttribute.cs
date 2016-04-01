using Framework.Datatable.RequestParser;
using Framework.LinqSupport.LinqSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Transverse.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class EnumConverterFromStringToEnumAttribute : ValueConverterAttribute
    {
        //private readonly Dictionary<int, string> _dictionary;
        private readonly Type _enumType;

        /*public EnumConverterFromStringToEnumAttribute(string propertyName)
        {
            var app = new ApplicationManager();
            try
            {
                _dictionary = app[propertyName] as Dictionary<int, string>;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Cannot find property type is Dictionary for property name " + propertyName);
            }
        }*/

        public EnumConverterFromStringToEnumAttribute(Type enumType)
        {
            _enumType = enumType;
        }

        public override void Parse(string inValue, FilterHelper.ColumnFilterInfo info)
        {
            if (!string.IsNullOrEmpty(inValue))
            {
                inValue = inValue.ToLower().Trim();

                /*if (_dictionary != null)
                {
                    var enumValues = _dictionary.Where(x => x.Value.ToLower().Contains(inValue)).ToList();
                    if (!enumValues.Any()) return;

                    var equalValues = enumValues.Select(value => value.Key).Cast<object>().ToList();
                    info.EqualValue = equalValues;
                }*/

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
                                equalValues.Add(Enum.Parse(_enumType, field.Name));
                            }
                        }
                    }

                    info.EqualValue = equalValues;
                }
            }
        }
    }
}