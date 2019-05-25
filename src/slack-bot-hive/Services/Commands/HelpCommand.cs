using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using slack_bot_hive.Configuration;
using System.Threading.Tasks;
using slack_bot_hive.Services.Hangfire;

namespace slack_bot_hive.Services.Commands
{
    public class HelpCommand : ICommand
    {
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly string _accessToken;
        public HelpCommand(IDisappearingSlackMessageProvider disappearingSlackMessageProvider,
            IOptions<AppConfiguration> appConfiguration
            )
        {
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
        }
        public Regex GetEventPattern => new Regex(Constants.Constants.EventPatternHelp);

        public string GetActionName => Constants.Constants.ActionNameEmpty;

        public async Task ExecuteAction(SlackAction slackAction)
        {
            throw new System.NotImplementedException();
        }

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {
            await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                text: Constants.Constants.MessageHelp, 
                channel: slackEvent.Event.Channel,
                attachment: "");

        }
    }
}
