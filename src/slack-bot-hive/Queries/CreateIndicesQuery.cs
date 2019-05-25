using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Nest;
using slack_bot_hive.Models;
using slack_bot_hive.QueryResults;
using slack_bot_hive.Services.Elasticsearch;

namespace slack_bot_hive.Queries
{
    public class CreateIndicesQuery : MediatR.IRequest<CreateIndicesQueryResult>
    {
        public CreateIndicesQuery()
        {

        }
    }

    public class CreateIndexQueryHandler : IRequestHandler<CreateIndicesQuery, CreateIndicesQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public CreateIndexQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }

        public async Task<CreateIndicesQueryResult> Handle(CreateIndicesQuery request, CancellationToken cancellationToken)
        {

            var createNoteIndexResponse = await _elasticClient.CreateIndexAsync(Constants.Constants.IndexNameNote, c => c
                .Mappings(ms => ms
                    .Map<Note>(m => m.AutoMap())
                )
            );

            var createAuthorIndexResponse = await _elasticClient.CreateIndexAsync(Constants.Constants.IndexNameAuthor, c => c
                .Mappings(ms => ms
                    .Map<Author>(m => m.AutoMap())
                )
            );

            return new CreateIndicesQueryResult(createNoteIndexResponse.Acknowledged && createAuthorIndexResponse.Acknowledged);
        }
    }
}
