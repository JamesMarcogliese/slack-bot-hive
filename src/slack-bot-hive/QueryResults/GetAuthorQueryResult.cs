using Nest;
using slack_bot_hive.Models;

namespace slack_bot_hive.QueryResults
{
    public class GetAuthorQueryResult
    {
        public Author Document { get; set; }
        public QueryResult Result { get; set; }
        public GetAuthorQueryResult(Author document, QueryResult result)
        {
            Document = document;
            Result = result;
        }
    }
}
