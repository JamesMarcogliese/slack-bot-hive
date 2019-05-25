using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using slack_bot_hive.Models;

namespace slack_bot_hive.QueryResults
{
    public class GetCacheItemResult
    {
        public IEnumerable<Note> Notes { get; }
        public bool CacheHit { get; }

        public GetCacheItemResult(bool cacheHit, IEnumerable<Note> notes)
        {
            CacheHit = cacheHit;
            Notes = notes;
        }
    }
}
