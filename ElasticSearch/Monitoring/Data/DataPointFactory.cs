namespace Elasticsearch.Monitoring.Data
{
    using System.Collections.Generic;

    using Elasticsearch.Utilities.AppInfo;
    using Elasticsearch.Utilities.Time;

    public class DataPointFactory : IDataPointFactory
    {
        private readonly IAppInfoService appInfoService;

        private readonly IClock clock;

        public DataPointFactory(IAppInfoService appInfoService, IClock clock)
        {
            this.appInfoService = appInfoService;
            this.clock = clock;
        }

        public DataPoint CreateDataPoint(MetricReport metricReport)
        {
            var dataPoint = new DataPoint
            {
                Measurement = metricReport.Measurement,
                TimeStamp = this.clock.UtcNow,
                TagSet = new Dictionary<string, string> { ["Environment"] = this.appInfoService.GetEnvironment() },
                FieldSet = new Dictionary<string, double>()
            };

            foreach (var (key, value) in metricReport.GetFieldSet())
            {
                dataPoint.FieldSet[key] = value;
            }

            foreach (var (key, value) in metricReport.GetTagSet())
            {
                dataPoint.TagSet[key] = value;
            }

            return dataPoint;
        }
    }
}