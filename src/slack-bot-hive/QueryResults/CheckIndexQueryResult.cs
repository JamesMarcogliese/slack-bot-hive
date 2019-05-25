using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slack_bot_hive.QueryResults
{
    public class CheckIndexQueryResult
    {
        public bool Exists { get; }
        public CheckIndexQueryResult(bool exists)
            {
                Exists = exists;
            }
    }
}
