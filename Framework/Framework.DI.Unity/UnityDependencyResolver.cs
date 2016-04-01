//////////////////////////////////////////////////////////////////////
// File Name    : UnityDependencyResolver
// System Name  : Framework.DI.Unity
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 11/3/2015 4:50:28 PM - Create Date
/////////////////////////////////////////////////////////////////////

using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace Framework.DI.Unity
{
    /// <summary>
    /// Unity dependency injection
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        /// <summary>
        /// HTTP context key
        /// </summary>
        private const string HttpContextKey = "perRequestContainer";

        /// <summary>
        /// Container
        /// </summary>
        private readonly IUnityContainer _container;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="container"></param>
        public UnityDependencyResolver(IUnityContainer container)
        {
            _container = container ?? (IoCFactory.Instance.CurrentContainer as IoCUnityContainer).Container;
        }

        /// <summary>
        /// GetService / Resolve by <see cref="Type"/>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Cannot resolve.</exception>
        public object GetService(Type serviceType)
        {
            try
            {
                if (typeof(IController).IsAssignableFrom(serviceType))
                {
                    return ChildContainer.Resolve(serviceType);
                }

                return IsRegistered(serviceType) ? ChildContainer.Resolve(serviceType) : null;
            }
            catch (Exception ex)
            {
                DebugWriteException(ex);
                throw;
            }
        }

        /// <summary>
        /// Get Services by <see cref="Type"/>
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (IsRegistered(serviceType))
            {
                yield return ChildContainer.Resolve(serviceType);
            }

            foreach (var service in ChildContainer.ResolveAll(serviceType))
            {
                yield return service;
            }
        }

        /// <summary>
        /// Children container
        /// </summary>
        protected IUnityContainer ChildContainer
        {
            get
            {
                var childContainer = HttpContext.Current.Items[HttpContextKey] as IUnityContainer;

                if (childContainer == null)
                {
                    HttpContext.Current.Items[HttpContextKey] = childContainer = _container.CreateChildContainer();
                }

                return childContainer;
            }
        }

        /// <summary>
        /// Dispose children container
        /// </summary>
        public static void DisposeOfChildContainer()
        {
            var childContainer = HttpContext.Current.Items[HttpContextKey] as IUnityContainer;

            childContainer?.Dispose();
        }

        /// <summary>
        /// Check type is already register or not
        /// </summary>
        /// <param name="typeToCheck"></param>
        /// <returns></returns>
        private bool IsRegistered(Type typeToCheck)
        {
            var isRegistered = true;

            if (typeToCheck.IsInterface || typeToCheck.IsAbstract)
            {
                isRegistered = ChildContainer.IsRegistered(typeToCheck);

                if (!isRegistered && typeToCheck.IsGenericType)
                {
                    var openGenericType = typeToCheck.GetGenericTypeDefinition();

                    isRegistered = ChildContainer.IsRegistered(openGenericType);
                }
            }

            return isRegistered;
        }

        /// <summary>
        /// Print to console <see langword="interface"/> map to class
        /// </summary>
        /// <param name="ex"></param>
        private static void DebugWriteException(Exception ex)
        {
            Debug.WriteLine("----------------------------------------", "Exception");
            Debug.WriteLine("Please try to CLEAN and BUILD project again", "Information");
            Debug.WriteLine("----------------------------------------", "Exception");
            Debug.WriteLine(ex.InnerException?.Message ?? ex.Message, "Exception");
            Debug.WriteLine("----------------------------------------", "Exception");
        }
    }
}