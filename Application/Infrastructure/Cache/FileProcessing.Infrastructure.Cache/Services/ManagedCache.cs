using FileProcessing.Infrastructure.Cache.Abstraction;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FileProcessing.Infrastructure.Cache.Services
{
    public class ManagedCache(IDistributedCache _cache, ILogger<ManagedCache> _logger) : ICacheProcessKEngine
    {
        public Task<string> DataSerialization(object data)
        {
            return Task.FromResult(JsonSerializer.Serialize(data));

        }
        public Task<T?> DataDeserialization<T>(string data)
        {
            var obj = JsonSerializer.Deserialize<T>(data);
            return Task.FromResult(obj);
        }
        public Task<bool> SetCacheAsync<T>(string key, T data, int TTL) where T : class
        {
            try
            {
                var jsonStringify = DataSerialization(data).Result;

                _cache.SetStringAsync(key, jsonStringify,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = DateTimeOffset.UtcNow.AddMinutes(TTL)
                    });
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occur when set cache key: {error}", ex);
                throw;
            }
        }
        public Task<T> GetCacheAsync<T>(string key) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveCacheAsync(string key)
        {
            throw new NotImplementedException();
        }

    }
}
