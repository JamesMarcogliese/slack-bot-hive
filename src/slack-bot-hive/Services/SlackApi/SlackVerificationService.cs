using slack_bot_hive.Configuration;
using Microsoft.Extensions.Options;
using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.SlackApi
{
    public class SlackVerificationService : ISlackVerificationService
    {
        private readonly string _verificationToken;
        public SlackVerificationService(IOptions<AppConfiguration> appConfiguration)
        {
            _verificationToken = appConfiguration.Value.SlackVerificationToken;
        }
        public string VerifySlackEventRequest(SlackEvent challengePayload)
        {
            if (challengePayload != null && challengePayload.Token == _verificationToken)
            {
                return challengePayload.Challenge;
            }
            return null;
        }

        public bool IsValidSlackPayload()
        {
            return true;
        }
    }
}
