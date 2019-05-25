using Nest;
using System;

namespace slack_bot_hive.Models
{
    [ElasticsearchType(Name = "note")]
    public class Note
    {
        [Text(Name = "text")]
        public string Text { get; set; }
        [Keyword(Name = "author")]
        public string Author { get; set; }
        [Keyword(Name = "team")]
        public string Team { get; set; }
        [Date(Name = "timestamp")]
        public long Timestamp { get; set; }
        [Keyword(Name = "document_id")]
        public string DocumentId { get; set; }
        public Note(string text, string author, string team, long timestamp)
        {
            Text = text;
            Author = author;
            Team = team;
            Timestamp = timestamp;
        }
    }
}
