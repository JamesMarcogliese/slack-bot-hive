using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Filters
{
    public interface IFilter
    {
        bool IsBlocked(SlackEvent slackEvent);
    }
}
