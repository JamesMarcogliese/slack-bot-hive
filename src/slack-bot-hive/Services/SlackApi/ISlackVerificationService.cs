using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.SlackApi
{
    public interface ISlackVerificationService
    {
        string VerifySlackEventRequest(SlackEvent challengePayload);
        bool IsValidSlackPayload();
    }
}
