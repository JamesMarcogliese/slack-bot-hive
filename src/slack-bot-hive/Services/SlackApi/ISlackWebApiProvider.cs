using Slack.Api.CSharp.WebApi;

namespace slack_bot_hive.Services.SlackApi
{
    public interface ISlackWebApiProvider
    {
        ISlackWebAPI GetSlackWebAPI();
    }
}
