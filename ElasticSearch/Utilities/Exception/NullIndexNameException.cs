namespace Elasticsearch.Utilities.Exception
{
    using System;

    public class NullIndexNameException : Exception
    {
        public NullIndexNameException()
            : base("No index name is provided") { }

        public NullIndexNameException(Exception inner)
            : base("No index name is provided", inner) { }
    }
}