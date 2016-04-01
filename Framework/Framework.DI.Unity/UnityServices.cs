//////////////////////////////////////////////////////////////////////
// File Name    : UnityServices
// System Name  : Framework.DI.Unity
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 11/3/2015 4:50:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

using Framework.DI.Contracts.Interfaces;
using Framework.Utility;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Framework.DI.Unity
{
    /// <summary>
    /// Unity Services Helper
    /// </summary>
    public static class UnityServices
    {
        /// <summary>
        ///     Auto mapping interfaces and object
        /// </summary>
        /// <param name="container"></param>
        /// <param name="assemblies"></param>
        public static void AutoRegister(IUnityContainer container, IList<Assembly> assemblies = null)
        {
            Validate(container);

            if (assemblies == null)
            {
                assemblies = AssemblyHelper.GetAssemblies();
            }

            IEnumerable<Type> listClass = assemblies.GetClassesFromAssemblies();

            RegisterTypesWithConvention(container, listClass);
        }

        /// <summary>
        ///     Register types with convention Interface must inheritance
        ///     <see cref="IDependency"/>/
        ///     <see cref="IPerRequestDependency"/>/
        ///     <see cref="ISingletonDependency"/>
        /// </summary>
        /// <param name="container"></param>
        /// <param name="listClass"></param>
        public static void RegisterTypesWithConvention(IUnityContainer container, IEnumerable<Type> listClass)
        {
            Validate(container);

            foreach (Type @class in listClass)
            {
                var listInterface = @class.GetInterfacesToBeRegistered();
                Type @localClass = @class;
                foreach (Type @interface in listInterface.Where(x => x.IsGenericType == localClass.IsGenericType))
                {
                    if (typeof(ISingletonDependency).IsAssignableFrom(@interface))
                    {
                        // Type extend from ISingletonDependency interface,
                        // so register it as Singleton
                        container.RegisterType(@interface, @class, new ContainerControlledLifetimeManager());
                    }
                    else if (typeof(IPerRequestDependency).IsAssignableFrom(@interface))
                    {
                        // Type extend from IPerRequestDependency interface,
                        // so register it as Instant per Request
                        container.RegisterType(@interface, @class, new PerRequestLifetimeManager());
                    }
                    else if (typeof(IDependency).IsAssignableFrom(@interface))
                    {
                        // Type extend from IDependency interface,
                        // so register it as Instant per Dependency
                        container.RegisterType(@interface, @class, new HierarchicalLifetimeManager());
                    }
                }
            }

            PrintRegisterInformation(container);
        }

        /// <summary>
        ///     Validate for Unity Framework
        /// </summary>
        /// <param name="container"></param>
        private static void Validate(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
        }

        /// <summary>
        ///     Print to console register type and <see langword="interface"/> of container
        /// </summary>
        /// <param name="container"></param>
        [Conditional("DEBUG")]
        private static void PrintRegisterInformation(IUnityContainer container)
        {
            Debug.WriteLine("--------------------------------------------");
            Debug.WriteLine("DI register for " + container.Registrations.Count() + " class");
            foreach (var registed in container.Registrations)
            {
                Debug.WriteLine(
                    $"DI register for interface: {registed.RegisteredType.FullName} - Map To class: {registed.MappedToType.FullName}"
                    , "Information");
            }
            Debug.WriteLine("--------------------------------------------");
        }
    }
}