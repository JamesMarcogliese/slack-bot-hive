using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using slack_bot_hive.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace slack_bot_hive.Queries
{
    public class SetCacheItem : IRequest
    {
        public IEnumerable<Note> Items { get; }
        public string Key { get; }

        public SetCacheItem(string key, IEnumerable<Note> notes)
        {
            Key = key;
            Items = notes;
        }
    }

    public class SetCacheItemHandler : AsyncRequestHandler<SetCacheItem>
    {
        private readonly IMemoryCache _memoryCache;

        public SetCacheItemHandler(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        protected override async Task Handle(SetCacheItem request, CancellationToken cancellationToken)
        {
            _memoryCache.Set(request.Key, request.Items, TimeSpan.FromMinutes(5));
        }
    }
}
