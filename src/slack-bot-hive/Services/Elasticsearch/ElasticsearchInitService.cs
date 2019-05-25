using System.Collections.Generic;
using System.Threading.Tasks;
using slack_bot_hive.Queries;
using MediatR;

namespace slack_bot_hive.Services.Elasticsearch
{
    public interface IElasticsearchInitService
    {
        void InitElasticsearchIndices();
    }

    public class ElasticsearchInitService : IElasticsearchInitService
    {
        private readonly IMediator _mediator;
        public ElasticsearchInitService(IMediator mediator)
        {
            _mediator = mediator;

        }

        public void InitElasticsearchIndices()
        {
            List<string> listOfIndexes = new List<string>()
            {
                slack_bot_hive.Constants.Constants.IndexNameAuthor,
                slack_bot_hive.Constants.Constants.IndexNameNote
            };

            if (!IndicesExist(listOfIndexes))
                CreateIndices();
        }

        public bool IndicesExist(IEnumerable<string> indexNames)
        {
            bool indicesExist = true;

            foreach (string indexName in indexNames)
            {
                var result = _mediator.Send(new CheckIndexQuery(indexName));
                indicesExist = indicesExist && result.Result.Exists;
            }

            return indicesExist;
        }

        public async Task CreateIndices()
        {
            await _mediator.Send(new CreateIndicesQuery());
        }
    }
}
