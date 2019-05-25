using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slack_bot_hive.QueryResults
{
    public class CreateIndicesQueryResult
    {
        public bool Created { get; }
        public CreateIndicesQueryResult(bool created)
            {
                Created = created;
            }
    }
}
