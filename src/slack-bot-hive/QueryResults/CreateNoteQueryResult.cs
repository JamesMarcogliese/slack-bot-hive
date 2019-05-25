using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slack_bot_hive.QueryResults
{
    public class CreateNoteQueryResult
    {
        public QueryResult Result { get; set; }
        public CreateNoteQueryResult(QueryResult result)
        {
            Result = result;
        }
    }
}
