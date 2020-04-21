namespace Elasticsearch.Utilities.ElasticClientProvider
{
    using Nest;

    public interface IElasticClientFactory
    {
        ElasticClient Create(string url);
    }
}