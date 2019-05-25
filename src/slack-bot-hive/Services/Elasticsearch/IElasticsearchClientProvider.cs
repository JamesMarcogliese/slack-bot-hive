using Nest;

namespace slack_bot_hive.Services.Elasticsearch
{
    public interface IElasticsearchClientProvider
    {
        IElasticClient GetElasticClient();
    }
}
