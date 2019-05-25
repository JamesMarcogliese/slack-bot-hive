using System;
using System.Threading;
using System.Threading.Tasks;
using slack_bot_hive.Services.Elasticsearch;
using MediatR;
using Nest;
using slack_bot_hive.Models;
using slack_bot_hive.QueryResults;
using Elasticsearch.Net;

namespace slack_bot_hive.Queries
{
    public class CreateAuthorQuery : MediatR.IRequest<CreateAuthorQueryResult>
    {
        public Author Document { get; }
        public CreateAuthorQuery(Author document) => Document = document;
    }

    public class CreateAuthorQueryHandler : IRequestHandler<CreateAuthorQuery, CreateAuthorQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public CreateAuthorQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }

        public async Task<CreateAuthorQueryResult> Handle(CreateAuthorQuery request, CancellationToken cancellationToken)
        {
            request.Document.DocumentId = Guid.NewGuid().ToString();

            var response = await _elasticClient.IndexAsync<Author>(request.Document, p => p
                .Id(request.Document.DocumentId)
                .Index(Constants.Constants.IndexNameAuthor)
                .Refresh(Refresh.True));

            switch (response.Result)
            {
                case Result.Created:
                    return new CreateAuthorQueryResult(QueryResult.Success);
                default:
                    return new CreateAuthorQueryResult(QueryResult.Failure);
            }
        }
    }
}
