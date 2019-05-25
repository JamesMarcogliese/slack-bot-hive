using Slack.Api.CSharp.EventsApi;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using slack_bot_hive.Configuration;
using slack_bot_hive.Queries;
using slack_bot_hive.QueryResults;
using slack_bot_hive.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MediatR;
using System.Threading.Tasks;
using slack_bot_hive.Services.Hangfire;
using Slack.Api.CSharp.WebApi.Models;

namespace slack_bot_hive.Services.Commands
{
    public class SetTeamCommand : ICommand
    {
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly string _accessToken;
        private readonly string _teamMenuAttachment;
        private readonly IMediator _mediator;
        public SetTeamCommand(
            IDisappearingSlackMessageProvider disappearingSlackMessageProvider, 
            IOptions<AppConfiguration> appConfiguration, 
            IHostingEnvironment hostingEnvironment, 
            IMediator mediator)
        {
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
            _teamMenuAttachment = File.ReadAllText(hostingEnvironment.ContentRootPath + Constants.Constants.PathTeamMenu);
            _mediator = mediator;
        }
        public Regex GetEventPattern => new Regex(Constants.Constants.EventPatternSetTeam);

        public string GetActionName => Constants.Constants.ActionNameTeamSelection;

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {
            await _disappearingSlackMessageProvider.SendDisappearingSlackMessage(
                text: Constants.Constants.MessageTeamQuestion, 
                channel: slackEvent.Event.Channel, 
                attachment: _teamMenuAttachment);
        }

        public async Task ExecuteAction(SlackAction slackAction)
        {
            string selectedTeam = slackAction.Actions[0].SelectedOptions[0].value;

            GetAuthorQueryResult result = await _mediator.Send(new GetAuthorQuery(slackAction.User.Id));

            if (result.Result != QueryResult.NotFound)
                await _mediator.Send(new DeleteAuthorQuery(result.Document.DocumentId));

            CreateAuthorQueryResult creationResult = await _mediator.Send(new CreateAuthorQuery(new Author(slackAction.User.Id, selectedTeam)));

            await _disappearingSlackMessageProvider.SendDisappearingSlackUpdate(
                text: Constants.Constants.GetTeamJoinMessage(selectedTeam),
                channel: slackAction.Channel.Id,
                ts: slackAction.MessageTs,
                attachment: "");
        }
    }
}
