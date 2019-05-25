using slack_bot_hive.Services.Elasticsearch;
using MediatR;
using Nest;
using slack_bot_hive.QueryResults;

namespace slack_bot_hive.Queries
{
    public class CheckIndexQuery : MediatR.IRequest<CheckIndexQueryResult>
    {
        public string IndexName { get; }
        public CheckIndexQuery(string indexName)
        {
            IndexName = indexName;
        }
    }

    public class CheckIndexQueryHandler : RequestHandler<CheckIndexQuery, CheckIndexQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public CheckIndexQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }

        protected override CheckIndexQueryResult Handle(CheckIndexQuery request)
        {
            var response = _elasticClient.IndexExists(request.IndexName);

            return new CheckIndexQueryResult(response.Exists);
        }
    }
}
