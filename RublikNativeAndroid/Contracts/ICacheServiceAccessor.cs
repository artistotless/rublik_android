using System;
using RublikNativeAndroid.Services;

namespace RublikNativeAndroid.Contracts
{
    public interface ICacheServiceAccessor
    {
        public LocalCacheService GetCacheService();
    }
}
