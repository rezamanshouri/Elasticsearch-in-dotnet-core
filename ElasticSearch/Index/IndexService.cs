namespace Elasticsearch.Index
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Elasticsearch.Index.Entities;
    using Elasticsearch.Monitoring;
    using Elasticsearch.Monitoring.Data;
    using Elasticsearch.Utilities.Time;

    using Microsoft.Extensions.Logging;

    public class IndexService : IIndexService
    {
        public const string PeopleIndexName = "people";

        private readonly IClock clock;

        private readonly IMetricsApiAgent metricsApiAgent;

        private readonly IDomainEntityService domainEntityService;

        private readonly IElasticClientService elasticClientService;

        private readonly ILogger logger;

        private List<Func<IndexingMetric>> indexingActions;

        public IndexService(
            ILogger<IIndexService> logger,
            IDomainEntityService domainEntityService,
            IMetricsApiAgent metricsApiAgent,
            IClock clock,
            IElasticClientService elasticClientService)
        {
            this.logger = logger;
            this.domainEntityService = domainEntityService;
            this.metricsApiAgent = metricsApiAgent;
            this.clock = clock;
            this.elasticClientService = elasticClientService;

            this.InitializeIndexingActions();
        }

        public void IndexAll()
       {
            this.logger.LogInformation("Starting to index data to Elasticsearch");
            var indexingMetrics = this.indexingActions.Select(x => x.Invoke()).ToList();
            this.metricsApiAgent.UploadData(indexingMetrics);
        }

        private IndexingMetric Index<T>(List<T> entities, string indexName)
            where T : BaseEntity
        {
            this.logger.LogInformation($"Starting to index {entities.Count} documents to index '{indexName}'");

            var metric = new IndexingMetric { IndexName = indexName, EntityCount = entities.Count };
            var stopwatch = new Stopwatch();
            try
            {
                stopwatch.Start();
                this.elasticClientService.BulkIndex(entities, indexName);
                stopwatch.Stop();
                var duration = stopwatch.Elapsed.TotalSeconds;

                this.logger.LogInformation($"Finished indexing {entities.Count} documents to index '{indexName}' in {duration} seconds");

                metric.DurationInSeconds = duration;
                metric.ExitCode = 0;
            }
            catch (Exception e)
            {
                // Failure of indexing one entity should not prevent us from attempting to index other entities.
                this.logger.LogError(e, $"Indexing documents to '{indexName}' failed.");

                stopwatch.Stop();
                metric.DurationInSeconds = stopwatch.Elapsed.TotalSeconds;
                metric.ExitCode = 1;
            }

            metric.Timestamp = this.clock.UtcNow;
            return metric;
        }

        private void InitializeIndexingActions()
        {
            this.indexingActions = new List<Func<IndexingMetric>>
            {
                () => this.Index(this.domainEntityService.GetPeople(), PeopleIndexName),
            };
        }
    }
}