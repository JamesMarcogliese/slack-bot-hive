using System;
using slack_bot_hive.Configuration;
using Microsoft.Extensions.Options;
using Nest;

namespace slack_bot_hive.Services.Elasticsearch
{
    public class ElasticsearchClientProvider : IElasticsearchClientProvider
    {
        private ElasticClient Client { get; }
        public ElasticsearchClientProvider(IOptions<AppConfiguration> appConfiguration)
        {
            var settings = new ConnectionSettings(new Uri(appConfiguration.Value.ElasticsearchEndpoint));

            Client = new ElasticClient(settings);

        }
        public IElasticClient GetElasticClient()
        {
            return Client;
        }
    }
}
