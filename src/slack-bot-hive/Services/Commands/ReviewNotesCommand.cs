using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using slack_bot_hive.Configuration;
using slack_bot_hive.Queries;
using slack_bot_hive.QueryResults;
using System;
using MediatR;
using System.Linq;
using System.Threading.Tasks;
using slack_bot_hive.Services.AttachmentGenerators;
using slack_bot_hive.Services.CallbackIdBuilders;
using slack_bot_hive.Services.Hangfire;
using Slack.Api.CSharp.WebApi.Models;

namespace slack_bot_hive.Services.Commands
{
    public class ReviewNotesCommand : ICommand
    {
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly IMediator _mediator;
        private readonly string _accessToken;
        private readonly ReviewNotesAttachmentGenerator _attachmentGenerator;
        public ReviewNotesCommand(
            IDisappearingSlackMessageProvider disappearingSlackMessageProvider,
            IOptions<AppConfiguration> appConfiguration,
            IMediator mediator,
            ReviewNotesAttachmentGenerator attachmentGenerator 
            )
        {
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
            _mediator = mediator;
            _attachmentGenerator = attachmentGenerator;
        }
        public Regex GetEventPattern => new Regex(Constants.Constants.EventPatternReview);

        public string GetActionName => Constants.Constants.ActionNameReviewSelection;

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {

            GetNotesQueryResult results = await _mediator.Send(new GetNotesQuery(slackEvent.Event.User, null, null, GetNotesQueryType.GetUserNotes));

            if (results.Result == QueryResult.NotFound)
            {
                await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                    text: Constants.Constants.MessageNoUserNotesFound,
                    channel: slackEvent.Event.Channel,
                    attachment: "");

                return;
            }

            string cacheId = Guid.NewGuid().ToString();

            await _mediator.Send(new SetCacheItem(cacheId, results.Documents));

            dynamic notesAttachment = _attachmentGenerator.GenerateAttachment(results.Documents, cacheId, 1);

            await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                    text: Constants.Constants.MessageUserNotesFound,
                    channel: slackEvent.Event.Channel,
                    attachment: (string)notesAttachment);

        }

        public async Task ExecuteAction(SlackAction slackAction)
        {
            if (slackAction.CallbackId.Contains(Constants.Constants.CallbackIdTypeNote))
                HandleDeleteCallback(slackAction);
            else if (slackAction.CallbackId.Contains(Constants.Constants.CallbackIdTypePage))
                HandlePageCallback(slackAction);

        }

        public async Task HandleDeleteCallback(SlackAction slackAction)
        {

            DeleteCallbackId deleteCallbackId = DeleteCallbackId.FromJson(slackAction.CallbackId);

            DeleteNoteQueryResult result = await _mediator.Send(new DeleteNoteQuery(deleteCallbackId.DocumentId));

            if (result.Result == QueryResult.Success)
            {
                await _disappearingSlackMessageProvider.SendDisappearingSlackUpdate(
                    text: Constants.Constants.MessageNoteDeleted,
                    channel: slackAction.Channel.Id,
                    ts: slackAction.MessageTs,
                    attachment: "");
            }
        }

        public async Task HandlePageCallback(SlackAction slackAction)
        {
            PageCallbackId pageCallbackId = PageCallbackId.FromJson(slackAction.CallbackId);

            GetCacheItemResult result = await _mediator.Send(new GetCacheItem(pageCallbackId.CacheItemId));

            int page = 1;
            page = Convert.ToInt32(pageCallbackId.Page);

            switch (slackAction.Actions.First().Value)
            {
                case "nextpage":
                    page++;
                    break;
                case "prevpage":
                    page--;
                    break;
            }

            dynamic notesAttachment = _attachmentGenerator.GenerateAttachment(result.Notes, pageCallbackId.CacheItemId, page);

            await _disappearingSlackMessageProvider.SendDisappearingSlackUpdate(
                channel: slackAction.Channel.Id,
                ts: slackAction.MessageTs,
                attachment: (string)notesAttachment,
                text: "");
        }

    }
}
