//////////////////////////////////////////////////////////////////////
// File Name    : IProvider
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 5:54:10 PM - Create Date
/////////////////////////////////////////////////////////////////////

namespace Framework.Caching.Memory
{
    /// <summary>
    /// Memory caching provider
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Get value in memory by <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Set value in memory by <paramref name="key"/> and <paramref name="data"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Set(string key, object data);

        /// <summary>
        /// Set value in memory by <paramref name="key"/> and <paramref name="data"/> with <paramref name="cacheTime"/>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        void Set(string key, object data, int cacheTime);

        /// <summary>
        /// check <paramref name="key"/> is already in memory or not
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsSet(string key);

        /// <summary>
        /// Expired a <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        void Invalidate(string key);
    }
}