using System;

namespace Framework.Datatable.RequestParser
{
    /// <summary>
    /// Specify how the property of View Model is composed from underlying DB Model
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class MapUnderlyingPropertiesAttribute : Attribute
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="expression">Specify how the property of View Model is composed from underlying DB Model. For example: <code>FirstName + \" \" + LastName</code></param>
        public MapUnderlyingPropertiesAttribute(string expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Specify how the property of View Model is composed from underlying DB Model. For example: <code>FirstName + \" \" + LastName</code>
        /// </summary>
        public string Expression { get; private set; }
    }
}