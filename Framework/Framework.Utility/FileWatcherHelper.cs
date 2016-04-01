//////////////////////////////////////////////////////////////////////
// File Name    : FileWatcherHelper
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 60:15:32 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System.IO;

namespace Framework.Utility
{
    /// <summary>
    /// <see cref="File"/> watcher methods helpers
    /// </summary>
    public class FileWatcherHelper
    {
        private readonly FileSystemWatcher _watcher;

        /// <summary>
        /// Delegate for event on change
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public delegate void OnChangedHelper(object source, FileSystemEventArgs e);

        /// <summary>
        ///     On file changed event
        /// </summary>
        public OnChangedHelper OnChanged { get; set; }

        /// <summary>
        ///     Create new instance of file system watcher
        /// </summary>
        /// <exception cref="ArgumentException">The <paramref name="path" /> parameter contains invalid characters, is empty, or contains only white spaces. </exception>
        /// <exception cref="PathTooLongException">NoteIn the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.IO.IOException" />, instead.The <paramref name="path" /> parameter is longer than the system-defined maximum length.</exception>
        public FileWatcherHelper(string path)
        {
            _watcher = new FileSystemWatcher
            {
                Path = Path.GetDirectoryName(path),
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = Path.GetFileName(path),
                IncludeSubdirectories = false
            };

            // Add Event
            _watcher.Changed += OnFileChanged;
        }

        /// <summary>
        ///     Start watcher
        /// </summary>
        /// <exception cref="FileNotFoundException">The directory specified in <see cref="P:System.IO.FileSystemWatcher.Path" /> could not be found.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.FileSystemWatcher" /> object has been disposed.</exception>
        /// <exception cref="PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="ArgumentException"><see cref="P:System.IO.FileSystemWatcher.Path" /> has not been set or is invalid.</exception>
        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        ///     Stop watcher
        /// </summary>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.IO.FileSystemWatcher" /> object has been disposed.</exception>
        /// <exception cref="FileNotFoundException">The directory specified in <see cref="P:System.IO.FileSystemWatcher.Path" /> could not be found.</exception>
        /// <exception cref="PlatformNotSupportedException">The current operating system is not Microsoft Windows NT or later.</exception>
        /// <exception cref="ArgumentException"><see cref="P:System.IO.FileSystemWatcher.Path" /> has not been set or is invalid.</exception>
        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        ///     On file changed event
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnFileChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                _watcher.EnableRaisingEvents = false;

                // do delegate
                OnChanged?.Invoke(source, e);
            }
            finally
            {
                _watcher.EnableRaisingEvents = true;
            }
        }
    }
}