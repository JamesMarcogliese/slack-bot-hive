using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using slack_bot_hive.Configuration;
using slack_bot_hive.Queries;
using slack_bot_hive.QueryResults;
using slack_bot_hive.Models;
using System;
using MediatR;
using System.Threading.Tasks;
using slack_bot_hive.Services.Hangfire;

namespace slack_bot_hive.Services.Commands
{
    public class SaveNoteCommand : ICommand
    {
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly IMediator _mediator;
        private readonly string _accessToken;
        public SaveNoteCommand(
            IDisappearingSlackMessageProvider disappearingSlackMessageProvider, 
            IOptions<AppConfiguration> appConfiguration, 
            IMediator mediator)
        {
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
            _mediator = mediator;
        }
        public Regex GetEventPattern => new Regex(Constants.Constants.EventPatternSaveNote);

        public string GetActionName => Constants.Constants.ActionNameEmpty;

        public async Task ExecuteAction(SlackAction slackAction)
        {
            throw new NotImplementedException();
        }

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {
            string textToSave = GetEventKeywords(slackEvent.Event.Text);
            GetAuthorQueryResult authorResult = await _mediator.Send(new GetAuthorQuery(slackEvent.Event.User));

            if (authorResult.Result == QueryResult.NotFound)
            {
                await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                    text: Constants.Constants.GetCannotSaveMessage("save"),
                    channel: slackEvent.Event.Channel,
                    attachment: "");

                return;
            }

            Note note = new Note(
                textToSave,
                slackEvent.Event.User,
                authorResult.Document.Team,
                GetUnixTimeNow());

            CreateNoteQueryResult result = await _mediator.Send(new CreateNoteQuery(note));

            await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                text: Constants.Constants.MessageNoteSaved,
                channel: slackEvent.Event.Channel,
                attachment: "");
        }

        public long GetUnixTimeNow()
        {
            DateTime utc = DateTime.UtcNow;
            long unixTime = ((DateTimeOffset)utc).ToUnixTimeSeconds();
            return unixTime;
        }

        public string GetEventKeywords(string text)
        {
            return Regex.Replace(text, Constants.Constants.MatchPatternSaveNote, "");
        }
    }
}
