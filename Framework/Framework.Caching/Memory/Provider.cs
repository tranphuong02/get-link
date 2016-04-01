//////////////////////////////////////////////////////////////////////
// File Name    : Provider
// System Name  : BreezeGoodlife
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 03/11/2015 5:54:43 PM - Create Date
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;

namespace Framework.Caching.Memory
{
    /// <summary>
    ///     Cache provider for BreezeGoodlife System
    /// </summary>
    public sealed class Provider : IProvider
    {
        #region Fields

        /// <summary>
        /// <see cref="Instance"/> of <see cref="Provider"/>
        /// </summary>
        private static volatile Provider _instance;

        /// <summary>
        /// Sync Root
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// </summary>
        public MemoryCache MemoryCache { get; set; }

        /// <summary>
        /// </summary>
        public int MemcacheTimeout { get; set; }

        #endregion Fields

        #region Constructor

        /// <summary>
        ///     Create Memory Cache
        /// </summary>
        /// <param name="memcacheTimeout">If null, fetch in AppSettings key <see cref="memcacheTimeout"/> (minutes)</param>
        /// <param name="cacheName">If null, fetch in AppSettings key MemoryCache (memory cache name)</param>
        /// <exception cref="ConfigurationException">A value in the <paramref name="config" /> collection is invalid. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="MemcacheTimeout" /> is null. </exception>
        /// <exception cref="FormatException"><paramref name="MemcacheTimeout" /> is not in the correct format. </exception>
        /// <exception cref="OverflowException"><paramref name="MemcacheTimeout" /> represents a number less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />. </exception>
        public Provider(int? memcacheTimeout = null, string cacheName = null)
        {
            MemcacheTimeout = memcacheTimeout ?? int.Parse(ConfigurationManager.AppSettings["MemcacheTimeout"]);
            cacheName = string.IsNullOrWhiteSpace(cacheName) ? ConfigurationManager.AppSettings["MemoryCache"] : cacheName;
            MemoryCache = new MemoryCache(cacheName);
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        ///     Singleton initialization
        /// </summary>
        /// <exception cref="OverflowException" accessor="get"> represents a number less than <see cref="F:System.Int32.MinValue" /> or greater than <see cref="F:System.Int32.MaxValue" />. </exception>
        /// <exception cref="ConfigurationException" accessor="get">A value in the <paramref name="config" /> collection is invalid. </exception>
        public static Provider Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new Provider();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        ///     Get cache object by key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            if (MemoryCache[key] != null)
                return MemoryCache[key];
            return null;
        }

        /// <summary>
        ///     Set object into cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentException"><paramref name="MemcacheTimeout" /> is equal to <see cref="F:System.Double.NaN" />. </exception>
        /// <exception cref="OverflowException"><paramref name="MemcacheTimeout" /> is less than <see cref="F:System.TimeSpan.MinValue" /> or greater than <see cref="F:System.TimeSpan.MaxValue" />.-or-<paramref name="value" /> is <see cref="F:System.Double.PositiveInfinity" />.-or-<paramref name="value" /> is <see cref="F:System.Double.NegativeInfinity" />. </exception>
        public void Set(string key, object data)
        {
            MemoryCache.Remove(key);
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(MemcacheTimeout)
            };
            MemoryCache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        ///     Set object into cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Set(string key, object data, int cacheTime)
        {
            MemoryCache.Remove(key);
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(cacheTime)
            };

            MemoryCache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        ///     Set object into cache
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="expiry"></param>
        public void Set(string key, object data, DateTime expiry)
        {
            MemoryCache.Remove(key);
            var policy = new CacheItemPolicy
            {
                AbsoluteExpiration = expiry
            };

            MemoryCache.Add(new CacheItem(key, data), policy);
        }

        /// <summary>
        ///     Check cache object is existed in cache or not
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSet(string key)
        {
            return (MemoryCache[key] != null);
        }

        /// <summary>
        ///     Clear cache object by key
        /// </summary>
        /// <param name="key"></param>
        public void Invalidate(string key)
        {
            MemoryCache.Remove(key);
        }

        #endregion Public Methods

        #region Private Helper Methods

        /// <summary>
        ///  Get Enumerator
        /// </summary>
        /// <returns></returns>

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return (IEnumerator<KeyValuePair<string, object>>)MemoryCache.AsEnumerable();
        }

        #endregion Private Helper Methods
    }
}