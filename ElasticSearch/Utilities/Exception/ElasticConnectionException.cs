namespace Elasticsearch.Utilities.Exception
{
    using System;

    public class ElasticConnectionException : Exception
    {
        public ElasticConnectionException(string missingValues)
            : base($"Elasticsearch connection {missingValues} cannot be retrieved.") { }

        public ElasticConnectionException(string missingValues, Exception inner)
            : base($"Elasticsearch connection {missingValues} cannot be retrieved.", inner) { }
    }
}