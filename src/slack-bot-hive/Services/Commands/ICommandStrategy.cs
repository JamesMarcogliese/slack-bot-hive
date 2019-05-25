using System.Threading.Tasks;
using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Commands
{
    public interface ICommandStrategy
    {
        Task ExecuteEvent(SlackEvent slackEvent);
        Task ExecuteAction(SlackAction slackAction);
    }
}
