using System.Threading.Tasks;

namespace slack_bot_hive.Services.Hangfire
{
    public interface IDisappearingSlackMessageProvider
    {
        Task SendDisappearingSlackMessage(string text, string attachment, string channel);
        Task SendDisappearingSlackUpdate(string text, string attachment, string channel, string ts);
        Task SnapSlackMessage(string channel, string ts);
    }
}