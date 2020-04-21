namespace Elasticsearch.Utilities.Time
{
    using System;

    public class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}