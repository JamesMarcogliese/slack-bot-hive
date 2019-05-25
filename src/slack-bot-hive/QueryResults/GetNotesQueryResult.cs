using Nest;
using slack_bot_hive.Models;
using System.Collections.Generic;

namespace slack_bot_hive.QueryResults
{
    public class GetNotesQueryResult
    {
        public IEnumerable<Note> Documents { get; set; }
        public QueryResult Result { get; set; }
        public GetNotesQueryResult(IEnumerable<Note> documents, QueryResult result)
        {
            Documents = documents;
            Result = result;
        }
    }
}
