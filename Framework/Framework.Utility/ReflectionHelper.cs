using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Framework.Utility
{
    /// <summary>
    /// Reflection helper
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        ///     Get attribute of member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        /// <exception cref="TypeLoadException">A custom attribute type cannot be loaded. </exception>
        /// <exception cref="ArgumentNullException">If <paramref name="T" /> is null.</exception>
        /// <exception cref="InvalidOperationException">This member belongs to a type that is loaded into the reflection-only context. See How to: Load Assemblies into the Reflection-Only Context.</exception>
        public static T GetAttribute<T>(this MemberInfo memberInfo) where T : class
        {
            var customAttributes = memberInfo.GetCustomAttributes(typeof(T), false);
            var attribute = customAttributes.FirstOrDefault(a => a is T) as T;
            return attribute;
        }

        /// <summary>
        ///     Attempts to set a named property of an entity to an arbitrary value. The value is set if the property is found.
        /// </summary>
        /// <typeparam name="T">An entity deriving of type EntityObject.</typeparam>
        /// <param name="entityToSet">The instance of the entity whose value will be set.</param>
        /// <param name="propertyName">The name of the property to set.</param>
        /// <param name="value">The <paramref name="value"/> of the property to set.</param>
        public static void SetProperty<T>(this T entityToSet, string propertyName, object value)
        {
            var targetProperty = entityToSet.GetType().GetProperty(propertyName);
            if (targetProperty != null)
            {
                targetProperty.SetValue(entityToSet, value, null);
            }
        }

        /// <summary>
        ///     Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<PropertyInfo> GetAllPropertiesOfType(Type type)
        {
            return type.GetProperties().ToList();
        }

        /// <summary>
        ///     Get all properties from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> GetAllPropertyNamesOfType(Type type)
        {
            var properties = type.GetProperties();
            return properties.Select(p => p.Name).ToList();
        }

        /// <summary>
        ///     Get value of property by name
        /// </summary>
        /// <param name="entity">the entity</param>
        /// <param name="propertyName">the property name</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T entity, string propertyName)
        {
            var targetProperty = entity.GetType().GetProperty(propertyName);
            if (targetProperty != null)
                return targetProperty.GetValue(entity);
            return null;
        }

        /// <summary>
        /// Returns the constructor in <paramref name="type"/> that matches the specified constructor parameter types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <param name="constructorParameters">The constructor parameter types.</param>
        /// <returns>The constructor that matches the specified parameter types.</returns>
        public static ConstructorInfo GetConstructor(this Type type, params Type[] constructorParameters)
        {
            return type.GetTypeInfo().DeclaredConstructors
                .Single(c => !c.IsStatic && ParametersMatch(c.GetParameters(), constructorParameters));
        }

        /// <summary>
        /// Returns the non-static declared methods of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="MethodInfo"/> objects.</returns>
        public static IEnumerable<MethodInfo> GetMethodsHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic);
            }

            return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic)
                    .Concat(GetMethodsHierarchical(type.GetTypeInfo().BaseType));
        }

        /// <summary>
        /// Returns the non-static method of a type or its based type.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <param name="methodName">The name of the method to seek.</param>
        /// <param name="closedParameters">The (closed) parameter type signature of the method.</param>
        /// <returns>The discovered <see cref="MethodInfo"/></returns>
        public static MethodInfo GetMethodHierarchical(this Type type, string methodName, Type[] closedParameters)
        {
            return type.GetMethodsHierarchical().Single(
                    m => m.Name.Equals(methodName) &&
                        ParametersMatch(m.GetParameters(), closedParameters));
        }

        /// <summary>
        /// Returns the declared properties of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="PropertyInfo"/> objects.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo().DeclaredProperties
                                      .Concat(GetPropertiesHierarchical(type.GetTypeInfo().BaseType));
        }

        /// <summary>
        /// Determines if the types in a parameter set ordinal matches the set of supplied types.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="closedConstructorParameterTypes"></param>
        /// <returns></returns>
        public static bool ParametersMatch(ParameterInfo[] parameters, Type[] closedConstructorParameterTypes)
        {
            Guard.ArgumentNotNull(parameters, "parameters");
            Guard.ArgumentNotNull(closedConstructorParameterTypes, "closedConstructorParameterTypes");

            if (parameters.Length != closedConstructorParameterTypes.Length)
            {
                return false;
            }

            return !parameters.Where((t, i) => !(t.ParameterType == closedConstructorParameterTypes[i])).Any();
        }

        /// <summary>
        /// Get member information
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TU"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static MemberInfo GetMemberInfo<T, TU>(Expression<Func<T, TU>> expression)
        {
            var member = expression.Body as MemberExpression;
            if (member != null)
            {
                return member.Member;
            }

            throw new ArgumentException(@"Expression is not a member access", nameof(expression));
        }
    }
}