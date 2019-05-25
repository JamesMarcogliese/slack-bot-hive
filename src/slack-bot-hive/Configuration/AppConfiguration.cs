using System.ComponentModel.DataAnnotations;

namespace slack_bot_hive.Configuration
{
    public class AppConfiguration
    {
        [Required]
        public string SlackVerificationToken { get; set; }
        [Required]
        public string SlackBotUserAccessToken { get; set; }
        [Required]
        public string ElasticsearchEndpoint { get; set; }
    }
}
