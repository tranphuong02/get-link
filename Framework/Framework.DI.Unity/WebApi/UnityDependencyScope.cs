using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;

namespace Framework.DI.Unity.WebApi
{
    public class UnityDependencyScope : IDependencyScope
    {
        protected IUnityContainer Container { get; set; }

        public UnityDependencyScope(IUnityContainer container)
        {
            Container = container ?? (IoCFactory.Instance.CurrentContainer as IoCUnityContainer).Container;
        }

        public object GetService(Type serviceType)
        {
            if (typeof(IHttpController).IsAssignableFrom(serviceType))
            {
                return Container.Resolve(serviceType);
            }

            try
            {
                return Container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return null;
            }
        }

        public T GetService<T>()
        {
            try
            {
                var serviceType = typeof(T);
                return (T)Container.Resolve(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return default(T);
            }
        }

        public T GetService<T>(string name)
        {
            try
            {
                var serviceType = typeof(T);
                return (T)Container.Resolve(serviceType, name);
            }
            catch (ResolutionFailedException)
            {
                return default(T);
            }
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            try
            {
                return Container.ResolveAll(serviceType);
            }
            catch (ResolutionFailedException)
            {
                return new List<object>();
            }
        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public void DisposeManagedResources()
        {
            if (Container == null)
            {
                return;
            }

            Container.Dispose();
            Container = null;
        }
    }
}