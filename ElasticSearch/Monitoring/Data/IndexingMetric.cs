namespace Elasticsearch.Monitoring.Data
{
    using System;
    using System.Collections.Generic;

    public class IndexingMetric : MetricReport
    {
        public double DurationInSeconds { get; set; }

        public int EntityCount { get; set; }

        public int ExitCode { get; set; }

        public string IndexName { get; set; }

        public override string Measurement
        {
            get => "indexing";
            set { }
        }

        public override Dictionary<string, double> GetFieldSet()
        {
            return new Dictionary<string, double>
            {
                ["Count"] = this.EntityCount, ["Duration"] = this.DurationInSeconds, ["ExitCode"] = this.ExitCode
            };
        }

        public override Dictionary<string, string> GetTagSet()
        {
            return new Dictionary<string, string> { ["IndexName"] = this.IndexName };
        }
    }
}