using System;
using System.Threading;
using System.Threading.Tasks;
using slack_bot_hive.Services.Elasticsearch;
using MediatR;
using slack_bot_hive.QueryResults;
using slack_bot_hive.Models;
using Nest;
using Elasticsearch.Net;

namespace slack_bot_hive.Queries
{
    public class CreateNoteQuery : MediatR.IRequest<CreateNoteQueryResult>
    {
        public Note Document { get; }
        public CreateNoteQuery(Note document)
        {
            Document = document;
        }
    }

    public class CreateNoteQueryHandler : IRequestHandler<CreateNoteQuery, CreateNoteQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public CreateNoteQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }
        public async Task<CreateNoteQueryResult> Handle(CreateNoteQuery request, CancellationToken cancellationToken)
        {
            request.Document.DocumentId = Guid.NewGuid().ToString();

            var response = await _elasticClient.IndexAsync<Note>(request.Document, p => p
                .Id(request.Document.DocumentId)
                .Index(Constants.Constants.IndexNameNote)
                .Refresh(Refresh.True));

            switch (response.Result)
            {
                case Result.Created:
                    return new CreateNoteQueryResult(QueryResult.Success);
                default:
                    return new CreateNoteQueryResult(QueryResult.Failure);
            }
        }
    }
}
