namespace slack_bot_hive.Services.Filters
{
    public interface IEventFilterBuilder
    {
        IEventFilter GetFilter();
        EventFilterBuilder AddBotFilter();
        EventFilterBuilder AddChangedFilter();
        EventFilterBuilder AddGroupDmFilter();

    }
}
