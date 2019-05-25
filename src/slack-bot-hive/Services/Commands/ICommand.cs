using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace slack_bot_hive.Services
{
    public interface ICommand
    {
        Regex GetEventPattern { get; }
        string GetActionName { get; }
        Task ExecuteEvent(SlackEvent slackEvent);
        Task ExecuteAction(SlackAction slackAction);
    }
}
