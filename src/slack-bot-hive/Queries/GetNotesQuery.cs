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
    public class GetNotesQuery : MediatR.IRequest<GetNotesQueryResult>
    {
        public string User { get; }
        public string Team { get; }
        public string Keywords { get; }
        public GetNotesQueryType QueryType { get; }
        public GetNotesQuery(string user,  string team, string keywords, GetNotesQueryType queryType)
        {
            switch (queryType)
            {
                case GetNotesQueryType.GetUserNotes:
                    if (string.IsNullOrWhiteSpace(user))
                        throw new System.Exception("GetNotesQueryType of GetUserNotes requires a valid User parameter.");
                    User = user;
                    QueryType = queryType;
                    break;
                case GetNotesQueryType.SearchTeamNotes:
                    if (string.IsNullOrWhiteSpace(team) || string.IsNullOrWhiteSpace(keywords))
                        throw new System.Exception("GetNotesQueryType of SearchTeamNotes requires valid Team and Keyword parameters.");
                    Team = team;
                    Keywords = keywords;
                    QueryType = queryType;
                    break;
                case GetNotesQueryType.SearchUserNotes:
                    if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(keywords))
                        throw new System.Exception("GetNotesQueryType of SearchUserNotes requires valid User and Keyword parameters.");
                    User = user;
                    Keywords = keywords;
                    QueryType = queryType;
                    break;
            }
        }

    }

    public class GetNotesQueryHandler : IRequestHandler<GetNotesQuery, GetNotesQueryResult>
    {
        private readonly IElasticClient _elasticClient;
        public GetNotesQueryHandler(IElasticsearchClientProvider elasticsearchClientProvider)
        {
            _elasticClient = elasticsearchClientProvider.GetElasticClient();
        }

        public async Task<GetNotesQueryResult> Handle(GetNotesQuery request, CancellationToken cancellationToken)
        {

            switch (request.QueryType)
            {
                case GetNotesQueryType.GetUserNotes:
                    return await GetUserNotes(request, cancellationToken);
                case GetNotesQueryType.SearchTeamNotes:
                    return await SearchTeamNotes(request, cancellationToken);
                case GetNotesQueryType.SearchUserNotes:
                    return await SearchUserNotes(request, cancellationToken);
                default:
                    throw new System.Exception("GetNotesQueryType cannot be null.");
            }
        }

        public async Task<GetNotesQueryResult> GetUserNotes (GetNotesQuery request, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<Note>(s => s
                .Query(q => q
                    .Match(m => m
                        .Field(f => f
                            .Author == request.User
                            )
                        )
                    ).Sort(so => so 
                        .Descending(d => d
                            .Timestamp)
                        ).Index(Constants.Constants.IndexNameNote
                    ).Scroll("5m")
                );

            if (searchResponse.Total > 0)
            {
                List<Note> documents = new List<Note>();
                documents.AddRange(searchResponse.Documents);

                var results = _elasticClient.Scroll<Note>("5m", searchResponse.ScrollId);

                while (results.Documents.Any())
                {
                    documents.AddRange(results.Documents);
                    results = _elasticClient.Scroll<Note>("5m", results.ScrollId);
                }

                return new GetNotesQueryResult(documents, QueryResult.Success);
            }

            if (searchResponse.IsValid)
            {
                return new GetNotesQueryResult(null, QueryResult.NotFound);
            }

            return new GetNotesQueryResult(null, QueryResult.Failure);
        }

        public async Task<GetNotesQueryResult> SearchTeamNotes(GetNotesQuery request, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<Note>(s => s
                .Query(q => q
                    .Bool(b => b
                        .Must(m => m
                            .Term(t => t
                                .Field(f => f
                                    .Team == request.Team
                                    )
                                )
                            ).Must(m => m
                                .CommonTerms(c => c
                                    .Field(f => f.Text)
                                        .Query(request.Keywords)
                                    )
                                ).MustNot(mn => mn
                                    .Term(t => t
                                        .Field(f => f
                                            .Author == request.User)))
                        )
                    ).Index(Constants.Constants.IndexNameNote)
                );

            if (searchResponse.Total > 0)
            {
                IEnumerable<Note> documents = searchResponse.Documents.AsEnumerable<Note>();

                return new GetNotesQueryResult(documents, QueryResult.Success);
            }
            else if (searchResponse.IsValid)
            {
                return new GetNotesQueryResult(null, QueryResult.NotFound);
            }

            return new GetNotesQueryResult(null, QueryResult.Failure);
        }

        public async Task<GetNotesQueryResult> SearchUserNotes(GetNotesQuery request, CancellationToken cancellationToken)
        {
            var searchResponse = await _elasticClient.SearchAsync<Note>(s => s
               .Query(q => q
                   .Bool(b => b
                       .Must(m => m
                           .Term(t => t
                               .Field(f => f
                                   .Author == request.User
                                   )
                               )
                           ).Must(m => m
                               .CommonTerms(c => c
                                   .Field(f => f.Text)
                                       .Query(request.Keywords)
                                    )
                                )
                       )
                   ).Index(Constants.Constants.IndexNameNote)
               );

            if (searchResponse.Total > 0)
            {
                IEnumerable<Note> documents = searchResponse.Documents.AsEnumerable<Note>();

                return new GetNotesQueryResult(documents, QueryResult.Success);
            }
            else if (searchResponse.IsValid)
            {
                return new GetNotesQueryResult(null, QueryResult.NotFound);
            }

            return new GetNotesQueryResult(null, QueryResult.Failure);
        }
    }
}
