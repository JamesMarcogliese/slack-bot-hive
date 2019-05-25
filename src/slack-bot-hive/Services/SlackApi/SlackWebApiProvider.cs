using Slack.Api.CSharp.WebApi;

namespace slack_bot_hive.Services.SlackApi
{
    public class SlackWebApiProvider : ISlackWebApiProvider
    {
        public SlackWebApiProvider(){}

        public ISlackWebAPI GetSlackWebAPI()
        {
            return new SlackWebAPI();
        }
    }
}
