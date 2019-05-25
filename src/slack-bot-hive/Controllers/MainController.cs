using Microsoft.AspNetCore.Mvc;
using Slack.Api.CSharp.EventsApi;
using slack_bot_hive.Services;
using slack_bot_hive.Services.Filters;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;
using slack_bot_hive.Configuration;
using slack_bot_hive.Services.SlackApi;
using slack_bot_hive.Services.Commands;
using slack_bot_hive.Services.Hangfire;
using Microsoft.Extensions.Options;

namespace slack_bot_hive.Controllers
{
    [Route("slack")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly ISlackVerificationService _slackVerificationService;
        private readonly ICommandStrategy _commandStrategy;
        private readonly IEventFilter _eventFilter;
        private readonly IDisappearingSlackMessageProvider _disappearingSlackMessageProvider;
        private readonly string _accessToken;
        public MainController(
            ISlackVerificationService slackVerificationService,
            ICommandStrategy commandStrategy,
            IEventFilterBuilder eventFilterBuilder,
            IDisappearingSlackMessageProvider disappearingSlackMessageProvider,
            IOptions<AppConfiguration> appConfiguration)
        {
            _slackVerificationService = slackVerificationService;
            _commandStrategy = commandStrategy;
            _eventFilter = eventFilterBuilder
                .AddBotFilter()
                .AddChangedFilter()
                .AddGroupDmFilter()
                .GetFilter();
            _disappearingSlackMessageProvider = disappearingSlackMessageProvider;
            _accessToken = appConfiguration.Value.SlackBotUserAccessToken;
        }

        [HttpPost]
        [Route("events")]
        public async Task<IActionResult> Event([FromBody]SlackEvent request)
        {

            switch (request.Type)
            {
                case (Constants.Constants.RequestTypeUrlVerification):
                    return Ok(_slackVerificationService.VerifySlackEventRequest(request));
                case (Constants.Constants.RequestTypeEventCallback):
                    if (!_eventFilter.IsBlocked(request))
                        Hangfire.BackgroundJob.Enqueue(() => 
                            _commandStrategy.ExecuteEvent(request));
                    await _disappearingSlackMessageProvider.SnapSlackMessage(
                        request.Event.Channel,
                        request.Event.Ts);
                    break;
                default:
                    return Ok();
            }

            return Ok();

        }

        [HttpPost]
        [Route("actions")]
        [Consumes("application/x-www-form-urlencoded")]
        public async Task<IActionResult> ActionAsync([FromForm] IFormCollection formCollection)
        {
            StringValues request;
            formCollection.TryGetValue("payload", out request);
            SlackAction action = SlackAction.FromJson(request.ToString());

            if (action.Type == Constants.Constants.RequestTypeInteractiveMessage)
                Hangfire.BackgroundJob.Enqueue(() => _commandStrategy.ExecuteAction(action)); 

            return Ok();
        }
    }
}
