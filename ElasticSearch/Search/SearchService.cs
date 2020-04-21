namespace Elasticsearch.Search
{
    using System.Diagnostics;
    using System.Text;

    using Elasticsearch.Index;
    using Elasticsearch.Utilities.ElasticClientProvider;

    using Microsoft.Extensions.Logging;

    using Nest;

    public class SearchService : ISearchService
    {
        private readonly IElasticClient elasticClient;

        private readonly ILogger logger;

        public SearchService(ILogger<ISearchService> logger, IElasticClientProvider elasticClientProvider)

        {
            this.logger = logger;

            this.elasticClient = elasticClientProvider.Create(false);
        }

        public string Search(string query, int from = 0, int size = 100)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var searchResponse = this.elasticClient.Search<dynamic>(
                s => s.AllTypes()
                    .Index(
                        new[]
                        {
                            IndexService.PeopleIndexName
                        }).IgnoreUnavailable().Size(size).From(from).Query(
                        q => q.Bool(b => b.Must(m => m.SimpleQueryString(c => c.Query(query).Lenient().AnalyzeWildcard())))));

            stopwatch.Stop();
            this.logger.LogInformation(
                $"Query for '{query}' returned {searchResponse.Total} hits in {stopwatch.Elapsed.TotalSeconds} seconds");

            return Encoding.Default.GetString(searchResponse.ApiCall.ResponseBodyInBytes);
        }
    }
}