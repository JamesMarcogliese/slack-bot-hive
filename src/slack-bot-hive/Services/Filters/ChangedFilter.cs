using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Filters
{
    public class ChangedFilter : IFilter
    {
        public bool IsBlocked(SlackEvent slackEvent)
        {
            return slackEvent.Event.SubType == "message_changed";
        }
    }
}
