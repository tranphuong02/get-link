﻿//////////////////////////////////////////////////////////////////////
// File Name    : Guard
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 10/12/2015 11:06:31 AM - Create Date
/////////////////////////////////////////////////////////////////////

using Framework.Utility.Properties;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace Framework.Utility
{
    /// <summary>
    /// A <see langword="static"/> helper class that includes various parameter checking routines.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if the given argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException"> if tested value if null.</exception>
        /// <param name="argumentValue">Argument value to test.</param>
        /// <param name="argumentName">Name of the argument being tested.</param>
        public static void ArgumentNotNull(object argumentValue,
                                           string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        /// <summary>
        /// Throws an exception if the tested string argument is <see langword="null"/> or the empty string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if string value is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown if the string is empty</exception>
        /// <param name="argumentValue">Argument value to check.</param>
        /// <param name="argumentName">Name of argument being <see langword="checked"/>.</param>
        public static void ArgumentNotNullOrEmpty(string argumentValue,
                                                  string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException(Resources.ArgumentMustNotBeEmpty, argumentName);
            }
        }

        /// <summary>
        /// Verifies that an argument type is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentValueType">The type of the value being assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assignmentTargetType"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Condition.</exception>
        /// <exception cref="FormatException"><paramref name="assignmentTargetType" /> is invalid.-or- The index of a format item is less than zero, or greater than or equal to the length of the <paramref name="args" /> array. </exception>
        public static void TypeIsAssignable(Type assignmentTargetType, Type assignmentValueType, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException(nameof(assignmentTargetType));
            }

            if (assignmentValueType == null)
            {
                throw new ArgumentNullException(nameof(assignmentValueType));
            }

            if (!assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentValueType.GetTypeInfo()))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.TypesAreNotAssignable,
                    assignmentTargetType,
                    assignmentValueType),
                    argumentName);
            }
        }

        /// <summary>
        /// Verifies that an argument instance is assignable from the provided type (meaning
        /// interfaces are implemented, or classes exist in the base class hierarchy, or instance can be
        /// assigned through a runtime wrapper, as is the case for COM Objects).
        /// </summary>
        /// <param name="assignmentTargetType">The argument type that will be assigned to.</param>
        /// <param name="assignmentInstance">The instance that will be assigned.</param>
        /// <param name="argumentName">Argument name.</param>
        /// <exception cref="ArgumentNullException"><paramref name="assignmentTargetType"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">Condition.</exception>
        public static void InstanceIsAssignable(Type assignmentTargetType, object assignmentInstance, string argumentName)
        {
            if (assignmentTargetType == null)
            {
                throw new ArgumentNullException(nameof(assignmentTargetType));
            }

            if (assignmentInstance == null)
            {
                throw new ArgumentNullException(nameof(assignmentInstance));
            }

            if (!assignmentTargetType.GetTypeInfo().IsAssignableFrom(assignmentInstance.GetType().GetTypeInfo()))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.TypesAreNotAssignable,
                        assignmentTargetType,
                        GetTypeName(assignmentInstance)),
                    argumentName);
            }
        }

        /// <summary>
        /// Get type name
        /// </summary>
        /// <param name="assignmentInstance"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Need to use exception as flow control here, no other choice")]
        private static string GetTypeName(object assignmentInstance)
        {
            string assignmentInstanceType;
            try
            {
                assignmentInstanceType = assignmentInstance.GetType().FullName;
            }
            catch
            {
                assignmentInstanceType = Resources.UnknownType;
            }
            return assignmentInstanceType;
        }
    }
}