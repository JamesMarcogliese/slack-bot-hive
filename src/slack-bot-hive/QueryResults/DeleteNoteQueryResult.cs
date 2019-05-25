using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slack_bot_hive.QueryResults
{
    public class DeleteNoteQueryResult
    {
        public QueryResult Result { get; set; }
        public DeleteNoteQueryResult(QueryResult result)
        {
            Result = result;
        }
    }
}
