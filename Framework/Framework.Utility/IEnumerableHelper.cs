using System.Collections.Generic;
using System.Linq;

namespace Framework.Utility
{
    public static class IEnumerableHelper
    {
        /// <summary>
        /// Break a list of items into chunks of a specific size
        /// </summary>
        public static IEnumerable<List<T>> Chunk<T>(this List<T> source, int chunksize)
        {
            while (source.Any())
            {
                yield return source.Take(chunksize).ToList();
                source = source.Skip(chunksize).ToList();
            }
        }
    }
}