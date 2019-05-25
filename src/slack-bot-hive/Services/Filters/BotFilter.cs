using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Filters
{
    public class BotFilter : IFilter
    {
        public bool IsBlocked(SlackEvent slackEvent)
        {
            return slackEvent.Event.SubType == "bot_message";
        }
    }
}
