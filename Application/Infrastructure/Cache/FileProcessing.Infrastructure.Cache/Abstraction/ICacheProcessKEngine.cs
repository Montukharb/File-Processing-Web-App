using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Cache.Abstraction
{
    public interface ICacheProcessKEngine
    {
        Task<bool> SetCacheAsync<T>(string key, T data, int TTL) where T : class;
        Task<T> GetCacheAsync<T>(string key) where T : class;
        Task<bool> RemoveCacheAsync(string key);
    }
}
