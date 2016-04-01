//////////////////////////////////////////////////////////////////////
// File Name    : Redis
// Summary      :
// Author       : dinh.nguyen
// Change Log   : 23/11/2015 3:31:53 PM - Create Date
/////////////////////////////////////////////////////////////////////

using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System.Collections.Generic;

namespace Framework.Caching
{
    /// <summary>
    /// Redis caching server
    /// </summary>
    public static class Redis
    {
        /// <summary>
        /// Host, default value is localhost
        /// </summary>
        public const string Host = "localhost";

        /// <summary>
        ///     Remove all redis data
        /// </summary>
        public static void FlushAll()
        {
            using (var redis = new RedisClient(Host))
            {
                redis.FlushAll();
            }
        }

        /// <summary>
        ///     Remove all data of <see langword="object"/> in redis caching server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void RemoveAll<T>()
        {
            using (var redis = new RedisClient(Host))
            {
                IRedisTypedClient<T> redisObject = redis.As<T>();
                var redisObjectName = typeof(T).Name;
                IRedisList<T> listObject = redisObject.Lists[redisObjectName];
                listObject.RemoveAll();
            }
        }

        /// <summary>
        ///     Add list to redis caching server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void AddList<T>(List<T> listNew)
        {
            using (var redis = new RedisClient(Host))
            {
                IRedisTypedClient<T> redisObject = redis.As<T>();
                var redisObjectName = typeof(T).Name;
                IRedisList<T> listObject = redisObject.Lists[redisObjectName];
                listObject.AddRange(listNew);
            }
        }

        /// <summary>
        ///     get list from redis caching server
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static List<T> GetAll<T>()
        {
            using (var redis = new RedisClient(Host))
            {
                IRedisTypedClient<T> redisObject = redis.As<T>();
                var redisObjectName = typeof(T).Name;
                IRedisList<T> listObject = redisObject.Lists[redisObjectName];
                return listObject.GetAll();
            }
        }
    }
}