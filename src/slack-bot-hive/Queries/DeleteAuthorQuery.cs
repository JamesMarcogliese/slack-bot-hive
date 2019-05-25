using System.Threading;
using System.Threading.Tasks;
using slack_bot_hive.Services.Elasticsearch;
using MediatR;
using Nest;
using slack_bot_hive.Models;
using slack_bot_hive.QueryResults;

namespace slack_bot_hive.Queries
{
    public class DeleteAuthorQuery : MediatR.IRequest<DeleteAuthorQueryResult>
    {
        public string DocumentId { get; }
        public DeleteAuthorQuery(string documentId)
        {
            DocumentId = documentId;
        }
    }

    public class DeleteAuthorQueryHandler : IRequestHandler<DeleteAuthorQuery, DeleteAuthorQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public DeleteAuthorQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }
        public async Task<DeleteAuthorQueryResult> Handle(DeleteAuthorQuery request, CancellationToken cancellationToken)
        {
            var response = await _elasticClient.DeleteAsync<Author>(new DocumentPath<Author>(new Id(request.DocumentId))
                .Index(Constants.Constants.IndexNameAuthor));

            switch (response.Result)
            {
                case Result.Deleted:
                    return new DeleteAuthorQueryResult(QueryResult.Success);
                case Result.NotFound:
                    return new DeleteAuthorQueryResult(QueryResult.NotFound);
                default:
                    return new DeleteAuthorQueryResult(QueryResult.Failure);
            }
        }
    }
}
