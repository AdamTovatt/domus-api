using System.Collections.Concurrent;

namespace DomusApi.Helpers
{
    public class CacheManager
    {
        private readonly ConcurrentDictionary<Type, object> caches = new();

        public void AddCache<T>(ConcurrentDictionary<Guid, T> cache) where T : class
        {
            caches[typeof(T)] = cache;
        }

        public ConcurrentDictionary<Guid, T>? GetCache<T>() where T : class
        {
            if (caches.TryGetValue(typeof(T), out object? cache))
            {
                return cache as ConcurrentDictionary<Guid, T>;
            }
            return null;
        }
    }
}
