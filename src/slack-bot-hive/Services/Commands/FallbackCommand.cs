using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using slack_bot_hive.Configuration;
using System.Threading.Tasks;
using slack_bot_hive.Services.Hangfire;
using Slack.Api.CSharp.WebApi.Models;

namespace slack_bot_hive.Services.Commands
{
    public class FallbackCommand : ICommand
    {
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly string _accessToken;
        public Regex GetEventPattern => new Regex(Constants.Constants.EventPatternFallback);

        public string GetActionName => Constants.Constants.ActionNameEmpty;

        public FallbackCommand(IDisappearingSlackMessageProvider disappearingSlackMessageProvider,
            IOptions<AppConfiguration> appConfiguration
            )
        {
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
        }

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {
            await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                text: Constants.Constants.MessageFallback,
                channel: slackEvent.Event.Channel,
                attachment: "");
        }

        public async Task ExecuteAction(SlackAction slackAction)
        {
            throw new System.NotImplementedException();
        }
    }
}
