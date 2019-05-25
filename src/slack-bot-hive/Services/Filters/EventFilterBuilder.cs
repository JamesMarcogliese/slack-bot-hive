using System.Collections.Generic;

namespace slack_bot_hive.Services.Filters
{
    public class EventFilterBuilder : IEventFilterBuilder
    {
        List<IFilter> _filters = new List<IFilter>();

        public EventFilterBuilder AddGroupDmFilter()
        {
            _filters.Add(new GroupDmFilter());
            return this;
        }

        public EventFilterBuilder AddBotFilter()
        {
            _filters.Add(new BotFilter());
            return this;
        }

        public EventFilterBuilder AddChangedFilter()
        {
            _filters.Add(new ChangedFilter());
            return this;
        }

        public IEventFilter GetFilter()
        {
            return new EventFilter(_filters);
        }
    }
}
