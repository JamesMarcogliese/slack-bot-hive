using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Filters
{
    public class GroupDmFilter : IFilter
    {
        public bool IsBlocked(SlackEvent slackEvent)
        {
            return slackEvent.Event.Channel.StartsWith('G');
        }
    }
}
