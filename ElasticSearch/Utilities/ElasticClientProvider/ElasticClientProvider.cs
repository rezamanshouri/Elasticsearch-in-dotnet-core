namespace Elasticsearch.Utilities.ElasticClientProvider
{
    using System.Linq;

    using Elasticsearch.Utilities.AppSettingsModels;
    using Elasticsearch.Utilities.Exception;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using Nest;

    public class ElasticClientProvider : IElasticClientProvider
    {
        private readonly ElasticConnection connection;

        private readonly IElasticClientFactory elasticClientFactory;

        private readonly ILogger logger;

        private readonly string readerPassword;

        private readonly string readerUsername;

        private readonly string writerPassword;

        private readonly string writerUsername;

        public ElasticClientProvider(
            ILogger<IElasticClientProvider> logger,
            IConfiguration config,
            IElasticClientFactory elasticClientFactory,
            ElasticConnection connection)
        {
            this.elasticClientFactory = elasticClientFactory;
            this.logger = logger;

            this.connection = connection;

            // Reading credentials from IConfiguration directly allows to have credentials be stored either 
            // in the appsettings file (for development), or in the environment variables (for staging/production).
            this.readerUsername = config.GetValue<string>("ES_READER_USERNAME");
            this.readerPassword = config.GetValue<string>("ES_READER_PASSWORD");
            this.writerUsername = config.GetValue<string>("ES_WRITER_USERNAME");
            this.writerPassword = config.GetValue<string>("ES_WRITER_PASSWORD");

            var missingValues = string.Join(
                "/",
                new[]
                {
                    this.readerUsername == null ? "Reader Username" : null, this.readerPassword == null ? "Reader Password" : null,
                    this.writerUsername == null ? "Writer Username" : null, this.writerPassword == null ? "Writer Password" : null
                }.Where(x => x != null));
            if (!string.IsNullOrEmpty(missingValues))
            {
                var exception = new ElasticConnectionException(missingValues);
                this.logger.LogWarning(exception.Message);
                throw exception;
            }
        }

        public IElasticClient Create(bool isWriter)
        {
            var username = isWriter ? this.writerUsername : this.readerUsername;
            var password = isWriter ? this.writerPassword : this.readerPassword;

            return this.elasticClientFactory.Create($"https://{username}:{password}@{this.connection.Url}");
        }
    }
}