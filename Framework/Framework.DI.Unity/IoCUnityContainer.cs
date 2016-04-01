using Microsoft.Practices.Unity;
using System;

namespace Framework.DI.Unity
{
    /// <summary>
    /// Container Unity
    /// </summary>
    public sealed class IoCUnityContainer : IDisposable
    {
        /// <summary>
        /// Root container
        /// </summary>
        private IUnityContainer _rootContainer;

        #region Properties

        /// <summary>
        /// Container Unity
        /// </summary>
        public IUnityContainer Container => RootContainer;

        /// <summary>
        /// <see cref="Container"/> principal Unity
        /// </summary>
        private IUnityContainer RootContainer
        {
            get
            {
                if (_disposed)
                {
                    throw new InvalidOperationException("Disposed");
                }

                return _rootContainer ?? (_rootContainer = new UnityContainer());
            }
        }

        #endregion Properties

        #region IDisposable

        /// <summary>
        /// detect if already disposed
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Destructor
        /// </summary>
        ~IoCUnityContainer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="this" /> is null. </exception>
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
                    _rootContainer?.Dispose();
                }
            }
            _disposed = true;
        }

        #endregion IDisposable
    }
}