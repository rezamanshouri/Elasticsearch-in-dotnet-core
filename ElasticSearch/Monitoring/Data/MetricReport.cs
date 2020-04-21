namespace Elasticsearch.Monitoring.Data
{
    using System;
    using System.Collections.Generic;

    public abstract class MetricReport
    {
        public abstract string Measurement { get; set; }

        public virtual DateTime Timestamp { get; set; }

        public abstract Dictionary<string, double> GetFieldSet();

        public abstract Dictionary<string, string> GetTagSet();
    }
}