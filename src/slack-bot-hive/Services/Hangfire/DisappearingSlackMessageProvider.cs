using System;
using System.Threading.Tasks;
using slack_bot_hive.Configuration;
using slack_bot_hive.Services.SlackApi;
using Hangfire;
using Microsoft.Extensions.Options;
using Slack.Api.CSharp.WebApi;
using Slack.Api.CSharp.WebApi.Models;

namespace slack_bot_hive.Services.Hangfire
{
    public class DisappearingSlackMessageProvider : IDisappearingSlackMessageProvider
    {
        private readonly ISlackWebAPI _slackWebApi;
        private readonly string _botUserAccessToken;
        private readonly string _oAuthAccessToken;
        public DisappearingSlackMessageProvider(
            ISlackWebApiProvider _slackWebApiProvider,
            IOptions<AppConfiguration> appConfiguration)
        {
            _slackWebApi = _slackWebApiProvider.GetSlackWebAPI();
            _botUserAccessToken = appConfiguration.Value.SlackBotUserAccessToken;
        }

        public async Task SendDisappearingSlackMessage(string text, string attachment, string channel)
        {
            PostMessageOKResponse resp = await _slackWebApi.Chat.PostMessageAsync(
                token: _botUserAccessToken,
                text: text,
                channel: channel,
                attachments: attachment,
                asUser: false);

            BackgroundJob.Schedule(() =>
                    DeleteSlackMessage(_botUserAccessToken, channel, resp.Ts, false),
                TimeSpan.FromSeconds(Constants.Constants.DisappearingMessageTimeSpan));
        }

        public async Task SendDisappearingSlackUpdate(string text, string attachment, string channel, string ts)
        {
            UpdateOKResponse resp = await _slackWebApi.Chat.UpdateAsync(
                token: _botUserAccessToken,
                text: text,
                channel: channel,
                ts: ts,
                attachments: attachment,
                asUser: false);

            BackgroundJob.Schedule(() =>
                    DeleteSlackMessage(_botUserAccessToken, channel, resp.Ts, false),
                TimeSpan.FromSeconds(Constants.Constants.DisappearingMessageTimeSpan));
        }

        public async Task SnapSlackMessage(string channel, string ts)
        {
            BackgroundJob.Schedule(() =>
                    DeleteSlackMessage(_oAuthAccessToken, channel, ts, true),
                TimeSpan.FromSeconds(Constants.Constants.DisappearingMessageTimeSpan));
        }

        public async Task DeleteSlackMessage(string token, string channel, string ts, bool asUser)
        {
            await _slackWebApi.Chat.DeleteAsync(
                token: token,
                channel: channel,
                ts: ts,
                asUser: asUser);
        }
    }
}

