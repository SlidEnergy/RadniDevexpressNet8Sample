using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CommonBlazor.Infrastructure
{
    public class ApplicationCache
    {
        private IDictionary<string, object?> _storage = new Dictionary<string, object?>();

        public ApplicationCache()
        {

        }

        private string GenerateStorageKey<T>(string key)
        {
            return $"{typeof(T).FullName}_{key}";
        }

        public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var storageKey = GenerateStorageKey<T>(key);

            _storage.TryGetValue(storageKey, out var value);

            return Task.FromResult((T?)value);
        }

        public Task<bool> ContainsAsync<T>(string key)
        {
            var storageKey = GenerateStorageKey<T>(key);

            return Task.FromResult(_storage.ContainsKey(storageKey));
        }

        public Task SetAsync<T>(string key, T? value, CancellationToken cancellationToken = default)
        {
            var storageKey = GenerateStorageKey<T>(key);

            if (_storage.ContainsKey(storageKey))
                _storage[storageKey] = value;
            else
                _storage.Add(storageKey, value);

            return Task.CompletedTask;
        }

        public Task ClearAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            var storageKey = GenerateStorageKey<T>(key);

            if (_storage.ContainsKey(storageKey))
                _storage.Remove(storageKey);

            return Task.CompletedTask;
        }
    }
}
