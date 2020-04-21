namespace Elasticsearch.Utilities.Time
{
    using System;

    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}