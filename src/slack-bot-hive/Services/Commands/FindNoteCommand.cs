using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using slack_bot_hive.Configuration;
using slack_bot_hive.Queries;
using slack_bot_hive.QueryResults;
using System;
using MediatR;
using System.Text;
using System.Threading.Tasks;
using slack_bot_hive.Services.AttachmentGenerators;
using slack_bot_hive.Services.Hangfire;

namespace slack_bot_hive.Services.Commands
{
    public class FindNoteCommand : ICommand
    {
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly IMediator _mediator;
        private readonly string _accessToken;
        private readonly FindNoteAttachmentGenerator _attachmentGenerator;
        public FindNoteCommand(
            IDisappearingSlackMessageProvider disappearingSlackMessageProvider,
            IOptions<AppConfiguration> appConfiguration,
            IMediator mediator,
            FindNoteAttachmentGenerator attachmentGenerator
            )
        {
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
            _mediator = mediator;
            _attachmentGenerator = attachmentGenerator;
        }
        public Regex GetEventPattern => new Regex(Constants.Constants.EventPatternFindNote);

        public string GetActionName => Constants.Constants.ActionNameEmpty;

        public string GetEventKeywords(string text)
        {
            return Regex.Replace(text, Constants.Constants.MatchPatternFindNote, "");
        }

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {
            string textToSearch = GetEventKeywords(slackEvent.Event.Text);

            GetAuthorQueryResult authorResult = await _mediator.Send(new GetAuthorQuery(slackEvent.Event.User));

            if (authorResult.Result == QueryResult.NotFound)
            {
                await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                    text: Constants.Constants.GetCannotSaveMessage("search"),
                    channel: slackEvent.Event.Channel, 
                    attachment: "");

                return;
            }

            GetNotesQueryResult userNotes = await _mediator.Send(
                new GetNotesQuery(
                    slackEvent.Event.User, 
                    authorResult.Document.Team, 
                    textToSearch, 
                    GetNotesQueryType.SearchUserNotes));

            GetNotesQueryResult teamNotes = await _mediator.Send(
                new GetNotesQuery(
                    slackEvent.Event.User, 
                    authorResult.Document.Team, 
                    textToSearch, 
                    GetNotesQueryType.SearchTeamNotes));

            if (userNotes.Result == QueryResult.NotFound && teamNotes.Result == QueryResult.NotFound)
            {
                await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                    text: Constants.Constants.MessageSearchNoNotes,
                    channel: slackEvent.Event.Channel,
                    attachment: "");

                return;
            }

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(_attachmentGenerator.GenerateAttachment(userNotes.Documents));
            stringBuilder.Append(_attachmentGenerator.GenerateAttachment(teamNotes.Documents));

            string notesAttachment = stringBuilder.ToString();

            await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                    text: Constants.Constants.MessageSearchFoundNotes,
                    channel: slackEvent.Event.Channel,
                    attachment: notesAttachment);
        }

        public async Task ExecuteAction(SlackAction slackAction)
        {
            throw new NotImplementedException();
        }
    }
}
