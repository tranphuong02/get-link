using Microsoft.Practices.Unity;
using System;

namespace Framework.DI.Unity
{
    /// <summary>
    /// IoC Factory
    /// </summary>
    public sealed class IoCFactory : IDisposable
    {
        #region Singleton

        /// <summary>
        /// Get singleton instance of <see cref="IoCFactory"/>
        /// </summary>
        public static IoCFactory Instance { get; } = new IoCFactory();

        #endregion Singleton

        /// <summary>
        /// Get current configured IContainer
        /// <remarks>
        /// At <see langword="this"/> moment only <see cref="IoCUnityContainer"/> exists
        /// </remarks>
        /// </summary>
        public IoCUnityContainer CurrentContainer { get; }

        /// <summary>
        /// Get <see langword="object"/> instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetObjectInstance<T>() where T : class
        {
            return Instance.CurrentContainer.Container.Resolve(typeof(T)) as T;
        }

        #region ctor

        private IoCFactory()
        {
            CurrentContainer = new IoCUnityContainer();
        }

        #endregion ctor

        #region IDisposable

        /// <summary>
        /// detect if already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Destructor
        /// </summary>
        ~IoCFactory()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (CurrentContainer != null)
                        CurrentContainer.Dispose();
                }
            }
            _disposed = true;
        }

        #endregion IDisposable
    }
}