namespace Elasticsearch.Search
{
    public interface ISearchService
    {
        string Search(string query, int from = 0, int size = 100);
    }
}