using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.Commands
{
    public class CommandStrategy : ICommandStrategy
    {
        private readonly IEnumerable<ICommand> _commands;
        public CommandStrategy(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public async Task ExecuteAction(SlackAction slackAction)
        {
            await _commands.FirstOrDefault(x => slackAction.CallbackId.Contains((string) x.GetActionName))?.ExecuteAction(slackAction);
        }

        public async Task ExecuteEvent(SlackEvent slackEvent)
        {
            if (string.IsNullOrWhiteSpace(slackEvent.Event.Text))
                return;
            await _commands.FirstOrDefault(x => x.GetEventPattern.IsMatch(slackEvent.Event.Text))?.ExecuteEvent(slackEvent);
        }
    }
}
