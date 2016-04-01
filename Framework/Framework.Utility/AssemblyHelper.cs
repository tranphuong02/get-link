//////////////////////////////////////////////////////////////////////
// File Name    : AssemblyHellper
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 10/12/2015 10:57:44 AM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Framework.Utility
{
    /// <summary>
    /// Helper methods for assembly
    /// </summary>
    public static class AssemblyHelper
    {
        /// <summary>
        ///     Get and load all assemblies in assemblies resolver folder
        ///     If an assembly is loaded, not load it - just get
        /// </summary>
        /// <returns></returns>
        /// <exception cref="SecurityException">The caller does not have the required permissions. </exception>
        /// <exception cref="ArgumentException"><see cref="AppDomain.CurrentDomain.RelativeSearchPath" /> is a zero-length string, contains only white space, or contains one or more of the invalid characters defined in <see cref="M:System.IO.Path.GetInvalidPathChars" />.-or- The system could not retrieve the absolute path. </exception>
        /// <exception cref="NotSupportedException"><see cref="AppDomain.CurrentDomain.RelativeSearchPath" /> contains a colon (":") that is not part of a volume identifier (for example, "c:\"). </exception>
        /// <exception cref="AppDomainUnloadedException">The operation is attempted on an unloaded application domain. </exception>
        /// <exception cref="IOException"><see cref="AppDomain.CurrentDomain.RelativeSearchPath" /> is a file name.-or-A network error has occurred. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive). </exception>
        /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="ArgumentNullException"><see cref="AppDomain.CurrentDomain.RelativeSearchPath" /> or <paramref name="searchPattern" /> is null. </exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="FileLoadException">A file that was found could not be loaded. </exception>
        /// <exception cref="FileNotFoundException">The <paramref name="path" /> parameter is an empty string ("") or does not exist. </exception>
        /// <exception cref="BadImageFormatException"><paramref name="path" /> is not a valid assembly. -or-Version 2.0 or later of the common language runtime is currently loaded and <paramref name="path" /> was compiled with a later version.</exception>
        public static IList<Assembly> GetAssemblies()
        {
            // all assemblies in assemblies resolver folder
            IList<Assembly> assemblies = new List<Assembly>();
            string path = Path.GetFullPath(AppDomain.CurrentDomain.RelativeSearchPath);
            List<string> dllFilesFullPath = Directory.GetFiles(path, searchPattern: "*.dll").ToList();

            // loaded assemblies
            Assembly[] loadedAssemblies =
                AppDomain.CurrentDomain
                .GetAssemblies();

            List<string> loadedAssembliesFullPathWithoutExtension =
                loadedAssemblies
                .Select(x => Path.GetFileNameWithoutExtension(x.CodeBase))
                .ToList();

            // check to get assemblies is loaded, get and load assemblies not loaded
            foreach (var dll in dllFilesFullPath)
            {
                string dllWithoutExtension = Path.GetFileNameWithoutExtension(dll);

                // if already loaded, then add
                // else, load and add that assembly to list
                if (loadedAssembliesFullPathWithoutExtension.Contains(dllWithoutExtension))
                {
                    Assembly loadedAssembly =
                        loadedAssemblies.FirstOrDefault(
                            x => dllWithoutExtension != null && x.FullName.Contains(dllWithoutExtension));

                    if (loadedAssembly != null)
                    {
                        assemblies.Add(loadedAssembly);
                    }
                }
                else
                {
                    assemblies.Add(Assembly.LoadFile(dll));
                }
            }
            return assemblies;
        }

        /// <summary>
        /// Get classes from <paramref name="assemblies"/>
        /// </summary>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetClassesFromAssemblies(this IEnumerable<Assembly> assemblies)
        {
            var allClasses =
                assemblies != null
                ? AllClasses.FromAssemblies(assemblies)
                : AllClasses.FromAssembliesInBasePath();

            return allClasses
                .Where(n => n.Namespace != null && !n.IsInterface)
                .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i)
                .ToList();
        }

        /// <summary>
        /// Get interfaces to be registered
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="TargetInvocationException">A static initializer is invoked and throws an exception. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="interfaces" /> or <paramref name="type" /> is null.</exception>
        public static IEnumerable<Type> GetInterfacesToBeRegistered(this Type type)
        {
            var allInterfacesOnType =
                type
                .GetInterfaces()
                .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i)
                .ToList();

            return allInterfacesOnType;
        }
    }
}