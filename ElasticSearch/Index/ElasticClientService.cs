namespace Elasticsearch.Index
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Elasticsearch.Index.Entities;
    using Elasticsearch.Net;
    using Elasticsearch.Utilities.AppSettingsModels;
    using Elasticsearch.Utilities.ElasticClientProvider;
    using Elasticsearch.Utilities.Exception;

    using Microsoft.Extensions.Logging;

    using Nest;

    public class ElasticClientService : IElasticClientService
    {
        private readonly IElasticClient elasticClient;

        private readonly ILogger logger;

        private readonly int maxDocumentsPerBatch;

        public ElasticClientService(
            ILogger<IElasticClientService> logger,
            IElasticClientProvider elasticClientProvider,
            IndexingSettings indexingSettings)
        {
            this.logger = logger;
            this.maxDocumentsPerBatch = indexingSettings.MaxDocumentsPerBatch;
            this.elasticClient = elasticClientProvider.Create(true);
        }

        public void BulkIndex<T>(List<T> entities, string indexName)
            where T : BaseEntity
        {
            /*
             * Reindexing data with zero downtime:
             *   - Create an index suffixed with '_v1', e.g. 'people_v1'
             *   - Create an alias for that index which can be used for searching, e.g. 'people'
             *   - To reindex the data:
             *     - Create a new index suffixed with '_v2', e.g. 'people_v2'
             *     - Swap the alias to point to the new index in a single atomic step
             *     - Delete the old index suffixed with '_v1'
             *
             * source: https://www.elastic.co/blog/changing-mapping-with-zero-downtime
             */

            if (!entities.Any())
            {
                this.logger.LogInformation($"There are no documents to be indexed to index '{indexName}'.");
                return;
            }

            var sanitizedIndexName = this.SanitizeIndexName(indexName);

            // To make this repeatable, v1 and v2 switch what they represent each time the index is recreated, i.e.
            // if v1/v2 represents the old/new index in the current run of the indexer, the next time it runs, v1/v2 represents the new/old index, and so on.
            var indexV1 = $"{sanitizedIndexName}_v1";
            var indexV2 = $"{sanitizedIndexName}_v2";

            var oldIndex = this.elasticClient.IndexExists(indexV2).Exists ? indexV2 : indexV1;
            var newIndex = oldIndex.Equals(indexV2) ? indexV1 : indexV2;

            // Delete newIndex in case both indexes exist.
            this.elasticClient.DeleteIndex(newIndex, x => x.RequestConfiguration(y => y.AllowedStatusCodes(404)));
            this.elasticClient.CreateIndex(newIndex, x => x.Settings(s => s.NumberOfShards(1)).Mappings(ms => ms.Map<T>(m => m.AutoMap())));

            // Depending on the type of Elasticsearch cluster, there might be a limit on the size (e.g. 10 MB) of the index request.
            // For this reason, we divide the entities into baches and index each batch separately.
            var documentBatches = new List<List<T>>();
            var count = entities.Count;
            for (var i = 0; i < count; i += this.maxDocumentsPerBatch)
            {
                documentBatches.Add(entities.GetRange(i, Math.Min(this.maxDocumentsPerBatch, count - i)));
            }

            foreach (var batch in documentBatches)
            {
                this.elasticClient.Bulk(b => b.Index(newIndex).IndexMany(batch));
            }

            // wait for newIndex to be operable.
            this.elasticClient.ClusterHealth(c => c.WaitForStatus(WaitForStatus.Yellow).Index(newIndex));
            this.elasticClient.Alias(
                a => a.Add(add => add.Alias(indexName).Index(newIndex)).Remove(remove => remove.Alias(indexName).Index("*")));

            // Allow Not Found status code for the very first run of the indexer.
            this.elasticClient.DeleteIndex(oldIndex, x => x.RequestConfiguration(y => y.AllowedStatusCodes(404)));
        }

        private string SanitizeIndexName(string indexName)
        {
            if (!string.IsNullOrWhiteSpace(indexName))
            {
                return indexName.ToLower();
            }

            var exception = new NullIndexNameException();
            this.logger.LogWarning(exception.Message);
            throw exception;
        }
    }
}