using System.Collections.Generic;
using System.Linq;
using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Filters
{
    public class EventFilter : IEventFilter
    {
        private readonly List<IFilter> _filters;
        public EventFilter(IEnumerable<IFilter> filters)
        {
            _filters = filters.ToList<IFilter>();
        }
        public bool IsBlocked(SlackEvent slackEvent)
        {
            return _filters.Any(x => x.IsBlocked(slackEvent));
        }
    }
}
