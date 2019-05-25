using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using slack_bot_hive.Services.Elasticsearch;
using MediatR;
using slack_bot_hive.QueryResults;
using slack_bot_hive.Models;
using Nest;

namespace slack_bot_hive.Queries
{
    public class GetAuthorQuery : MediatR.IRequest<GetAuthorQueryResult>
    {
        public string User { get; }
        public GetAuthorQuery(string user)
        {
            this.User = user;
        }
    }

    public class GetAuthorQueryHandler : IRequestHandler<GetAuthorQuery, GetAuthorQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public GetAuthorQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }

        public async Task<GetAuthorQueryResult> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<Author>(s => s
                   .Query(q => q
                       .Term(t => t
                            .Field(f => f
                                .SlackId == request.User
                                )
                            )
                        ).Index(Constants.Constants.IndexNameAuthor)
                    );
            
            if (searchResponse.Total > 0)
            {
                IReadOnlyCollection<Author> documents = searchResponse.Documents;

                Author author = documents.First();

                return new GetAuthorQueryResult(author, QueryResult.Success);
            }
            else if (searchResponse.IsValid)
            {
                return new GetAuthorQueryResult(null, QueryResult.NotFound);
            }

            return new GetAuthorQueryResult(null, QueryResult.Failure);
        }
    }
}
