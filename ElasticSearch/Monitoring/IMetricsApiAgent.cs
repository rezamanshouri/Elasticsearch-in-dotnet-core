namespace Elasticsearch.Monitoring
{
    using System.Collections.Generic;

    using Elasticsearch.Monitoring.Data;

    public interface IMetricsApiAgent
    {
        void UploadData<T>(IList<T> metricReports)
            where T : MetricReport;
    }
}