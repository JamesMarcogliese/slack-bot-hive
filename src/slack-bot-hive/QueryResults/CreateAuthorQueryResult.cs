using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slack_bot_hive.QueryResults
{
    public class CreateAuthorQueryResult
    {
        public QueryResult Result { get; set; }
        public CreateAuthorQueryResult(QueryResult result)
        {
            Result = result;
        }
    }
}
