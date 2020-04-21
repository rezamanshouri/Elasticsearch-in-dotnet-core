namespace Elasticsearch.Utilities.ElasticClientProvider
{
    using Nest;

    public interface IElasticClientProvider
    {
        IElasticClient Create(bool isWriter);
    }
}