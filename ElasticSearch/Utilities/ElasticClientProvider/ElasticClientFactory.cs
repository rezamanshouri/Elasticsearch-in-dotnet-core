namespace Elasticsearch.Utilities.ElasticClientProvider
{
    using System;

    using Nest;

    public class ElasticClientFactory : IElasticClientFactory
    {
        public ElasticClient Create(string url)
        {
            var connectionSettings = new ConnectionSettings(new Uri(url));

            // Ensure that any elastic client methods bubble up exceptions from the client or server.
            // Enable seeing raw queries sent to Elasticsearch when debugging.
            connectionSettings.DisableDirectStreaming().ThrowExceptions().EnableDebugMode();

            return new ElasticClient(connectionSettings);
        }
    }
}