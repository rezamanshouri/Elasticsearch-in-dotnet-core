namespace Elasticsearch.Index
{
    using System.Collections.Generic;

    using Elasticsearch.Index.Entities;

    public interface IElasticClientService
    {
        void BulkIndex<T>(List<T> entities, string indexName)
            where T : BaseEntity;
    }
}