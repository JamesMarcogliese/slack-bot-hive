using Nest;
using System;

namespace slack_bot_hive.Models
{
    [ElasticsearchType(Name = "author")]
    public class Author
    {
        [Keyword(Name = "slack_id")]
        public string SlackId { get; set; }
        [Keyword(Name = "team")]
        public string Team { get; set; }
        [Keyword(Name = "document_id")]
        public string DocumentId { get; set; }
        public Author(string slackId, string team)
        {
            SlackId = slackId;
            Team = team;
        }
    }
}
