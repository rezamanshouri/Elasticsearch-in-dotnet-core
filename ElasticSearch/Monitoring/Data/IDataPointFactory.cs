namespace Elasticsearch.Monitoring.Data
{
    public interface IDataPointFactory
    {
        DataPoint CreateDataPoint(MetricReport metricReport);
    }
}