using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slack_bot_hive.QueryResults
{
    public class DeleteAuthorQueryResult
    {
        public QueryResult Result { get; set; }
        public DeleteAuthorQueryResult(QueryResult result)
        {
            Result = result;
        }
    }
}
