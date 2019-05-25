using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using slack_bot_hive.QueryResults;
using slack_bot_hive.Models;
using Microsoft.Extensions.Caching.Memory;

namespace slack_bot_hive.Queries
{
    public class GetCacheItem : MediatR.IRequest<GetCacheItemResult>
    {
        public string Key { get; }

        public GetCacheItem(string key)
        {
            Key = key;
        }
    }

    public class GetCacheItemHandler : IRequestHandler<GetCacheItem, GetCacheItemResult>
    {
        private readonly IMemoryCache _memoryCache;

        public GetCacheItemHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<GetCacheItemResult> Handle(GetCacheItem request, CancellationToken cancellationToken)
        {
            IEnumerable<Note> cacheResult;

            bool cacheHit = _memoryCache.TryGetValue(request.Key, out cacheResult);

            return new GetCacheItemResult(cacheHit, cacheResult);
        }
    }
}
