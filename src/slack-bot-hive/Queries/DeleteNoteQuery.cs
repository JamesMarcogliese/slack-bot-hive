using System.Threading;
using System.Threading.Tasks;
using slack_bot_hive.Services.Elasticsearch;
using MediatR;
using Nest;
using slack_bot_hive.Services;
using slack_bot_hive.Models;
using slack_bot_hive.QueryResults;

namespace slack_bot_hive.Queries
{
    public class DeleteNoteQuery : MediatR.IRequest<DeleteNoteQueryResult>
    {
        public string DocumentId { get; }
        public DeleteNoteQuery(string documentId)
        {
            DocumentId = documentId;
        }
    }

    public class DeleteNoteQueryHandler : IRequestHandler<DeleteNoteQuery, DeleteNoteQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public DeleteNoteQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }
        public async Task<DeleteNoteQueryResult> Handle(DeleteNoteQuery request, CancellationToken cancellationToken)
        {
            var response = await _elasticClient.DeleteAsync<Note>(new DocumentPath<Note>(new Id(request.DocumentId))
                .Index(Constants.Constants.IndexNameNote));

            switch (response.Result)
            {
                case Result.Deleted:
                    return new DeleteNoteQueryResult(QueryResult.Success);
                case Result.NotFound:
                    return new DeleteNoteQueryResult(QueryResult.NotFound);
                default:
                    return new DeleteNoteQueryResult(QueryResult.Failure);
            }
        }
    }
}
